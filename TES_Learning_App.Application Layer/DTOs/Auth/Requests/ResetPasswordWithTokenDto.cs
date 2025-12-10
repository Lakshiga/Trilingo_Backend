using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class ResetPasswordWithTokenDto
    {
        [Required(ErrorMessage = "Reset token is required.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

