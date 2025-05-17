using Krizium.KidsReadingApp.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Data.Repositories
{
    public interface IReadingProgressRepository
    {
        /// <summary>
        /// Gets reading progress for a specific book and user
        /// </summary>
        Task<ReadingProgress?> GetReadingProgressAsync(int userId, int bookId);
        
        /// <summary>
        /// Gets all reading progress for a user
        /// </summary>
        Task<List<ReadingProgress>> GetUserReadingProgressAsync(int userId);
        
        /// <summary>
        /// Gets the most recently read books for a user
        /// </summary>
        Task<List<ReadingProgress>> GetRecentBooksAsync(int userId, int count = 5);
        
        /// <summary>
        /// Adds or updates reading progress
        /// </summary>
        Task<ReadingProgress> UpsertReadingProgressAsync(ReadingProgress progress);
        
        /// <summary>
        /// Deletes reading progress
        /// </summary>
        Task<bool> DeleteReadingProgressAsync(int userId, int bookId);
    }
}
