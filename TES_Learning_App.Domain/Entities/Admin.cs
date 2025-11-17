using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Common;

namespace TES_Learning_App.Domain.Entities
{
    public class Admin : BaseProfile
    {
        // 'Id', 'UserId', and 'User' are all inherited automatically.

        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; } // e.g., "Content Manager"

        // --- THE UNIQUE ONE-TO-ONE RELATIONSHIP LOGIC LIVES HERE ---
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
