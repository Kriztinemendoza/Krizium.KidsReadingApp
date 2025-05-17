using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Krizium.KidsReadingApp.Data.Configuration;

namespace Krizium.KidsReadingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BookEntity> Books { get; set; }
        public DbSet<PageEntity> Pages { get; set; }
        public DbSet<ParagraphEntity> Paragraphs { get; set; }
        public DbSet<WordEntity> Words { get; set; }
        public DbSet<ReadingProgressEntity> ReadingProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new PageConfiguration());
            modelBuilder.ApplyConfiguration(new ParagraphConfiguration());
            modelBuilder.ApplyConfiguration(new ReadingProgressConfiguration());
            modelBuilder.ApplyConfiguration(new WordConfiguration());

            // Apply configurations from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}