using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class ReadingProgressService
    {
        private readonly IReadingProgressRepository _progressRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<ReadingProgressService> _logger;

        // For a simple app, we'll use a fixed user ID
        // In a real application, this would come from authentication
        private const int DEFAULT_USER_ID = 1;

        public ReadingProgressService(
            IReadingProgressRepository progressRepository,
            IBookRepository bookRepository,
            ILogger<ReadingProgressService> logger)
        {
            _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ReadingProgress> GetBookProgressAsync(int bookId)
        {
            try
            {
                var progress = await _progressRepository.GetProgressForBookAsync(DEFAULT_USER_ID, bookId);

                if (progress == null)
                {
                    // Create a new progress record if one doesn't exist
                    progress = new ReadingProgress
                    {
                        UserId = DEFAULT_USER_ID,
                        BookId = bookId,
                        LastPageRead = 1,
                        LastReadTime = DateTime.UtcNow,
                        TimesCompleted = 0
                    };

                    await _progressRepository.SaveProgressAsync(progress);
                }

                return progress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reading progress for book {bookId}");

                // Return a default progress so the app doesn't crash
                return new ReadingProgress
                {
                    UserId = DEFAULT_USER_ID,
                    BookId = bookId,
                    LastPageRead = 1,
                    LastReadTime = DateTime.UtcNow,
                    TimesCompleted = 0
                };
            }
        }

        public async Task UpdateProgressAsync(int bookId, int pageNumber)
        {
            try
            {
                var progress = await _progressRepository.GetProgressForBookAsync(DEFAULT_USER_ID, bookId);

                if (progress == null)
                {
                    // Create new progress
                    progress = new ReadingProgress
                    {
                        UserId = DEFAULT_USER_ID,
                        BookId = bookId,
                        LastPageRead = pageNumber,
                        LastReadTime = DateTime.UtcNow,
                        TimesCompleted = 0
                    };
                }
                else
                {
                    // Update existing progress
                    progress.LastPageRead = pageNumber;
                    progress.LastReadTime = DateTime.UtcNow;

                    // Check if book was completed
                    var book = await _bookRepository.GetBookByIdAsync(bookId);
                    if (book != null && book.Pages.Count > 0)
                    {
                        int lastPageNumber = book.Pages.Count;
                        if (pageNumber >= lastPageNumber)
                        {
                            progress.TimesCompleted++;
                        }
                    }
                }

                await _progressRepository.SaveProgressAsync(progress);
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

        public async Task ResetProgressForBookAsync(int bookId)
        {
            try
            {
                await _progressRepository.ResetProgressAsync(DEFAULT_USER_ID, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting progress for book {bookId}");
            }
        }

        public async Task<int> GetLastReadPageForBookAsync(int bookId)
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

        public async Task<bool> HasStartedBookAsync(int bookId)
        {
            try
            {
                var progress = await _progressRepository.GetProgressForBookAsync(DEFAULT_USER_ID, bookId);
                return progress != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if book {bookId} has been started");
                return false;
            }
        }
    }
}