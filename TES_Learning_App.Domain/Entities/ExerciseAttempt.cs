using System;
using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Domain.Entities
{
    /// <summary>
    /// Represents a student's attempt at an exercise
    /// </summary>
    public class ExerciseAttempt
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Student
        /// </summary>
        [Required]
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        /// <summary>
        /// Foreign key to Exercise
        /// </summary>
        [Required]
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        /// <summary>
        /// The score achieved in this attempt (0-10)
        /// </summary>
        [Required]
        [Range(0, 10)]
        public int Score { get; set; }

        /// <summary>
        /// Indicates if this is the first attempt (which counts toward the final score)
        /// </summary>
        [Required]
        public bool IsFirstAttempt { get; set; }

        /// <summary>
        /// Timestamp when the attempt was completed
        /// </summary>
        [Required]
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional details about the attempt (e.g., time spent, corrections made)
        /// </summary>
        public string AttemptDetails { get; set; }
    }
}