using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data.EntityConfiguration
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            builder.HasKey(l => l.Id);

            // Configure the inherited translation properties
            builder.Property(l => l.Name_en).IsRequired().HasMaxLength(100);
            builder.Property(l => l.Name_ta).IsRequired().HasMaxLength(100);
            builder.Property(l => l.Name_si).IsRequired().HasMaxLength(100);

            // Configure the relationship to Language
            builder.HasOne(l => l.Language)
                   .WithMany(lang => lang.Levels)
                   .HasForeignKey(l => l.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
