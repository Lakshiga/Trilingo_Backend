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
    public class ActivityTypeConfiguration : IEntityTypeConfiguration<ActivityType>
    {
        public void Configure(EntityTypeBuilder<ActivityType> builder)
        {
            // 1. Define the Primary Key
            builder.HasKey(at => at.Id);

            // 2. Configure the INHERITED properties from BaseTranslation
            builder.Property(at => at.Name_en).IsRequired().HasMaxLength(100);
            builder.Property(at => at.Name_ta).IsRequired().HasMaxLength(100);
            builder.Property(at => at.Name_si).IsRequired().HasMaxLength(100);

            // 3. Configure the class-specific 'Description' property
            builder.Property(at => at.Description)
                   .HasMaxLength(255);

            // 4. Configure the JsonMethod property to store JSON templates
            builder.Property(at => at.JsonMethod)
                   .HasColumnType("nvarchar(max)"); // Use max for large JSON content

        }
    }
}
