using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Admin.Requests
{
    public class UpdateAdminDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Full name cannot exceed 255 characters.")]
        public string? FullName { get; set; }

        [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
        public string? JobTitle { get; set; }

        public Guid? RoleId { get; set; }

        public bool? MustChangePassword { get; set; }
    }
}


