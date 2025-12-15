using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer    .DTOs.Auth;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.DTOs.Auth.Requests;
using TES_Learning_App.Application_Layer.DTOs.Auth.Response;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TES_Learning_App.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                if (!result.IsSuccess) return Unauthorized(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = $"An error occurred during login: {ex.Message}" 
                });
            }
        }

        // Example of a secure endpoint
        [HttpGet("test-auth")]
        [Authorize(Roles = "Parent")] // Only a logged-in user with the "Parent" role can access this
        public IActionResult TestAuth()
        {
            // We can get the logged-in user's info from the token claims
            var username = User.Identity?.Name;
            return Ok($"Hello, {username}! You have successfully accessed a secure endpoint.");
        }

        [HttpGet("check-admin")]
        public async Task<IActionResult> CheckAdmin()
        {
            var adminUser = await _authService.CheckAdminUserAsync();
            return Ok(adminUser);
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var result = await _authService.CreateAdminUserAsync();
            return Ok(result);
        }

        [HttpPost("upload-profile-image")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "No file uploaded. Please select an image file." 
                });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "Invalid file type. Please upload a JPG, PNG, GIF, or WEBP image." 
                });
            }

            // Validate file size (5MB max)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = $"File size exceeds the maximum limit of 5MB. Your file is {file.Length / (1024.0 * 1024.0):F2}MB." 
                });
            }

            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new AuthResponseDto 
                    { 
                        IsSuccess = false, 
                        Message = "User not found. Please log in again." 
                    });
                }

                var result = await _authService.UploadProfileImageAsync(username, file);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = $"Error uploading profile image: {ex.Message}" 
                });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var result = await _authService.GetUserProfileAsync(username);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting user profile", error = ex.Message });
            }
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var result = await _authService.UpdateProfileAsync(username, dto);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating profile", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Token-based logout is handled client-side by removing the token
            // This endpoint is here for compatibility and potential future server-side session handling
            return Ok(new { message = "Logged out successfully" });
        }
    }
}