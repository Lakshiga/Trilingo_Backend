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
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            // Id, Avatar, and CreatedAt are inherited from BaseProfile.

            // --- CONFIGURE INHERITED PROPERTIES ---
            builder.Property(a => a.ProfileImageUrl)
                   .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
                   .IsRequired();

            builder.Property(a => a.FullName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(a => a.JobTitle)
                   .HasMaxLength(100);

            // The one-to-one relationship is already defined
            // on the UserConfiguration side (HasOne -> WithOne).
        }
    }
}
