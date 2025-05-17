using Krizium.KidsReadingApp.Api.Models;
using Krizium.KidsReadingApp.Api.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Services.Interfaces
{
    public interface IReadingProgressService
    {
        /// <summary>
        /// Gets reading progress for a specific book and user
        /// </summary>
        Task<ReadingProgressDto?> GetReadingProgressAsync(int userId, int bookId);
        
        /// <summary>
        /// Gets all reading progress for a user
        /// </summary>
        Task<List<ReadingProgressDto>> GetUserReadingProgressAsync(int userId);
        
        /// <summary>
        /// Gets the most recently read books for a user
        /// </summary>
        Task<List<BookProgressDto>> GetRecentBooksAsync(int userId, int count = 5);
        
        /// <summary>
        /// Updates reading progress
        /// </summary>
        Task<ReadingProgressDto> UpdateReadingProgressAsync(ReadingProgressRequest request);
        
        /// <summary>
        /// Resets reading progress for a book
        /// </summary>
        Task<bool> ResetReadingProgressAsync(int userId, int bookId);
        
        /// <summary>
        /// Deletes reading progress
        /// </summary>
        Task<bool> DeleteReadingProgressAsync(int userId, int bookId);
    }
}
