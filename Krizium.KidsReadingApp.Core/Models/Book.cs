namespace Krizium.KidsReadingApp.Core.Models
{

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public int AgeRangeMin { get; set; }
        public int AgeRangeMax { get; set; }
        public List<Page> Pages { get; set; } = new List<Page>();
    }

    public class Page
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int PageNumber { get; set; }
        public List<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();
        public string ImageUrl { get; set; }
    }

    public class Paragraph
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
    }

    public class Word
    {
        public int Id { get; set; }
        public int ParagraphId { get; set; }
        public string Text { get; set; }
        public string AudioCacheKey { get; set; }
        public bool IsAvailableOffline { get; set; }
    }

    public class ReadingProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int LastPageRead { get; set; }
        public DateTime LastReadTime { get; set; }
        public int TimesCompleted { get; set; }
    }

    public class BookProgress
    {
        public Book Book { get; set; }
        public ReadingProgress Progress { get; set; }
    }
}