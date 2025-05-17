using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Api.Data.Entities
{
    /// <summary>
    /// Represents a page in a book
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Unique identifier for the page
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the book this page belongs to
        /// </summary>
        public int BookId { get; set; }
        
        /// <summary>
        /// Page number in the book
        /// </summary>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// URL to the page's image, if any
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Background color of the page (hex code)
        /// </summary>
        public string? BackgroundColor { get; set; }
        
        /// <summary>
        /// Text color of the page (hex code)
        /// </summary>
        public string? TextColor { get; set; }
        
        /// <summary>
        /// Navigation property for the book this page belongs to
        /// </summary>
        public virtual Book Book { get; set; } = null!;
        
        /// <summary>
        /// Navigation property for paragraphs on this page
        /// </summary>
        public virtual ICollection<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();
    }
}
