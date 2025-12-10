namespace TES_Learning_App.Application_Layer.DTOs.Admin.Response
{
    public class AdminListDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool MustChangePassword { get; set; }
    }
}

