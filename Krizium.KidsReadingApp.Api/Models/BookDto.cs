namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Represents a book in the reading app
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// Unique identifier for the book
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Author of the book
        /// </summary>
        public string Author { get; set; } = string.Empty;
        
        /// <summary>
        /// URL to the book's cover image
        /// </summary>
        public string? CoverImageUrl { get; set; }
        
        /// <summary>
        /// Minimum recommended age for readers
        /// </summary>
        public int AgeRangeMin { get; set; }
        
        /// <summary>
        /// Maximum recommended age for readers
        /// </summary>
        public int AgeRangeMax { get; set; }
        
        /// <summary>
        /// Total number of pages in the book
        /// </summary>
        public int PageCount { get; set; }
        
        /// <summary>
        /// Brief description of the book
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Date the book was added to the library
        /// </summary>
        public DateTime DateAdded { get; set; }
        
        /// <summary>
        /// List of categories/tags for the book
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();
        
        /// <summary>
        /// List of page summaries for the book (lightweight version of pages)
        /// </summary>
        public List<PageSummaryDto>? Pages { get; set; }
    }
}
