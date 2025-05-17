using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krizium.KidsReadingApp.Data.Configuration
{
    public class WordConfiguration : IEntityTypeConfiguration<WordEntity>
    {
        public void Configure(EntityTypeBuilder<WordEntity> builder)
        {
            builder.ToTable("Words");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.ParagraphId)
                .IsRequired();

            builder.Property(w => w.Text)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Order)
                .IsRequired();

            builder.Property(w => w.AudioCacheKey)
                .HasMaxLength(200);

            builder.Property(w => w.IsAvailableOffline)
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(w => new { w.ParagraphId, w.Order });
        }
    }
}