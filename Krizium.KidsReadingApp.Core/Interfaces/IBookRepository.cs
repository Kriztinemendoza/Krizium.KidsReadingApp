using Krizium.KidsReadingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Core.Interfaces
{
    public interface IBookRepository
    {
        /// <summary>
        /// Gets all books in the library
        /// </summary>
        Task<List<Book>> GetAllBooksAsync();

        /// <summary>
        /// Gets a book by its ID
        /// </summary>
        Task<Book> GetBookByIdAsync(int id);

        /// <summary>
        /// Gets a specific page of a book
        /// </summary>
        Task<Page> GetBookPageAsync(int bookId, int pageNumber);

        /// <summary>
        /// Updates the reading progress for a user and book
        /// </summary>
        Task<bool> UpdateReadingProgressAsync(int userId, int bookId, int pageNumber);

        /// <summary>
        /// Marks a book for offline access
        /// </summary>
        Task<bool> MarkBookForOfflineAccessAsync(int bookId, bool available);

        /// <summary>
        /// Gets the reading progress for a specific book
        /// </summary>
        Task<ReadingProgress> GetReadingProgressAsync(int userId, int bookId);
    }
}