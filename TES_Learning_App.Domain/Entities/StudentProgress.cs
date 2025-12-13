using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Domain.Entities
{
    public class StudentProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public int MaxScore { get; set; } // Maximum possible score for the activity

        [Required]
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Time tracking for performance analysis
        public int TimeSpentSeconds { get; set; } = 0; // Time taken to complete activity

        // Attempt tracking
        public int AttemptNumber { get; set; } = 1; // Which attempt (1st, 2nd, etc.)

        // Additional metadata
        public bool IsCompleted { get; set; } = true; // Whether activity was fully completed
        public string? Notes { get; set; } // Optional notes/feedback

        // Foreign Key to Student
        // Progress is tracked for a specific Student (child) profile
        public Guid? StudentId { get; set; }
        public Student? Student { get; set; } = null!;

        // Foreign Key to Activity
        public int ActivityId { get; set; }
        // Navigation Property
        public Activity Activity { get; set; } = null!;
    }
}
