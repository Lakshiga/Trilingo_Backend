using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Admin.Requests
{
    public class CreateAdminDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(255, ErrorMessage = "Full name cannot exceed 255 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Role ID is required.")]
        public Guid RoleId { get; set; }

        public bool MustChangePassword { get; set; } = true;
    }
}


