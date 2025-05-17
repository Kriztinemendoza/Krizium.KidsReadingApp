using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Core.Interfaces
{
    public interface IBookApiService
    {
        /// <summary>
        /// Gets all books from the API
        /// </summary>
        Task<List<Book>> GetAllBooksAsync();

        /// <summary>
        /// Gets a specific book by ID
        /// </summary>
        Task<Book> GetBookByIdAsync(int id);

        /// <summary>
        /// Gets a specific page of a book
        /// </summary>
        Task<Page> GetBookPageAsync(int bookId, int pageNumber);

        /// <summary>
        /// Updates reading progress
        /// </summary>
        Task<bool> UpdateReadingProgressAsync(int userId, int bookId, int pageNumber);

        /// <summary>
        /// Gets reading progress for a book
        /// </summary>
        Task<ReadingProgress> GetReadingProgressAsync(int userId, int bookId);

        /// <summary>
        /// Gets recently read books
        /// </summary>
        Task<List<BookProgress>> GetRecentBooksAsync(int userId, int count = 5);
    }
}
