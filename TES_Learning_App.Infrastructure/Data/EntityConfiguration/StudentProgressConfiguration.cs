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
            builder.Property(sp => sp.MaxScore).IsRequired();
            builder.Property(sp => sp.CompletedAt).IsRequired();
            builder.Property(sp => sp.TimeSpentSeconds).IsRequired().HasDefaultValue(0);
            builder.Property(sp => sp.AttemptNumber).IsRequired().HasDefaultValue(1);
            builder.Property(sp => sp.IsCompleted).IsRequired().HasDefaultValue(true);
            builder.Property(sp => sp.Notes).HasMaxLength(500);

            // --- PERFORMANCE INDEXES ---
            // Composite index for common queries: Get progress by student and activity
            builder.HasIndex(sp => new { sp.StudentId, sp.ActivityId })
                   .HasDatabaseName("IX_StudentProgress_StudentId_ActivityId");

            // Index for date-based queries (Admin dashboard analytics)
            builder.HasIndex(sp => sp.CompletedAt)
                   .HasDatabaseName("IX_StudentProgress_CompletedAt");

            // Index for student-based queries (Mobile app)
            builder.HasIndex(sp => sp.StudentId)
                   .HasDatabaseName("IX_StudentProgress_StudentId");

            // Index for activity-based queries
            builder.HasIndex(sp => sp.ActivityId)
                   .HasDatabaseName("IX_StudentProgress_ActivityId");

            // --- RELATIONSHIP CONFIGURATIONS ---

            builder.HasOne(sp => sp.Student)
                   .WithMany() // No corresponding collection on Student
                   .HasForeignKey(sp => sp.StudentId)
                   // When a Student is deleted, set the StudentId in this table to NULL.
                   .OnDelete(DeleteBehavior.SetNull); // Preserves progress history

            // Relationship to Activity
            builder.HasOne(sp => sp.Activity)
                   .WithMany() // No corresponding collection on Activity
                   .HasForeignKey(sp => sp.ActivityId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
