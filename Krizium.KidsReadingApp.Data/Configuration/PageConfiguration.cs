using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krizium.KidsReadingApp.Data.Configuration
{
    public class PageConfiguration : IEntityTypeConfiguration<PageEntity>
    {
        public void Configure(EntityTypeBuilder<PageEntity> builder)
        {
            builder.ToTable("Pages");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.BookId)
                .IsRequired();

            builder.Property(p => p.PageNumber)
                .IsRequired();

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            // Relationships
            builder.HasMany(p => p.Paragraphs)
                .WithOne(p => p.Page)
                .HasForeignKey(p => p.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => new { p.BookId, p.PageNumber })
                .IsUnique();
        }
    }
}