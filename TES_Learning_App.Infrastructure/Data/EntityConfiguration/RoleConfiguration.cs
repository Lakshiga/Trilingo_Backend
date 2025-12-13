using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data.EntityConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Define Primary Key
            builder.HasKey(r => r.Id);
            // Define constraints for properties
            builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
            builder.HasIndex(r => r.RoleName).IsUnique(); // Ensure role names are unique
            //// Define the one-to-many relationship with User
            //builder.HasMany(r => r.Users)
            //       .WithOne(u => u.Role)
            //       .HasForeignKey(u => u.RoleId)
            //       .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a Role if Users are assigned to it.
        }
    }
}
