using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Method is required.")]
        public ResetMethod Method { get; set; }
    }

    public enum ResetMethod
    {
        OTP,
        LINK
    }
}

