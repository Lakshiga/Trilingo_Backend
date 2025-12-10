using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Admin.Requests;
using TES_Learning_App.Application_Layer.DTOs.Admin.Response;
using TES_Learning_App.Application_Layer.DTOs.Common;
using TES_Learning_App.Application_Layer.Exceptions;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDto> CreateAdminAsync(CreateAdminDto dto)
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ValidationException("Email", new[] { "Email is required." });
            }

            // Check if email already exists
            if (await _unitOfWork.AuthRepository.UserExistsAsync(dto.Email))
            {
                throw new ValidationException("Email", new[] { "Email is already taken." });
            }

            // Check if username already exists
            var existingUser = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(dto.Username);
            if (existingUser != null)
            {
                throw new ValidationException("Username", new[] { "Username is already taken." });
            }

            // Validate password strength
            var passwordErrors = ValidatePassword(dto.Password);
            if (passwordErrors.Any())
            {
                throw new ValidationException("Password", passwordErrors.ToDictionary(k => "Password", v => new[] { v }));
            }

            // Validate role exists
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }

            // Validate role is Admin or SuperAdmin
            if (role.RoleName != "Admin" && role.RoleName != "SuperAdmin")
            {
                throw new ValidationException("RoleId", new[] { "Role must be Admin or SuperAdmin." });
            }

            // Create password hash
            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = dto.RoleId,
                Role = role,
                IsFirstLogin = true,
                MustChangePassword = dto.MustChangePassword
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Create admin profile
            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                FullName = dto.FullName,
                JobTitle = dto.JobTitle,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AdminRepository.AddAsync(admin);
            await _unitOfWork.CompleteAsync();

            return MapToAdminDto(user, admin);
        }

        public async Task<AdminDto?> GetAdminByIdAsync(Guid adminId)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                return null;
            }

            // Load user with role
            var user = await _unitOfWork.UserRepository.GetByIdAsync(admin.UserId);
            if (user == null)
            {
                return null;
            }

            // Eager load role
            if (user.Role == null)
            {
                user.Role = await _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId);
            }

            return MapToAdminDto(user, admin);
        }

        public async Task<IEnumerable<AdminListDto>> GetAllAdminsAsync()
        {
            var admins = await _unitOfWork.AdminRepository.GetAllAsync();
            var adminList = new List<AdminListDto>();

            foreach (var admin in admins)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(admin.UserId);
                if (user != null)
                {
                    if (user.Role == null)
                    {
                        user.Role = await _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId);
                    }

                    adminList.Add(new AdminListDto
                    {
                        Id = admin.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = admin.FullName,
                        JobTitle = admin.JobTitle,
                        RoleName = user.Role?.RoleName ?? "Unknown",
                        CreatedAt = admin.CreatedAt,
                        IsFirstLogin = user.IsFirstLogin,
                        MustChangePassword = user.MustChangePassword
                    });
                }
            }

            return adminList.OrderByDescending(a => a.CreatedAt);
        }

        public async Task<AdminDto> UpdateAdminAsync(Guid adminId, UpdateAdminDto dto)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("Admin not found.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(admin.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            bool updated = false;

            // Update username if provided
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                var existingUser = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(dto.Username);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    throw new ValidationException("Username", new[] { "Username is already taken." });
                }
                user.Username = dto.Username;
                updated = true;
            }

            // Update email if provided
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email.ToLower() != user.Email.ToLower())
            {
                if (await _unitOfWork.AuthRepository.UserExistsAsync(dto.Email))
                {
                    var existingUser = await _unitOfWork.AuthRepository.GetUserByIdentifierAsync(dto.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        throw new ValidationException("Email", new[] { "Email is already taken." });
                    }
                }
                user.Email = dto.Email.ToLower();
                updated = true;
            }

            // Update admin profile fields
            if (!string.IsNullOrWhiteSpace(dto.FullName) && dto.FullName != admin.FullName)
            {
                admin.FullName = dto.FullName;
                updated = true;
            }

            if (dto.JobTitle != null && dto.JobTitle != admin.JobTitle)
            {
                admin.JobTitle = dto.JobTitle;
                updated = true;
            }

            // Update role if provided
            if (dto.RoleId.HasValue && dto.RoleId.Value != user.RoleId)
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId.Value);
                if (role == null)
                {
                    throw new KeyNotFoundException("Role not found.");
                }

                if (role.RoleName != "Admin" && role.RoleName != "SuperAdmin")
                {
                    throw new ValidationException("RoleId", new[] { "Role must be Admin or SuperAdmin." });
                }

                user.RoleId = dto.RoleId.Value;
                user.Role = role;
                updated = true;
            }

            // Update password change requirement
            if (dto.MustChangePassword.HasValue && dto.MustChangePassword.Value != user.MustChangePassword)
            {
                user.MustChangePassword = dto.MustChangePassword.Value;
                if (dto.MustChangePassword.Value)
                {
                    user.IsFirstLogin = true; // Reset first login flag
                }
                updated = true;
            }

            if (updated)
            {
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.AdminRepository.UpdateAsync(admin);
                await _unitOfWork.CompleteAsync();
            }

            // Reload role if needed
            if (user.Role == null)
            {
                user.Role = await _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId);
            }

            return MapToAdminDto(user, admin);
        }

        public async Task DeleteAdminAsync(Guid adminId)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("Admin not found.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(admin.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Check if this is the last SuperAdmin
            if (user.Role?.RoleName == "SuperAdmin")
            {
                var allAdmins = await _unitOfWork.AdminRepository.GetAllAsync();
                var superAdminCount = 0;
                foreach (var a in allAdmins)
                {
                    var u = await _unitOfWork.UserRepository.GetByIdAsync(a.UserId);
                    if (u?.Role?.RoleName == "SuperAdmin")
                    {
                        superAdminCount++;
                    }
                }

                if (superAdminCount <= 1)
                {
                    throw new ValidationException("Admin", new[] { "Cannot delete the last SuperAdmin." });
                }
            }

            // Delete admin profile first (cascade will handle user deletion)
            await _unitOfWork.AdminRepository.DeleteAsync(admin);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<AdminDto> ResetAdminPasswordAsync(Guid adminId, ResetAdminPasswordDto dto)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("Admin not found.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(admin.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Validate password strength
            var passwordErrors = ValidatePassword(dto.NewPassword);
            if (passwordErrors.Any())
            {
                throw new ValidationException("NewPassword", passwordErrors.ToDictionary(k => "NewPassword", v => new[] { v }));
            }

            // Update password
            CreatePasswordHash(dto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.MustChangePassword = dto.MustChangePassword;
            user.IsFirstLogin = dto.MustChangePassword; // Set first login if password change required

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            // Reload role if needed
            if (user.Role == null)
            {
                user.Role = await _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId);
            }

            return MapToAdminDto(user, admin);
        }

        // Helper methods
        private AdminDto MapToAdminDto(User user, Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = admin.FullName,
                JobTitle = admin.JobTitle,
                ProfileImageUrl = user.ProfileImageUrl,
                RoleName = user.Role?.RoleName ?? "Unknown",
                RoleId = user.RoleId,
                CreatedAt = admin.CreatedAt,
                IsFirstLogin = user.IsFirstLogin,
                MustChangePassword = user.MustChangePassword
            };
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required.");
                return errors;
            }

            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");
            if (!Regex.IsMatch(password, "[A-Z]"))
                errors.Add("Password must contain at least one uppercase letter.");
            if (!Regex.IsMatch(password, "[a-z]"))
                errors.Add("Password must contain at least one lowercase letter.");
            if (!Regex.IsMatch(password, "[0-9]"))
                errors.Add("Password must contain at least one number.");
            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                errors.Add("Password must contain at least one special character.");

            return errors;
        }
    }
}
