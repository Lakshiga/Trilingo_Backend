using System;
using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Domain.Entities
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to Activity
        public int ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;

        // JSON content for this exercise
        public string JsonData { get; set; } = string.Empty;

        // Sequence order for exercises within an activity
        public int SequenceOrder { get; set; } = 1;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
