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
    public class StageConfiguration : IEntityTypeConfiguration<Stage>
    {
        public void Configure(EntityTypeBuilder<Stage> builder)
        {
            builder.HasKey(s => s.Id);

            // Configure the inherited translation properties
            builder.Property(s => s.Name_en).IsRequired().HasMaxLength(150);
            builder.Property(s => s.Name_ta).IsRequired().HasMaxLength(150);
            builder.Property(s => s.Name_si).IsRequired().HasMaxLength(150);

            // Configure the relationship to Level
            builder.HasOne(s => s.Level)
                   .WithMany(l => l.Stages)
                   .HasForeignKey(s => s.LevelId)
                   .OnDelete(DeleteBehavior.Cascade); // If a Level is deleted, its Stages are deleted too
        }
    }
}
