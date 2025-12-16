using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Student.Request
{
    /// <summary>
    /// DTO for submitting exercise attempt results
    /// </summary>
    public class SubmitExerciseAttemptDto
    {
        /// <summary>
        /// ID of the exercise being attempted
        /// </summary>
        [Required]
        public int ExerciseId { get; set; }

        /// <summary>
        /// Score achieved in this attempt (0-10)
        /// </summary>
        [Required]
        [Range(0, 10)]
        public int Score { get; set; }

        /// <summary>
        /// Time spent on the exercise in seconds
        /// </summary>
        public int TimeSpentSeconds { get; set; }

        /// <summary>
        /// Number of attempts made (used to determine if this is the first attempt)
        /// </summary>
        [Required]
        public int AttemptNumber { get; set; }

        /// <summary>
        /// Additional details about the attempt
        /// </summary>
        public string? AttemptDetails { get; set; }
    }
}