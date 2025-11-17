using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class ActivityType : BaseTranslation
    {
        [Key]
        public int Id { get; set; } // Primary Key

        // 'Name_en', 'Name_ta', 'Name_si' are automatically included.

        //[Required]
        //[StringLength(100)]
        //public string TypeName { get; set; } = string.Empty; // e.g., "Flashcards"

        [StringLength(255)]
        public string? Description { get; set; }

        public string? JsonMethod { get; set; } // Stores the JSON template for this activity type

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
