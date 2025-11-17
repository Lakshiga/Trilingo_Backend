using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Domain.Entities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LanguageName { get; set; } = string.Empty;

        // Navigation Property: A Language has many Levels
        public ICollection<Level> Levels { get; set; } = new List<Level>();
    }
}
 