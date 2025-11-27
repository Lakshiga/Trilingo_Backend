namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Age { get; set; }
        public string? NativeLanguage { get; set; }
        public string? LearningLanguage { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
