using Krizium.KidsReadingApp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Krizium.KidsReadingApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<Paragraph> Paragraphs { get; set; } = null!;
        public DbSet<Word> Words { get; set; } = null!;
        public DbSet<ReadingProgress> ReadingProgress { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure Books
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
                entity.Property(e => e.Categories).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Publisher).HasMaxLength(100);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.DateAdded).HasDefaultValueSql("GETDATE()");
            });

            // Configure Pages
            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.BackgroundColor).HasMaxLength(10);
                entity.Property(e => e.TextColor).HasMaxLength(10);
                
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.Pages)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.BookId, e.PageNumber }).IsUnique();
            });

            // Configure Paragraphs
            modelBuilder.Entity<Paragraph>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FontSize).HasMaxLength(20);
                entity.Property(e => e.Alignment).HasMaxLength(10);
                
                entity.HasOne(e => e.Page)
                    .WithMany(p => p.Paragraphs)
                    .HasForeignKey(e => e.PageId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.PageId, e.Order });
            });

            // Configure Words
            modelBuilder.Entity<Word>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AudioUrl).HasMaxLength(500);
                entity.Property(e => e.AudioCacheKey).HasMaxLength(100);
                entity.Property(e => e.Definition).HasMaxLength(1000);
                entity.Property(e => e.Phonetics).HasMaxLength(100);
                
                entity.HasOne(e => e.Paragraph)
                    .WithMany(p => p.Words)
                    .HasForeignKey(e => e.ParagraphId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.ParagraphId, e.Order });
            });

            // Configure ReadingProgress
            modelBuilder.Entity<ReadingProgress>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.ReadingProgress)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.UserId, e.BookId }).IsUnique();
            });
        }
    }
}
