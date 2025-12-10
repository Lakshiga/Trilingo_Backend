namespace TES_Learning_App.Application_Layer.DTOs.Progress.Response
{
    public class ProgressDto
    {
        public int Id { get; set; }
        public Guid? StudentId { get; set; }
        public string? StudentNickname { get; set; }
        public string? StudentAvatar { get; set; }
        public int ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public string? ActivityNameEn { get; set; }
        public string? ActivityNameTa { get; set; }
        public string? ActivityNameSi { get; set; }
        public int? StageId { get; set; }
        public string? StageName { get; set; }
        public int? LevelId { get; set; }
        public string? LevelName { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double PercentageScore { get; set; }
        public DateTime CompletedAt { get; set; }
        public int TimeSpentSeconds { get; set; }
        public string TimeSpentFormatted { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
    }
}


