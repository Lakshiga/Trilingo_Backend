using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class Stage : BaseTranslation
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //[StringLength(150)]
        //public string StageName { get; set; } = string.Empty;
        // 'Name_en', 'Name_ta', 'Name_si' are automatically included.

        // Foreign Key to Level
        public int LevelId { get; set; }
        // Navigation Property: A Stage belongs to one Level
        public Level Level { get; set; } = null!;

        // Navigation Property: A Stage has many Activities
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}