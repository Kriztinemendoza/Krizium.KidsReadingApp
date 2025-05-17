using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Api.Data.Entities
{
    /// <summary>
    /// Represents a paragraph of text on a page
    /// </summary>
    public class Paragraph
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
        /// Font size for this paragraph (if different from page default)
        /// </summary>
        public string? FontSize { get; set; }
        
        /// <summary>
        /// Alignment of this paragraph (left, center, right, justify)
        /// </summary>
        public string? Alignment { get; set; }
        
        /// <summary>
        /// Navigation property for the page this paragraph belongs to
        /// </summary>
        public virtual Page Page { get; set; } = null!;
        
        /// <summary>
        /// Navigation property for words in this paragraph
        /// </summary>
        public virtual ICollection<Word> Words { get; set; } = new List<Word>();
    }
}