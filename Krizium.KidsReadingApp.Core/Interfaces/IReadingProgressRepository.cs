using Krizium.KidsReadingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Core.Interfaces
{
    public interface IReadingProgressRepository
    {
        /// <summary>
        /// Gets all reading progress records for a user
        /// </summary>
        Task<List<ReadingProgress>> GetUserProgressAsync(int userId);

        /// <summary>
        /// Gets the reading progress for a specific book
        /// </summary>
        Task<ReadingProgress> GetProgressForBookAsync(int userId, int bookId);

        /// <summary>
        /// Saves a reading progress record
        /// </summary>
        Task<bool> SaveProgressAsync(ReadingProgress progress);

        /// <summary>
        /// Resets the reading progress for a book
        /// </summary>
        Task<bool> ResetProgressAsync(int userId, int bookId);

        /// <summary>
        /// Gets the most recently read books for a user
        /// </summary>
        Task<List<BookProgress>> GetRecentBooksAsync(int userId, int count = 5);
    }
}