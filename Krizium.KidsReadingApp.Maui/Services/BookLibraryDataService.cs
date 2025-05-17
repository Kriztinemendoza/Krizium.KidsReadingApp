using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Page = Krizium.KidsReadingApp.Core.Models.Page;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class BookLibraryDataService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IReadingProgressRepository _progressRepository;
        private readonly ITtsService _ttsService;
        private readonly OfflineStorageService _offlineStorageService;
        private readonly ILogger<BookLibraryDataService> _logger;
        private const int DEFAULT_USER_ID = 1;

        public BookLibraryDataService(
            IBookRepository bookRepository,
            IReadingProgressRepository progressRepository,
            ITtsService ttsService,
            OfflineStorageService offlineStorageService,
            ILogger<BookLibraryDataService> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
            _ttsService = ttsService ?? throw new ArgumentNullException(nameof(ttsService));
            _offlineStorageService = offlineStorageService ?? throw new ArgumentNullException(nameof(offlineStorageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();

                // If we're offline and couldn't get online books, try to get offline books
                if (books == null || books.Count == 0)
                {
                    books = await _offlineStorageService.GetOfflineBooksAsync();
                }

                return books ?? new List<Book>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");

                // Try getting offline books as a fallback
                try
                {
                    return await _offlineStorageService.GetOfflineBooksAsync();
                }
                catch
                {
                    return new List<Book>();
                }
            }
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(bookId);

                // If book not found online, try offline
                if (book == null)
                {
                    var offlineBooks = await _offlineStorageService.GetOfflineBooksAsync();
                    book = offlineBooks.Find(b => b.Id == bookId);
                }

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID {bookId}");

                // Try getting from offline storage as a fallback
                try
                {
                    var offlineBooks = await _offlineStorageService.GetOfflineBooksAsync();
                    return offlineBooks.Find(b => b.Id == bookId);
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<Page> GetBookPageAsync(int bookId, int pageNumber)
        {
            try
            {
                // First try online
                var page = await _bookRepository.GetBookPageAsync(bookId, pageNumber);

                // If page not found online, try offline
                if (page == null)
                {
                    page = await _offlineStorageService.GetOfflinePageAsync(bookId, pageNumber);
                }

                // Update reading progress if page was found
                if (page != null)
                {
                    await UpdateReadingProgressAsync(bookId, pageNumber);
                }

                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving page {pageNumber} for book {bookId}");

                // Try getting from offline storage as fallback
                try
                {
                    return await _offlineStorageService.GetOfflinePageAsync(bookId, pageNumber);
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<bool> MakeBookAvailableOfflineAsync(int bookId)
        {
            try
            {
                return await _offlineStorageService.MakeBookAvailableOfflineAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error making book {bookId} available offline");
                return false;
            }
        }

        public async Task<bool> RemoveBookFromOfflineAsync(int bookId)
        {
            try
            {
                return await _offlineStorageService.RemoveBookFromOfflineAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing book {bookId} from offline storage");
                return false;
            }
        }

        public async Task<bool> IsBookAvailableOfflineAsync(int bookId)
        {
            try
            {
                return await _offlineStorageService.IsBookAvailableOfflineAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if book {bookId} is available offline");
                return false;
            }
        }

        private async Task UpdateReadingProgressAsync(int bookId, int pageNumber)
        {
            try
            {
                await _bookRepository.UpdateReadingProgressAsync(DEFAULT_USER_ID, bookId, pageNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reading progress for book {bookId}, page {pageNumber}");
            }
        }

        public async Task<List<BookProgress>> GetRecentBooksAsync(int count = 5)
        {
            try
            {
                return await _progressRepository.GetRecentBooksAsync(DEFAULT_USER_ID, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent books");
                return new List<BookProgress>();
            }
        }

        public async Task<int> GetLastReadPageAsync(int bookId)
        {
            try
            {
                var progress = await _progressRepository.GetProgressForBookAsync(DEFAULT_USER_ID, bookId);
                return progress?.LastPageRead ?? 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting last read page for book {bookId}");
                return 1; // Default to first page
            }
        }

        public async Task<bool> CacheWordAudioAsync(string word)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(word))
                    return false;

                await _ttsService.GenerateAndCacheWordAudioAsync(word);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error caching audio for word: {word}");
                return false;
            }
        }

        public async Task<List<Book>> GetOfflineBooksAsync()
        {
            try
            {
                var books = await _offlineStorageService.GetOfflineBooksAsync();

                return books ?? new List<Book>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");

                // Try getting offline books as a fallback
                try
                {
                    return await _offlineStorageService.GetOfflineBooksAsync();
                }
                catch
                {
                    return new List<Book>();
                }
            }
        }

        public async Task<Page> GetOfflineBookPageAsync(int bookId, int pageNumber)
        {
            //try
            //{
            //    var book = await _offlineStorageService.GetOfflineBookPageAsync();

            //    return book ?? new Book();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error retrieving books");
            //}
            return new Page();
        }
    }
}