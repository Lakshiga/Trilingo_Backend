using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Auth;
using TES_Learning_App.Application_Layer.DTOs.Auth.Requests;
using TES_Learning_App.Application_Layer.DTOs.Auth.Response;
using Microsoft.AspNetCore.Http;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<object> CheckAdminUserAsync();
        Task<object> CreateAdminUserAsync();
        Task<AuthResponseDto> UploadProfileImageAsync(string username, IFormFile file);
        Task<AuthResponseDto> GetUserProfileAsync(string username);
        Task<AuthResponseDto> UpdateProfileAsync(string username, UpdateProfileDto dto);
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<AuthResponseDto> ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto dto);
        Task<AuthResponseDto> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDto dto);
        Task<AuthResponseDto> ChangePasswordAsync(string username, ChangePasswordDto dto);
    }
}
