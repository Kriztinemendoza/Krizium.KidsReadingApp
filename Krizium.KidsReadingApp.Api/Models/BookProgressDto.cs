namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Combines book information with reading progress
    /// </summary>
    public class BookProgressDto
    {
        /// <summary>
        /// Book information
        /// </summary>
        public BookDto Book { get; set; } = new BookDto();
        
        /// <summary>
        /// Reading progress information
        /// </summary>
        public ReadingProgressDto Progress { get; set; } = new ReadingProgressDto();
    }
}
