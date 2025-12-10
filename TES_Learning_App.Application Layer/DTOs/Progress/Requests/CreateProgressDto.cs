using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Progress.Requests
{
    public class CreateProgressDto
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Activity ID is required.")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Score is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive number.")]
        public int Score { get; set; }

        [Required(ErrorMessage = "Max score is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Max score must be greater than 0.")]
        public int MaxScore { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a positive number.")]
        public int TimeSpentSeconds { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "Attempt number must be at least 1.")]
        public int AttemptNumber { get; set; } = 1;

        public bool IsCompleted { get; set; } = true;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}


