namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Lightweight representation of a page, used in book listings
    /// </summary>
    public class PageSummaryDto
    {
        /// <summary>
        /// Unique identifier for the page
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Page number in the book
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// URL to the page's image, if any
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}
