namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Represents a page in a book with its content
    /// </summary>
    public class PageDto
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
        /// Paragraphs of text on this page
        /// </summary>
        public List<ParagraphDto> Paragraphs { get; set; } = new List<ParagraphDto>();
        
        /// <summary>
        /// Title of the book this page belongs to
        /// </summary>
        public string? BookTitle { get; set; }
        
        /// <summary>
        /// Author of the book this page belongs to
        /// </summary>
        public string? BookAuthor { get; set; }
        
        /// <summary>
        /// Navigation information: is there a previous page?
        /// </summary>
        public bool HasPreviousPage { get; set; }
        
        /// <summary>
        /// Navigation information: is there a next page?
        /// </summary>
        public bool HasNextPage { get; set; }
    }
}
