using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Data.Entities
{
    public class BookEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public int AgeRangeMin { get; set; }
        public int AgeRangeMax { get; set; }
        public bool IsAvailableOffline { get; set; }

        // Navigation properties
        public ICollection<PageEntity> Pages { get; set; } = new List<PageEntity>();
        public ICollection<ReadingProgressEntity> ReadingProgress { get; set; } = new List<ReadingProgressEntity>();
    }
}