namespace TES_Learning_App.Application_Layer.DTOs.Progress.Response
{
    public class StudentStatsDto
    {
        public Guid StudentId { get; set; }
        public string StudentNickname { get; set; } = string.Empty;
        public int TotalCompleted { get; set; }
        public int TotalAttempted { get; set; }
        public double AverageScore { get; set; }
        public int BestScore { get; set; }
        public int WorstScore { get; set; }
        public int TotalXpPoints { get; set; }
        public int TotalTimeSpentSeconds { get; set; }
        public Dictionary<string, int> ActivitiesByStage { get; set; } = new();
        public Dictionary<string, double> AverageScoreByStage { get; set; } = new();
        public List<ActivityProgressDto> RecentProgress { get; set; } = new();
    }

    public class ActivityProgressDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public DateTime CompletedAt { get; set; }
        public int AttemptNumber { get; set; }
    }
}


