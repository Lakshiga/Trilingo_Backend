using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class MainActivity : BaseTranslation
    {
        [Key]
        public int Id { get; set; } // Primary Key

        //[Required]
        //[StringLength(100)]
        //public string Name { get; set; } = string.Empty; // e.g., "Speaking"

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
