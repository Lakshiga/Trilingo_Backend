using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Domain.Entities
{
    public class StudentProgress
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The final score for this exercise (from first attempt)
        /// </summary>
        [Required]
        [Range(0, 10)]
        public int Score { get; set; }

        [Required]
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        // Progress is tracked for a specific Student (child) profile
        public Guid? StudentId { get; set; }
        public Student? Student { get; set; } = null!;

        // Foreign Key to Activity
        public int ActivityId { get; set; }
        // Navigation Property
        public Activity Activity { get; set; } = null!;
        
        // Foreign Key to Exercise (optional, for more granular tracking)
        public int? ExerciseId { get; set; }
        // Navigation Property
        public Exercise? Exercise { get; set; }
    }
}