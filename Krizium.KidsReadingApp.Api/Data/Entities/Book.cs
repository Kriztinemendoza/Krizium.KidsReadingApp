using System;
using System.Collections.Generic;

namespace Krizium.KidsReadingApp.Api.Data.Entities
{
    /// <summary>
    /// Represents a book in the database
    /// </summary>
    public class Book
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
        /// Brief description of the book
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Date the book was added to the library
        /// </summary>
        public DateTime DateAdded { get; set; }
        
        /// <summary>
        /// Categories/tags for the book (comma-separated)
        /// </summary>
        public string? Categories { get; set; }
        
        /// <summary>
        /// Publishing year of the book
        /// </summary>
        public int? PublishYear { get; set; }
        
        /// <summary>
        /// Publisher of the book
        /// </summary>
        public string? Publisher { get; set; }
        
        /// <summary>
        /// ISBN of the book, if applicable
        /// </summary>
        public string? ISBN { get; set; }
        
        /// <summary>
        /// Flag indicating if the book is active/available
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Navigation property for pages in this book
        /// </summary>
        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
        
        /// <summary>
        /// Navigation property for reading progress records for this book
        /// </summary>
        public virtual ICollection<ReadingProgress> ReadingProgress { get; set; } = new List<ReadingProgress>();
    }
}
