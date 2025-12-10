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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<AuthResponseDto>(result.Message);
                return Unauthorized(errorResponse);
            }
            return Ok(result);
        }

        // Example of a secure endpoint
        [HttpGet("test-auth")]
        [Authorize(Roles = "Parent")] // Only a logged-in user with the "Parent" role can access this
        public IActionResult TestAuth()
        {
            // We can get the logged-in user's info from the token claims
            var username = GetUsername();
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
                var errorResponse = ErrorResponseService.CreateErrorResponse<AuthResponseDto>("No file uploaded", 400);
                return BadRequest(errorResponse);
            }

            var username = GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<AuthResponseDto>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _authService.UploadProfileImageAsync(username, file);
            return HandleServiceResult(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<AuthResponseDto>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _authService.GetUserProfileAsync(username);
            return HandleServiceResult(result);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var username = GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<AuthResponseDto>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _authService.UpdateProfileAsync(username, dto);
            return HandleServiceResult(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpPost("reset-password-otp")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordWithOtpDto dto)
        {
            var result = await _authService.ResetPasswordWithOtpAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpPost("reset-password-token")]
        public async Task<IActionResult> ResetPasswordWithToken([FromBody] ResetPasswordWithTokenDto dto)
        {
            var result = await _authService.ResetPasswordWithTokenAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var username = GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<AuthResponseDto>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _authService.ChangePasswordAsync(username, dto);
            
            // If password changed successfully and was first login, return special response
            if (result.IsSuccess && result.MustChangePassword)
            {
                return Ok(new { 
                    isSuccess = true, 
                    message = "Password changed successfully. Please login again.",
                    mustLoginAgain = true 
                });
            }
            
            return HandleServiceResult(result);
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