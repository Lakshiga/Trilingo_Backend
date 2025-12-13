using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data.EntityConfiguration
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.JsonData)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.SequenceOrder)
                .HasDefaultValue(1);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Foreign Key relationship to Activity
            builder.HasOne(e => e.Activity)
                .WithMany()
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
