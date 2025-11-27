using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Auth;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.DTOs.Auth.Requests;
using TES_Learning_App.Application_Layer.DTOs.Auth.Response;
using Microsoft.AspNetCore.Http;

namespace TES_Learning_App.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var result = await _authService.RegisterAsync(dto);
                if (!result.IsSuccess) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"An error occurred during registration: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

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
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            try
            {
                var result = await _authService.UploadProfileImageAsync(User.Identity?.Name, file);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading profile image", error = ex.Message });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

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