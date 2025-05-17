using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krizium.KidsReadingApp.Data.Configuration
{
    public class ParagraphConfiguration : IEntityTypeConfiguration<ParagraphEntity>
    {
        public void Configure(EntityTypeBuilder<ParagraphEntity> builder)
        {
            builder.ToTable("Paragraphs");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PageId)
                .IsRequired();

            builder.Property(p => p.Order)
                .IsRequired();

            // Relationships
            builder.HasMany(p => p.Words)
                .WithOne(w => w.Paragraph)
                .HasForeignKey(w => w.ParagraphId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => new { p.PageId, p.Order });
        }
    }
}