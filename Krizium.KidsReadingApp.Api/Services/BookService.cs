using Krizium.KidsReadingApp.Api.Models;
using Krizium.KidsReadingApp.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Models;
using Krizium.KidsReadingApp.Core.Interfaces;

namespace Krizium.KidsReadingApp.Api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(List<BookDto> Books, int TotalCount)> GetAllBooksAsync(int? ageMin = null, int? ageMax = null, string? category = null, int page = 1, int pageSize = 20)
        {
            try
            {
                // Calculate skip for pagination
                int skip = (page - 1) * pageSize;
                
                // Get books from repository
                //var books = await _bookRepository.GetAllBooksAsync(ageMin, ageMax, category, skip, pageSize);
                var books = await _bookRepository.GetAllBooksAsync();

                // Get total count for pagination
                var totalCount = 1; //TODO: await _bookRepository.GetBookCountAsync(ageMin, ageMax, category);
                
                // Map to DTOs
                var bookDtos = books.Select(MapBookToDto).ToList();
                
                // Add page count for each book
                foreach (var bookDto in bookDtos)
                {
                    //bookDto.PageCount = await _bookRepository.GetBookPageCountAsync(bookDto.Id);
                }
                
                return (bookDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                throw;
            }
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                if (book == null)
                {
                    return null;
                }
                
                var bookDto = MapBookToDto(book);
                
                // Get page summaries
                //var pages = await _bookRepository.GetBookPagesAsync(id);
                //bookDto.Pages = pages.Select(p => new PageSummaryDto
                //{
                //    Id = p.Id,
                //    PageNumber = p.PageNumber,
                //    ImageUrl = p.ImageUrl
                //}).ToList();
                
                //bookDto.PageCount = pages.Count;
                
                return bookDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting book with ID {id}");
                throw;
            }
        }

        public async Task<PageDto?> GetBookPageAsync(int bookId, int pageNumber)
        {
            try
            {
                var page = await _bookRepository.GetBookPageAsync(bookId, pageNumber);
                if (page == null)
                {
                    return null;
                }
                
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    return null;
                }
                
                var pageDto = MapPageToDto(page);
                
                // Add book information
                pageDto.BookTitle = book.Title;
                pageDto.BookAuthor = book.Author;
                
                // Add navigation information
                pageDto.HasPreviousPage = pageNumber > 1;
                //pageDto.HasNextPage = pageNumber < await _bookRepository.GetBookPageCountAsync(bookId);
                
                return pageDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting page {pageNumber} for book {bookId}");
                throw;
            }
        }

        //public async Task<List<string>> GetCategoriesAsync()
        //{
        //    try
        //    {
        //        return await _bookRepository.GetCategoriesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting categories");
        //        throw;
        //    }
        //}

        //public async Task<BookDto> AddBookAsync(BookDto bookDto)
        //{
        //    try
        //    {
        //        // Map DTO to entity
        //        var book = new Book
        //        {
        //            Title = bookDto.Title,
        //            Author = bookDto.Author,
        //            CoverImageUrl = bookDto.CoverImageUrl,
        //            AgeRangeMin = bookDto.AgeRangeMin,
        //            AgeRangeMax = bookDto.AgeRangeMax,
        //            //Description = bookDto.Description,
        //            //Categories = string.Join(",", bookDto.Categories),
        //            //DateAdded = DateTime.UtcNow,
        //            //IsActive = true
        //        };
                
        //        // Add to repository
        //        var addedBook = await _bookRepository.AddBookAsync(book);
                
        //        // Map back to DTO
        //        return MapBookToDto(addedBook);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error adding book");
        //        throw;
        //    }
        //}

        //public async Task<bool> UpdateBookAsync(BookDto bookDto)
        //{
        //    try
        //    {
        //        // Check if book exists
        //        var existingBook = await _bookRepository.GetBookByIdAsync(bookDto.Id);
        //        if (existingBook == null)
        //        {
        //            return false;
        //        }
                
        //        // Map DTO to entity
        //        var book = new Book
        //        {
        //            Id = bookDto.Id,
        //            Title = bookDto.Title,
        //            Author = bookDto.Author,
        //            CoverImageUrl = bookDto.CoverImageUrl,
        //            AgeRangeMin = bookDto.AgeRangeMin,
        //            AgeRangeMax = bookDto.AgeRangeMax,
        //            //Description = bookDto.Description,
        //            //Categories = string.Join(",", bookDto.Categories),
        //            //IsActive = true
        //        };
                
        //        // Update in repository
        //        return await _bookRepository.UpdateBookAsync(book);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error updating book with ID {bookDto.Id}");
        //        throw;
        //    }
        //}

        //public async Task<bool> DeleteBookAsync(int id)
        //{
        //    try
        //    {
        //        return await _bookRepository.DeleteBookAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error deleting book with ID {id}");
        //        throw;
        //    }
        //}

        public async Task<PageDto> AddPageAsync(int bookId, PageDto pageDto)
        {
            try
            {
                // Check if book exists
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    throw new ArgumentException($"Book with ID {bookId} not found");
                }
                
                // TODO: Implement page adding logic
                // This would require extending the repository with page-specific methods
                
                throw new NotImplementedException("Adding pages is not yet implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding page to book {bookId}");
                throw;
            }
        }

        public async Task<bool> UpdatePageAsync(int bookId, PageDto pageDto)
        {
            try
            {
                // Check if book exists
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    return false;
                }
                
                // TODO: Implement page updating logic
                // This would require extending the repository with page-specific methods
                
                throw new NotImplementedException("Updating pages is not yet implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating page {pageDto.PageNumber} for book {bookId}");
                throw;
            }
        }

        public async Task<bool> DeletePageAsync(int bookId, int pageNumber)
        {
            try
            {
                // TODO: Implement page deletion logic
                // This would require extending the repository with page-specific methods
                
                throw new NotImplementedException("Deleting pages is not yet implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting page {pageNumber} for book {bookId}");
                throw;
            }
        }

        #region Mapping Methods

        private BookDto MapBookToDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CoverImageUrl = book.CoverImageUrl,
                AgeRangeMin = book.AgeRangeMin,
                AgeRangeMax = book.AgeRangeMax,
                //Description = book.Description,
                //DateAdded = book.DateAdded,
                //Categories = book.Categories?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                //    .Select(c => c.Trim())
                //    .ToList() ?? new List<string>()
            };
        }

        private PageDto MapPageToDto(Page page)
        {
            return new PageDto
            {
                Id = page.Id,
                BookId = page.BookId,
                PageNumber = page.PageNumber,
                ImageUrl = page.ImageUrl,
                //Paragraphs = page.Paragraphs.OrderBy(p => p.Order).Select(MapParagraphToDto).ToList()
            };
        }

        private ParagraphDto MapParagraphToDto(Paragraph paragraph)
        {
            return new ParagraphDto
            {
                Id = paragraph.Id,
                PageId = paragraph.PageId,
                //Order = paragraph.Order,
                //Words = paragraph.Words.OrderBy(w => w.Order).Select(MapWordToDto).ToList()
            };
        }

        private WordDto MapWordToDto(Word word)
        {
            return new WordDto
            {
                Id = word.Id,
                ParagraphId = word.ParagraphId,
                Text = word.Text,
                //Order = word.Order,
                //AudioUrl = word.AudioUrl,
                //DifficultyLevel = word.DifficultyLevel,
                //IsVocabularyWord = word.IsVocabularyWord
            };
        }

        #endregion
    }
}
