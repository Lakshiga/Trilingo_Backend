using System;

namespace TES_Learning_App.Application_Layer.DTOs.Progress.Requests
{
    public class ProgressFilterDto
    {
        public Guid? StudentId { get; set; }
        public int? ActivityId { get; set; }
        public int? StageId { get; set; }
        public int? LevelId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public bool? IsCompleted { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "CompletedAt";
        public bool SortDescending { get; set; } = true;
    }
}


