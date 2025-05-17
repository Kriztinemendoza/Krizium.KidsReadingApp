using System;

namespace Krizium.KidsReadingApp.Api.Data.Entities
{
    /// <summary>
    /// Represents a user's reading progress for a book
    /// </summary>
    public class ReadingProgress
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
        /// Total time spent reading this book in seconds
        /// </summary>
        public int TotalTimeSpentSeconds { get; set; }
        
        /// <summary>
        /// Navigation property for the book this progress is for
        /// </summary>
        public virtual Book Book { get; set; } = null!;
    }
}
