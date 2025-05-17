using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Data.Entities
{
    public class ParagraphEntity
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int Order { get; set; }

        // Navigation properties
        public PageEntity Page { get; set; }
        public ICollection<WordEntity> Words { get; set; } = new List<WordEntity>();
    }
}