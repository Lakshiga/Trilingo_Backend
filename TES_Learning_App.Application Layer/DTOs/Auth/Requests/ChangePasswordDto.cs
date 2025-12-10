using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}


