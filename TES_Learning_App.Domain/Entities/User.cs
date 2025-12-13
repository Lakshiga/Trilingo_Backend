using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = [];

        [Required]
        public byte[] PasswordSalt { get; set; } = [];

        // Foreign Key to Role
        public Guid RoleId { get; set; }
        // Navigation Property: A User has one Role
        public Role Role { get; set; } = null!;

        // Navigation Property: A User can have one Student profile
        //public Student? Student { get; set; }
        // A User (Parent) can have a collection of Student (Child) profiles.
        public ICollection<Student> StudentProfiles { get; set; } = new List<Student>();

        // Profile image URL
        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        // One-to-one relationship with an Admin profile
        public Admin? AdminProfile { get; set; }
    }
}
