namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Represents a paragraph of text on a page
    /// </summary>
    public class ParagraphDto
    {
        /// <summary>
        /// Unique identifier for the paragraph
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the page this paragraph belongs to
        /// </summary>
        public int PageId { get; set; }
        
        /// <summary>
        /// Order/position of this paragraph on the page
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Words that make up this paragraph
        /// </summary>
        public List<WordDto> Words { get; set; } = new List<WordDto>();
    }
}
