using Krizium.KidsReadingApp.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Data.Repositories
{
    public interface IBookRepository
    {
        /// <summary>
        /// Gets all books with optional filtering and paging
        /// </summary>
        Task<List<Book>> GetAllBooksAsync(int? ageMin = null, int? ageMax = null, string? category = null, int skip = 0, int take = 20);
        
        /// <summary>
        /// Gets total count of books matching filter criteria
        /// </summary>
        Task<int> GetBookCountAsync(int? ageMin = null, int? ageMax = null, string? category = null);
        
        /// <summary>
        /// Gets a book by ID
        /// </summary>
        Task<Book?> GetBookByIdAsync(int id);
        
        /// <summary>
        /// Gets a specific page of a book
        /// </summary>
        Task<Page?> GetBookPageAsync(int bookId, int pageNumber);
        
        /// <summary>
        /// Gets all pages for a book
        /// </summary>
        Task<List<Page>> GetBookPagesAsync(int bookId);
        
        /// <summary>
        /// Adds a new book
        /// </summary>
        Task<Book> AddBookAsync(Book book);
        
        /// <summary>
        /// Updates an existing book
        /// </summary>
        Task<bool> UpdateBookAsync(Book book);
        
        /// <summary>
        /// Deletes a book
        /// </summary>
        Task<bool> DeleteBookAsync(int id);
        
        /// <summary>
        /// Gets all available categories
        /// </summary>
        Task<List<string>> GetCategoriesAsync();
        
        /// <summary>
        /// Gets total page count for a book
        /// </summary>
        Task<int> GetBookPageCountAsync(int bookId);
    }
}
