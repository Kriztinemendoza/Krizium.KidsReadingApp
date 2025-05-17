using System;

namespace Krizium.KidsReadingApp.Data.Entities
{
    public class ReadingProgressEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int LastPageRead { get; set; }
        public DateTime LastReadTime { get; set; }
        public int TimesCompleted { get; set; }

        // Navigation property
        public BookEntity Book { get; set; }
    }
}