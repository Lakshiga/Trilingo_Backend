using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TES_Learning_App.Domain.Entities;


namespace TES_Learning_App.Infrastructure.Data.EntityConfiguration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // The Id, Avatar, and CreatedAt are inherited from BaseProfile.
            // Define Primary Key
            // --- CONFIGURE INHERITED PROPERTIES ---
            builder.Property(s => s.Avatar)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.Nickname)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.DateOfBirth).IsRequired();

            builder.Property(s => s.NativeLanguageCode)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(s => s.TargetLanguageCode)
                   .IsRequired()
                   .HasMaxLength(10);

        }
    }
}
