using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krizium.KidsReadingApp.Data.Configuration
{
    public class ReadingProgressConfiguration : IEntityTypeConfiguration<ReadingProgressEntity>
    {
        public void Configure(EntityTypeBuilder<ReadingProgressEntity> builder)
        {
            builder.ToTable("ReadingProgress");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.BookId)
                .IsRequired();

            builder.Property(r => r.LastPageRead)
                .IsRequired();

            builder.Property(r => r.LastReadTime)
                .IsRequired();

            builder.Property(r => r.TimesCompleted)
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();
        }
    }
}