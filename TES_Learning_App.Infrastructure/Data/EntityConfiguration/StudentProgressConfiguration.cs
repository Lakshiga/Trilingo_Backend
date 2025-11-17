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
    public class StudentProgressConfiguration : IEntityTypeConfiguration<StudentProgress>
    {
        public void Configure(EntityTypeBuilder<StudentProgress> builder)
        {
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Score).IsRequired();
            builder.Property(sp => sp.CompletedAt).IsRequired();

            // --- RELATIONSHIP CONFIGURATIONS ---

            builder.HasOne(sp => sp.Student)
                   .WithMany() // No corresponding collection on Student
                   .HasForeignKey(sp => sp.StudentId)
                   // When a Student is deleted, set the StudentId in this table to NULL.
                   .OnDelete(DeleteBehavior.SetNull); // When a Student is deleted, set the StudentId in this table to NULL.

            // Relationship to Activity
            builder.HasOne(sp => sp.Activity)
                   .WithMany() // No corresponding collection on Activity
                   .HasForeignKey(sp => sp.ActivityId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
