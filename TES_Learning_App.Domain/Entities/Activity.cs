using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class Activity : BaseTranslation
    {
        [Key]
        public int Id { get; set; }
        //public string ActivityType { get; set; } = string.Empty; // "Game" or "Exam"
        public string? Details_JSON { get; set; } // The flexible JSON data

        // Link to the curriculum structure
        public int StageId { get; set; } // Foreign Key
        public Stage Stage { get; set; } = null!;

        // Relationship to the "skill"
        public int MainActivityId { get; set; } // Foreign Key
        public MainActivity MainActivity { get; set; } = null!;

        // Relationship to the "method"
        public int ActivityTypeId { get; set; } // Foreign Key
        public ActivityType ActivityType { get; set; } = null!;

        // Sequence order for activities within a lesson
        public int SequenceOrder { get; set; } = 1;
    } 
}
