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
    public class MainActivityConfiguration : IEntityTypeConfiguration<MainActivity>
    {
        public void Configure(EntityTypeBuilder<MainActivity> builder)
        {
            builder.HasKey(m => m.Id);

            // --- CONFIGURE INHERITED PROPERTIES ---
            builder.Property(m => m.Name_en).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Name_ta).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Name_si).IsRequired().HasMaxLength(100);
        }

    }
}
