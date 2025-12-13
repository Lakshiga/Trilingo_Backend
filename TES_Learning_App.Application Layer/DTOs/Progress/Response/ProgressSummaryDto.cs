using System;
using System.Collections.Generic;

namespace TES_Learning_App.Application_Layer.DTOs.Progress.Response
{
    public class ProgressSummaryDto
    {
        public Guid StudentId { get; set; }
        public string StudentNickname { get; set; } = string.Empty;
        public string StudentAvatar { get; set; } = string.Empty;
        public int TotalActivitiesCompleted { get; set; }
        public int TotalActivitiesAttempted { get; set; }
        public double AverageScore { get; set; }
        public int TotalXpPoints { get; set; }
        public int TotalTimeSpentSeconds { get; set; }
        public string TotalTimeSpentFormatted { get; set; } = string.Empty;
        public DateTime? LastActivityDate { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}


