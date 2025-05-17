using System.ComponentModel.DataAnnotations;

namespace Krizium.KidsReadingApp.Api.Models.Requests
{
    /// <summary>
    /// Request model for updating reading progress
    /// </summary>
    public class ReadingProgressRequest
    {
        /// <summary>
        /// ID of the user
        /// </summary>
        [Required]
        public int UserId { get; set; }
        
        /// <summary>
        /// ID of the book
        /// </summary>
        [Required]
        public int BookId { get; set; }
        
        /// <summary>
        /// Page number that was read
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Optional time spent on this page in seconds
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a positive number")]
        public int? TimeSpentSeconds { get; set; }
        
        /// <summary>
        /// Optional flag indicating if the book was completed
        /// </summary>
        public bool IsBookCompleted { get; set; }
    }
}
