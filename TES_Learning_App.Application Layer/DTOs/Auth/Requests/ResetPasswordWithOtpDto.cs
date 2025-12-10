using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class ResetPasswordWithOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

