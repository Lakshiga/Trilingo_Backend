using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Domain.Entities
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation Property: A Role can have many Users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
