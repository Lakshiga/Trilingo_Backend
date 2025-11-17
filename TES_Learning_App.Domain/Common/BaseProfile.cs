using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Domain.Common
{
    public abstract class BaseProfile
    {
        [Key]
        public Guid Id { get; set; }

        //[Required]
        //[StringLength(100)]
        //public string Avatar { get; set; } = string.Empty; // The shared Avatar property!

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
