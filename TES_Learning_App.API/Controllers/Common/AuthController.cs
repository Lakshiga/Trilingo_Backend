using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Controllers.Common;
using TES_Learning_App.Application_Layer.DTOs.Auth.Requests;
using TES_Learning_App.Application_Layer.DTOs.Auth.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Common
{
    /// <summary>
    /// Common authentication controller for all users
    /// Route: api/auth
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// POST: api/auth/register
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Login and get JWT token
        /// POST: api/auth/login?admin=true (for admin panel)
        /// POST: api/auth/login (for regular users)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromQuery] bool admin = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // If admin=true query parameter is present, use admin login validation
                var result = admin 
                    ? await _authService.AdminLoginAsync(dto)
                    : await _authService.LoginAsync(dto);
                
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

        /// <summary>
        /// Admin panel login - Only allows Admin users
        /// POST: api/auth/admin-login
        /// </summary>
        [HttpPost("admin-login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.AdminLoginAsync(dto);
                if (!result.IsSuccess) return Unauthorized(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"An error occurred during admin login: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Logout (client-side token removal)
        /// POST: api/auth/logout
        /// </summary>
        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Get current user profile
        /// GET: api/auth/profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var username = GetCurrentUsername();
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

        /// <summary>
        /// Update user profile
        /// PUT: api/auth/update-profile
        /// </summary>
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var username = GetCurrentUsername();
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

        /// <summary>
        /// Upload profile image
        /// POST: api/auth/upload-profile-image
        /// </summary>
        [HttpPost("upload-profile-image")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { isSuccess = false, message = "No file uploaded" });
            }

            try
            {
                var username = GetCurrentUsername();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { isSuccess = false, message = "User not found" });
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
                return StatusCode(500, new { isSuccess = false, message = "Error uploading profile image", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if admin user exists (Development/Setup only)
        /// GET: api/auth/check-admin
        /// </summary>
        [HttpGet("check-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAdmin()
        {
            var adminUser = await _authService.CheckAdminUserAsync();
            return Ok(adminUser);
        }

        /// <summary>
        /// Create admin user (Development/Setup only)
        /// POST: api/auth/create-admin
        /// </summary>
        [HttpPost("create-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin()
        {
            var result = await _authService.CreateAdminUserAsync();
            return Ok(result);
        }

        /// <summary>
        /// Change password
        /// PUT: api/auth/change-password
        /// </summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var username = GetCurrentUsername();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var result = await _authService.ChangePasswordAsync(username, dto);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error changing password", error = ex.Message });
            }
        }

        /// <summary>
        /// Test authenticated endpoint
        /// GET: api/auth/test-auth
        /// </summary>
        [HttpGet("test-auth")]
        [Authorize(Roles = "Parent")]
        public IActionResult TestAuth()
        {
            var username = GetCurrentUsername();
            return Ok($"Hello, {username}! You have successfully accessed a secure endpoint.");
        }
    }
}

