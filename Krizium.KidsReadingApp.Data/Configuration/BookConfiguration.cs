using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krizium.KidsReadingApp.Data.Configuration
{
    public class BookConfiguration : IEntityTypeConfiguration<BookEntity>
    {
        public void Configure(EntityTypeBuilder<BookEntity> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.CoverImageUrl)
                .HasMaxLength(500);

            builder.Property(b => b.AgeRangeMin)
                .IsRequired();

            builder.Property(b => b.AgeRangeMax)
                .IsRequired();

            builder.Property(b => b.IsAvailableOffline)
                .HasDefaultValue(false);

            // Relationships
            builder.HasMany(b => b.Pages)
                .WithOne(p => p.Book)
                .HasForeignKey(p => p.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.ReadingProgress)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}