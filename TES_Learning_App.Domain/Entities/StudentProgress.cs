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
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        // Progress is tracked for a specific Student (child) profile
        public Guid? StudentId { get; set; }
        public Student? Student { get; set; } = null!;

        // Foreign Key to Activity
        public int ActivityId { get; set; }
        // Navigation Property
        public Activity Activity { get; set; } = null!;
    }
}
