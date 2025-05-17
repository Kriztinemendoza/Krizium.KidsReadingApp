using Krizium.KidsReadingApp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(AppDbContext context, ILogger<BookRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Book>> GetAllBooksAsync(int? ageMin = null, int? ageMax = null, string? category = null, int skip = 0, int take = 20)
        {
            try
            {
                var query = _context.Books
                    .Where(b => b.IsActive)
                    .AsQueryable();

                // Apply filters
                if (ageMin.HasValue)
                {
                    query = query.Where(b => b.AgeRangeMin <= ageMin.Value && b.AgeRangeMax >= ageMin.Value);
                }

                if (ageMax.HasValue)
                {
                    query = query.Where(b => b.AgeRangeMin <= ageMax.Value && b.AgeRangeMax >= ageMax.Value);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(b => b.Categories != null && b.Categories.Contains(category));
                }

                // Apply paging
                return await query
                    .OrderBy(b => b.Title)
                    .Skip(skip)
                    .Take(take)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                throw;
            }
        }

        public async Task<int> GetBookCountAsync(int? ageMin = null, int? ageMax = null, string? category = null)
        {
            try
            {
                var query = _context.Books
                    .Where(b => b.IsActive)
                    .AsQueryable();

                // Apply filters
                if (ageMin.HasValue)
                {
                    query = query.Where(b => b.AgeRangeMin <= ageMin.Value && b.AgeRangeMax >= ageMin.Value);
                }

                if (ageMax.HasValue)
                {
                    query = query.Where(b => b.AgeRangeMin <= ageMax.Value && b.AgeRangeMax >= ageMax.Value);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(b => b.Categories != null && b.Categories.Contains(category));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting book count");
                throw;
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            try
            {
                return await _context.Books
                    .Where(b => b.Id == id && b.IsActive)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting book with ID {id}");
                throw;
            }
        }

        public async Task<Page?> GetBookPageAsync(int bookId, int pageNumber)
        {
            try
            {
                return await _context.Pages
                    .Include(p => p.Paragraphs)
                    .ThenInclude(p => p.Words)
                    .Where(p => p.BookId == bookId && p.PageNumber == pageNumber)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting page {pageNumber} for book {bookId}");
                throw;
            }
        }

        public async Task<List<Page>> GetBookPagesAsync(int bookId)
        {
            try
            {
                return await _context.Pages
                    .Where(p => p.BookId == bookId)
                    .OrderBy(p => p.PageNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting pages for book {bookId}");
                throw;
            }
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            try
            {
                book.DateAdded = DateTime.UtcNow;
                book.IsActive = true;

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book");
                throw;
            }
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                var existingBook = await _context.Books.FindAsync(book.Id);
                if (existingBook == null)
                {
                    return false;
                }

                // Update properties but preserve some
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.CoverImageUrl = book.CoverImageUrl;
                existingBook.AgeRangeMin = book.AgeRangeMin;
                existingBook.AgeRangeMax = book.AgeRangeMax;
                existingBook.Description = book.Description;
                existingBook.Categories = book.Categories;
                existingBook.PublishYear = book.PublishYear;
                existingBook.Publisher = book.Publisher;
                existingBook.ISBN = book.ISBN;
                existingBook.IsActive = book.IsActive;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {book.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return false;
                }

                // Soft delete
                book.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID {id}");
                throw;
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                var categories = new HashSet<string>();

                var categoryStrings = await _context.Books
                    .Where(b => b.IsActive && !string.IsNullOrEmpty(b.Categories))
                    .Select(b => b.Categories)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var categoryString in categoryStrings)
                {
                    if (categoryString != null)
                    {
                        var categoriesArray = categoryString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var category in categoriesArray)
                        {
                            categories.Add(category.Trim());
                        }
                    }
                }

                return categories.OrderBy(c => c).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                throw;
            }
        }

        public async Task<int> GetBookPageCountAsync(int bookId)
        {
            try
            {
                return await _context.Pages
                    .Where(p => p.BookId == bookId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting page count for book {bookId}");
                throw;
            }
        }
    }
}
