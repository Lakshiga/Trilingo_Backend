using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class UpdateProfileDto
    {
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "Age cannot exceed 50 characters")]
        public string? Age { get; set; }

        [StringLength(100, ErrorMessage = "NativeLanguage cannot exceed 100 characters")]
        public string? NativeLanguage { get; set; }

        [StringLength(100, ErrorMessage = "LearningLanguage cannot exceed 100 characters")]
        public string? LearningLanguage { get; set; }

        [StringLength(500, ErrorMessage = "ProfileImageUrl cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? ProfileImageUrl { get; set; }
    }
}
