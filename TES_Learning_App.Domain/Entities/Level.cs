using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class Level : BaseTranslation
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //[StringLength(100)]
        //public string LevelName { get; set; } = string.Empty;

        // Foreign Key to Language
        public int LanguageId { get; set; }
        // Navigation Property: A Level belongs to one Language
        public Language Language { get; set; } = null!;

        // Navigation Property: A Level has many Stages
        public ICollection<Stage> Stages { get; set; } = new List<Stage>();
    }
}
