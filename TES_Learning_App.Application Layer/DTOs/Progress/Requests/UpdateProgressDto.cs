using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Progress.Requests
{
    public class UpdateProgressDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive number.")]
        public int? Score { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max score must be greater than 0.")]
        public int? MaxScore { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a positive number.")]
        public int? TimeSpentSeconds { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Attempt number must be at least 1.")]
        public int? AttemptNumber { get; set; }

        public bool? IsCompleted { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}


