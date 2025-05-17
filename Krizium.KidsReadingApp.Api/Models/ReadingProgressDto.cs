using System;

namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Represents a user's reading progress for a book
    /// </summary>
    public class ReadingProgressDto
    {
        /// <summary>
        /// Unique identifier for the reading progress record
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the user this progress belongs to
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// ID of the book this progress is for
        /// </summary>
        public int BookId { get; set; }
        
        /// <summary>
        /// Last page number the user has read
        /// </summary>
        public int LastPageRead { get; set; }
        
        /// <summary>
        /// Timestamp of when the user last read this book
        /// </summary>
        public DateTime LastReadTime { get; set; }
        
        /// <summary>
        /// Number of times the user has completed the book
        /// </summary>
        public int TimesCompleted { get; set; }
        
        /// <summary>
        /// Percentage of the book that has been read (0-100)
        /// </summary>
        public int PercentComplete { get; set; }
        
        /// <summary>
        /// Title of the book (included for convenience)
        /// </summary>
        public string? BookTitle { get; set; }
        
        /// <summary>
        /// Cover image URL of the book (included for convenience)
        /// </summary>
        public string? BookCoverUrl { get; set; }
    }
}
