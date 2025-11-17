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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Define Primary Key
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            // Define constraints for properties
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.HasIndex(u => u.Username).IsUnique(); // Ensure usernames are unique

            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique(); // Ensure emails are unique

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.PasswordSalt).IsRequired();

            // Define the one-to-many relationship with Role
            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a Role if Users are assigned to it.

            // Define the one-to-many relationship with Student (as Parent)
            builder.HasMany(u => u.StudentProfiles)
                   .WithOne(s => s.Parent)
                   .HasForeignKey(s => s.ParentId)
                   .OnDelete(DeleteBehavior.Cascade); // If a Parent User is deleted, their children profiles are also deleted.

            // Define the one-to-one relationship with Admin
            builder.HasOne(u => u.AdminProfile)
                   .WithOne(a => a.User)
                   .HasForeignKey<Admin>(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // If an Admin User is deleted, their admin profile is also deleted.
        }

    }
}

