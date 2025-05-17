using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Data.Entities
{
    public class PageEntity
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int PageNumber { get; set; }
        public string ImageUrl { get; set; }

        // Navigation properties
        public BookEntity Book { get; set; }
        public ICollection<ParagraphEntity> Paragraphs { get; set; } = new List<ParagraphEntity>();
    }
}