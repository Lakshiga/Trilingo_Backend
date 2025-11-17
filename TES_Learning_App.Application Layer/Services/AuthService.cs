using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TES_Learning_App.Application_Layer.DTOs.Auth.Requests;
using TES_Learning_App.Application_Layer.DTOs.Auth.Response;
using TES_Learning_App.Application_Layer.Interfaces.Infrastructure;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IS3Service _s3Service;
        //public AuthService(IAuthRepository authRepository, ITokenService tokenService)
        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IS3Service s3Service)
        {
            _unitOfWork = unitOfWork;
            //_authRepository = authRepository;
            _tokenService = tokenService;
            _s3Service = s3Service;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                // --- MANUAL VALIDATION LOGIC ---
                var validationErrors = ValidateLogin(dto);

                // If there are any validation errors, stop immediately.
                if (validationErrors.Any())
                {
                    // We return a failure response with a list of all the errors.
                    return new AuthResponseDto { IsSuccess = false, Message = string.Join(" ", validationErrors) };
                }

                var user = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(dto.Identifier);
                //var user = await _authRepository.GetUserByIdentifierAsync(dto.Identifier);

                if (user == null) return new AuthResponseDto { IsSuccess = false, Message = "Invalid credentials." };

                // Validate that user has password hash and salt
                if (user.PasswordSalt == null || user.PasswordHash == null)
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "Invalid user account configuration." };
                }

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
                
                // Validate password hash length matches
                if (computedHash.Length != user.PasswordHash.Length)
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "Invalid credentials." };
                }

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                        return new AuthResponseDto { IsSuccess = false, Message = "Invalid credentials." };
                }

                var token = _tokenService.CreateToken(user);
                
                // Convert legacy local file paths to S3 URLs if needed
                string? profileImageUrl = null;
                try
                {
                    profileImageUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) 
                        ? _s3Service.GetFileUrl(user.ProfileImageUrl)
                        : null;
                }
                catch
                {
                    // If S3 service fails, continue without profile image URL
                    profileImageUrl = null;
                }

                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "Login successful.", 
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                    ProfileImageUrl = profileImageUrl,
                    Role = user.Role?.RoleName
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = $"An error occurred during login: {ex.Message}" 
                };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var validationErrors = ValidateRegistration(dto);

            if (validationErrors.Any())
            {
                return new AuthResponseDto { IsSuccess = false, Message = string.Join(" ", validationErrors) };
            }

            if (await _unitOfWork.AuthRepository.UserExistsAsync(dto.Email))
            //if (await _authRepository.UserExistsAsync(dto.Email))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Email is already taken." };
            }
            var user = new User { Username = dto.Username, Email = dto.Email.ToLower() };

            var createdUser = await _unitOfWork.AuthRepository.RegisterAsync(user, dto.Password);
            //var createdUser = await _authRepository.RegisterAsync(user, dto.Password);

            await _unitOfWork.CompleteAsync();

            // We can log the user in immediately after they register
            var loginDto = new LoginDto
            {
                Identifier = createdUser.Email,
                Password = dto.Password // Use the original password from the registration DTO
            };

            return await LoginAsync(loginDto);
        }

        public async Task<object> CheckAdminUserAsync()
        {
            var adminUser = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync("admin");
            if (adminUser == null)
            {
                return new { exists = false, message = "Admin user not found" };
            }
            return new { 
                exists = true, 
                username = adminUser.Username, 
                email = adminUser.Email, 
                role = adminUser.Role?.RoleName 
            };
        }

        public async Task<object> CreateAdminUserAsync()
        {
            try
            {
                // Check if admin user already exists
                var existingAdmin = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync("admin");
                if (existingAdmin != null)
                {
                    // Update existing admin user to have Admin role
                    var adminRole = await _unitOfWork.RoleRepository.GetAllAsync();
                    var adminRoleEntity = adminRole.FirstOrDefault(r => r.RoleName == "Admin");
                    if (adminRoleEntity != null)
                    {
                        existingAdmin.RoleId = adminRoleEntity.Id;
                        existingAdmin.Role = adminRoleEntity;
                        await _unitOfWork.CompleteAsync();
                        return new { success = true, message = "Existing admin user updated with Admin role" };
                    }
                }

                // Get the Admin role
                var adminRoleList = await _unitOfWork.RoleRepository.GetAllAsync();
                var adminRoleForNewUser = adminRoleList.FirstOrDefault(r => r.RoleName == "Admin");
                if (adminRoleForNewUser == null)
                {
                    return new { success = false, message = "Admin role not found" };
                }

                // Create password hash for default admin
                CreatePasswordHash("Admin123!", out byte[] passwordHash, out byte[] passwordSalt);

                // Create default admin user
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@teslearning.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    RoleId = adminRoleForNewUser.Id,
                    Role = adminRoleForNewUser
                };

                // Add the user directly to avoid RegisterAsync assigning Parent role
                await _unitOfWork.UserRepository.AddAsync(adminUser);
                await _unitOfWork.CompleteAsync();

                return new { success = true, message = "Admin user created successfully" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = $"Error creating admin user: {ex.Message}" };
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // --- THE NEW, DEDICATED VALIDATOR FOR LOGIN ---
        private List<string> ValidateLogin(LoginDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Identifier))
                errors.Add("Username or Email is required.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            return errors;
        }

        private List<string> ValidateRegistration(RegisterDto dto)
        {
            var errors = new List<string>();

            // Username Validation
            if (string.IsNullOrWhiteSpace(dto.Username))
                errors.Add("Username is required.");
            else if (dto.Username.Length > 100)
                errors.Add("Username cannot be longer than 100 characters.");

            // Email Validation (using a more robust Regex)
            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else
            {
                // Industrial Practice: Use a Regular Expression (Regex) for robust email format validation.
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                if (!emailRegex.IsMatch(dto.Email))
                    errors.Add("A valid email address is required.");
            }

            // Password Validation (with added complexity rules)
            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            else
            {
                if (dto.Password.Length < 8)
                    errors.Add("Password must be at least 8 characters long.");
                if (!Regex.IsMatch(dto.Password, "[A-Z]"))
                    errors.Add("Password must contain at least one uppercase letter.");
                if (!Regex.IsMatch(dto.Password, "[a-z]"))
                    errors.Add("Password must contain at least one lowercase letter.");
                if (!Regex.IsMatch(dto.Password, "[0-9]"))
                    errors.Add("Password must contain at least one number.");
                if (!Regex.IsMatch(dto.Password, "[^a-zA-Z0-9]"))
                    errors.Add("Password must contain at least one special character.");
            }

            return errors;
        }

        public async Task<AuthResponseDto> UploadProfileImageAsync(string username, IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "Username is required" };
                }

                var user = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(username);
                if (user == null)
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "User not found" };
                }

                // Generate unique filename for S3
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"profiles/{user.Id}_{Guid.NewGuid()}{fileExtension}";

                // Determine content type
                var contentType = file.ContentType ?? "application/octet-stream";
                if (contentType == "application/octet-stream")
                {
                    // Try to determine from extension
                    contentType = fileExtension.ToLower() switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".webp" => "image/webp",
                        _ => "image/jpeg"
                    };
                }

                // Upload to S3
                string imageUrl;
                using (var stream = file.OpenReadStream())
                {
                    imageUrl = await _s3Service.UploadFileAsync(uniqueFileName, stream, contentType);
                }

                // If user has an existing profile image in S3, delete it
                if (!string.IsNullOrEmpty(user.ProfileImageUrl) && !user.ProfileImageUrl.StartsWith("/uploads"))
                {
                    // Extract the key from the URL
                    var existingKey = user.ProfileImageUrl.Contains("/profiles/") 
                        ? user.ProfileImageUrl.Substring(user.ProfileImageUrl.IndexOf("/profiles/") + 1)
                        : null;
                    
                    if (!string.IsNullOrEmpty(existingKey))
                    {
                        await _s3Service.DeleteFileAsync(existingKey);
                    }
                }

                // Update user profile image URL
                user.ProfileImageUrl = imageUrl;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "Profile image uploaded successfully",
                    ProfileImageUrl = imageUrl
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = $"Error uploading profile image: {ex.Message}" };
            }
        }

        public async Task<AuthResponseDto> GetUserProfileAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "Username is required" };
                }

                var user = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(username);
                if (user == null)
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "User not found" };
                }

                // Convert legacy local file paths to S3 URLs if needed
                var profileImageUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) 
                    ? _s3Service.GetFileUrl(user.ProfileImageUrl)
                    : null;

                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "User profile retrieved successfully",
                    Username = user.Username,
                    Email = user.Email,
                    ProfileImageUrl = profileImageUrl,
                    Role = user.Role?.RoleName
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = $"Error getting user profile: {ex.Message}" };
            }
        }

        public async Task<AuthResponseDto> UpdateProfileAsync(string username, UpdateProfileDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "Username is required" };
                }

                var user = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(username);
                if (user == null)
                {
                    return new AuthResponseDto { IsSuccess = false, Message = "User not found" };
                }

                // Update only non-null fields
                bool updated = false;

                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
                {
                    // Check if email is already in use by another user
                    var existingUser = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(dto.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        return new AuthResponseDto { IsSuccess = false, Message = "Email already in use" };
                    }
                    user.Email = dto.Email;
                    updated = true;
                }

                if (!string.IsNullOrEmpty(dto.ProfileImageUrl) && dto.ProfileImageUrl != user.ProfileImageUrl)
                {
                    user.ProfileImageUrl = dto.ProfileImageUrl;
                    updated = true;
                }

                // Note: Name, Age, NativeLanguage, LearningLanguage are not in the User entity
                // You may need to add these fields to User entity or create a UserProfile entity

                if (updated)
                {
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                    await _unitOfWork.CompleteAsync();
                }

                // Convert legacy local file paths to S3 URLs if needed
                var profileImageUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) 
                    ? _s3Service.GetFileUrl(user.ProfileImageUrl)
                    : null;

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Profile updated successfully",
                    Username = user.Username,
                    Email = user.Email,
                    ProfileImageUrl = profileImageUrl
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = $"Error updating profile: {ex.Message}" };
            }
        }

    }
}

