namespace Krizium.KidsReadingApp.Data.Entities
{
    public class WordEntity
    {
        public int Id { get; set; }
        public int ParagraphId { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
        public string? AudioCacheKey { get; set; }
        public bool IsAvailableOffline { get; set; }

        // Navigation property
        public ParagraphEntity Paragraph { get; set; }
    }
}