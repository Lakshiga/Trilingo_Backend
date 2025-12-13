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
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.HasKey(a => a.Id);

            // Relationship to Stage
            builder.HasOne(a => a.Stage)
                   .WithMany(s => s.Activities)
                   .HasForeignKey(a => a.StageId)
                   .OnDelete(DeleteBehavior.Cascade); // CORRECT: If a Stage is deleted, its Activities go too.

            // Relationship to MainActivity (the "skill")
            builder.HasOne(a => a.MainActivity)
                   .WithMany(m => m.Activities)
                   .HasForeignKey(a => a.MainActivityId)
                   // using that skill will also be deleted, ensuring no orphans.
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship to ActivityType (the "method")
            builder.HasOne(a => a.ActivityType)
                   .WithMany(at => at.Activities)
                   .HasForeignKey(a => a.ActivityTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
