using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Page = Krizium.KidsReadingApp.Core.Models.Page;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class BookLibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookApiService _bookApiService;
        private readonly IReadingProgressRepository _progressRepository;
        private readonly ITtsService _ttsService;
        private readonly OfflineStorageService _offlineStorageService;
        private readonly NetworkService _networkService;
        private readonly ILogger<BookLibraryService> _logger;
        private const int DEFAULT_USER_ID = 1;

        public BookLibraryService(
            IBookRepository bookRepository,
            IBookApiService bookApiService,
            IReadingProgressRepository progressRepository,
            ITtsService ttsService,
            OfflineStorageService offlineStorageService,
            NetworkService networkService,
            ILogger<BookLibraryService> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _bookApiService = bookApiService ?? throw new ArgumentNullException(nameof(bookApiService));
            _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
            _ttsService = ttsService ?? throw new ArgumentNullException(nameof(ttsService));
            _offlineStorageService = offlineStorageService ?? throw new ArgumentNullException(nameof(offlineStorageService));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Subscribe to connectivity changes to trigger caching decisions
            _networkService.ConnectivityChanged += OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, bool isConnected)
        {
            // When connectivity is restored, we might want to sync local changes
            if (isConnected)
            {
                _logger.LogInformation("Network connectivity restored, syncing data...");
                // SyncLocalChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                // Try to get books from API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        var books = await _bookApiService.GetAllBooksAsync();

                        // Cache books for offline use
                        await CacheBooksLocallyAsync(books);

                        return books;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error retrieving books from API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, get from local storage
                var localBooks = await _bookRepository.GetAllBooksAsync();

                // If local DB is empty, try offline storage
                if (localBooks == null || localBooks.Count == 0)
                {
                    localBooks = await _offlineStorageService.GetOfflineBooksAsync();
                }

                return localBooks ?? new List<Book>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");

                // Last resort, try offline storage
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
                // Try to get book from API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        var book = await _bookApiService.GetBookByIdAsync(bookId);

                        // Cache book for offline use
                        if (book != null)
                        {
                            await CacheBookLocallyAsync(book);
                        }

                        return book;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error retrieving book {bookId} from API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, get from local storage
                var localBook = await _bookRepository.GetBookByIdAsync(bookId);

                // If not in local DB, try offline storage
                if (localBook == null)
                {
                    var offlineBooks = await _offlineStorageService.GetOfflineBooksAsync();
                    localBook = offlineBooks.Find(b => b.Id == bookId);
                }

                return localBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID {bookId}");

                // Last resort, try offline storage
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
                // Try to get page from API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        var page = await _bookApiService.GetBookPageAsync(bookId, pageNumber);

                        // Cache page for offline use
                        if (page != null)
                        {
                            await CachePageLocallyAsync(bookId, page);
                        }

                        // Update reading progress
                        await UpdateReadingProgressAsync(bookId, pageNumber);

                        return page;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error retrieving page {pageNumber} for book {bookId} from API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, get from local storage
                var localPage = await _bookRepository.GetBookPageAsync(bookId, pageNumber);

                // If not in local DB, try offline storage
                if (localPage == null)
                {
                    localPage = await _offlineStorageService.GetOfflinePageAsync(bookId, pageNumber);
                }

                // Update reading progress
                if (localPage != null)
                {
                    await UpdateReadingProgressAsync(bookId, pageNumber);
                }

                return localPage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving page {pageNumber} for book {bookId}");

                // Last resort, try offline storage
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
                // Get the complete book data first
                var book = await GetBookByIdAsync(bookId);
                if (book == null)
                {
                    return false;
                }

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
                // Try to update progress via API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        await _bookApiService.UpdateReadingProgressAsync(DEFAULT_USER_ID, bookId, pageNumber);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error updating reading progress via API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, update local storage
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
                // Try to get recent books from API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        return await _bookApiService.GetRecentBooksAsync(DEFAULT_USER_ID, count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error retrieving recent books from API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, get from local storage
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
                // Try to get reading progress from API if network is available
                if (_networkService.IsConnected)
                {
                    try
                    {
                        var progress = await _bookApiService.GetReadingProgressAsync(DEFAULT_USER_ID, bookId);
                        return progress?.LastPageRead ?? 1;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error retrieving reading progress from API, falling back to local storage");
                    }
                }

                // If API unavailable or network disconnected, get from local storage
                var localProgress = await _progressRepository.GetProgressForBookAsync(DEFAULT_USER_ID, bookId);
                return localProgress?.LastPageRead ?? 1;
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

        private async Task CacheBooksLocallyAsync(List<Book> books)
        {
            try
            {
                foreach (var book in books)
                {
                    await CacheBookLocallyAsync(book);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching books locally");
            }
        }

        private async Task CacheBookLocallyAsync(Book book)
        {
            // Implementation would save the book to local database
            // This is a simplified version
            _logger.LogInformation($"Book {book.Id} ({book.Title}) would be cached locally");
        }

        private async Task CachePageLocallyAsync(int bookId, Page page)
        {
            // Implementation would save the page to local database
            // This is a simplified version
            _logger.LogInformation($"Page {page.PageNumber} of book {bookId} would be cached locally");

            // Cache audio for words
            foreach (var paragraph in page.Paragraphs)
            {
                foreach (var word in paragraph.Words)
                {
                    if (!string.IsNullOrWhiteSpace(word.Text) && !_ttsService.IsWordCached(word.Text))
                    {
                        await _ttsService.GenerateAndCacheWordAudioAsync(word.Text);
                    }
                }
            }
        }
    }
}
