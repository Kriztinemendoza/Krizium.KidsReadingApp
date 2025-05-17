using Krizium.KidsReadingApp.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Services.Interfaces
{
    public interface IBookService
    {
        /// <summary>
        /// Gets all books with optional filtering and paging
        /// </summary>
        Task<(List<BookDto> Books, int TotalCount)> GetAllBooksAsync(int? ageMin = null, int? ageMax = null, string? category = null, int page = 1, int pageSize = 20);
        
        /// <summary>
        /// Gets a book by ID
        /// </summary>
        Task<BookDto?> GetBookByIdAsync(int id);
        
        /// <summary>
        /// Gets a specific page of a book
        /// </summary>
        Task<PageDto?> GetBookPageAsync(int bookId, int pageNumber);
        
        /// <summary>
        /// Gets all categories
        /// </summary>
        //Task<List<string>> GetCategoriesAsync();
        
        /// <summary>
        /// Adds a new book
        /// </summary>
        //Task<BookDto> AddBookAsync(BookDto bookDto);
        
        /// <summary>
        /// Updates an existing book
        /// </summary>
        //Task<bool> UpdateBookAsync(BookDto bookDto);
        
        /// <summary>
        /// Deletes a book
        /// </summary>
        //Task<bool> DeleteBookAsync(int id);
        
        /// <summary>
        /// Adds a new page to a book
        /// </summary>
        Task<PageDto> AddPageAsync(int bookId, PageDto pageDto);
        
        /// <summary>
        /// Updates an existing page
        /// </summary>
        Task<bool> UpdatePageAsync(int bookId, PageDto pageDto);
        
        /// <summary>
        /// Deletes a page
        /// </summary>
        Task<bool> DeletePageAsync(int bookId, int pageNumber);
    }
}