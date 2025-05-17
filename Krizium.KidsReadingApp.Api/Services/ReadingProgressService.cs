//using Krizium.KidsReadingApp.Api.Models;
//using Krizium.KidsReadingApp.Api.Models.Requests;
//using Krizium.KidsReadingApp.Api.Services.Interfaces;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Krizium.KidsReadingApp.Core.Interfaces;

//namespace Krizium.KidsReadingApp.Api.Services
//{
//    public class ReadingProgressService : IReadingProgressService
//    {
//        private readonly IReadingProgressRepository _progressRepository;
//        private readonly IBookRepository _bookRepository;
//        private readonly ILogger<ReadingProgressService> _logger;

//        public ReadingProgressService(
//            IReadingProgressRepository progressRepository,
//            IBookRepository bookRepository,
//            ILogger<ReadingProgressService> logger)
//        {
//            _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
//            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<ReadingProgressDto?> GetReadingProgressAsync(int userId, int bookId)
//        {
//            try
//            {
//                var progress = await _progressRepository.GetReadingProgressAsync(userId, bookId);
//                if (progress == null)
//                {
//                    return null;
//                }
                
//                var book = await _bookRepository.GetBookByIdAsync(bookId);
//                if (book == null)
//                {
//                    return null;
//                }
                
//                return MapReadingProgressToDto(progress, book);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting reading progress for user {userId} and book {bookId}");
//                throw;
//            }
//        }

//        public async Task<List<ReadingProgressDto>> GetUserReadingProgressAsync(int userId)
//        {
//            try
//            {
//                var progressRecords = await _progressRepository.GetUserReadingProgressAsync(userId);
//                var result = new List<ReadingProgressDto>();
                
//                foreach (var progress in progressRecords)
//                {
//                    if (progress.Book != null)
//                    {
//                        result.Add(MapReadingProgressToDto(progress, progress.Book));
//                    }
//                }
                
//                return result;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting reading progress for user {userId}");
//                throw;
//            }
//        }

//        public async Task<List<BookProgressDto>> GetRecentBooksAsync(int userId, int count = 5)
//        {
//            try
//            {
//                var recentProgress = await _progressRepository.GetRecentBooksAsync(userId, count);
//                var result = new List<BookProgressDto>();
                
//                foreach (var progress in recentProgress)
//                {
//                    if (progress.Book != null)
//                    {
//                        var bookDto = new BookDto
//                        {
//                            Id = progress.Book.Id,
//                            Title = progress.Book.Title,
//                            Author = progress.Book.Author,
//                            CoverImageUrl = progress.Book.CoverImageUrl,
//                            AgeRangeMin = progress.Book.AgeRangeMin,
//                            AgeRangeMax = progress.Book.AgeRangeMax,
//                            PageCount = await _bookRepository.GetBookPageCountAsync(progress.Book.Id)
//                        };
                        
//                        var progressDto = MapReadingProgressToDto(progress, progress.Book);
                        
//                        result.Add(new BookProgressDto
//                        {
//                            Book = bookDto,
//                            Progress = progressDto
//                        });
//                    }
//                }
                
//                return result;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting recent books for user {userId}");
//                throw;
//            }
//        }

//        public async Task<ReadingProgressDto> UpdateReadingProgressAsync(ReadingProgressRequest request)
//        {
//            try
//            {
//                // Validate book exists
//                var book = await _bookRepository.GetBookByIdAsync(request.BookId);
//                if (book == null)
//                {
//                    throw new ArgumentException($"Book with ID {request.BookId} not found");
//                }
                
//                // Get existing progress or create new
//                var existingProgress = await _progressRepository.GetReadingProgressAsync(request.UserId, request.BookId);
                
//                var progress = new ReadingProgress
//                {
//                    UserId = request.UserId,
//                    BookId = request.BookId,
//                    LastPageRead = request.PageNumber,
//                    LastReadTime = DateTime.UtcNow
//                };
                
//                if (existingProgress != null)
//                {
//                    // Update existing record
//                    progress.Id = existingProgress.Id;
//                    progress.TimesCompleted = existingProgress.TimesCompleted;
//                    progress.TotalTimeSpentSeconds = existingProgress.TotalTimeSpentSeconds;
                    
//                    // Add time spent if provided
//                    if (request.TimeSpentSeconds.HasValue)
//                    {
//                        progress.TotalTimeSpentSeconds += request.TimeSpentSeconds.Value;
//                    }
                    
//                    // Update completion count if book was completed
//                    if (request.IsBookCompleted)
//                    {
//                        progress.TimesCompleted += 1;
//                    }
//                }
//                else
//                {
//                    // Set initial values for new record
//                    progress.TimesCompleted = request.IsBookCompleted ? 1 : 0;
//                    progress.TotalTimeSpentSeconds = request.TimeSpentSeconds ?? 0;
//                }
                
//                // Save to repository
//                var updatedProgress = await _progressRepository.UpsertReadingProgressAsync(progress);
                
//                return MapReadingProgressToDto(updatedProgress, book);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error updating reading progress for user {request.UserId} and book {request.BookId}");
//                throw;
//            }
//        }

//        public async Task<bool> ResetReadingProgressAsync(int userId, int bookId)
//        {
//            try
//            {
//                // Check if progress exists
//                var progress = await _progressRepository.GetReadingProgressAsync(userId, bookId);
//                if (progress == null)
//                {
//                    return false;
//                }
                
//                // Reset progress values
//                progress.LastPageRead = 1;
//                progress.TimesCompleted = 0;
//                progress.LastReadTime = DateTime.UtcNow;
                
//                // Save to repository
//                await _progressRepository.UpsertReadingProgressAsync(progress);
                
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error resetting reading progress for user {userId} and book {bookId}");
//                throw;
//            }
//        }

//        public async Task<bool> DeleteReadingProgressAsync(int userId, int bookId)
//        {
//            try
//            {
//                return await _progressRepository.DeleteReadingProgressAsync(userId, bookId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error deleting reading progress for user {userId} and book {bookId}");
//                throw;
//            }
//        }

//        #region Mapping Methods

//        private ReadingProgressDto MapReadingProgressToDto(ReadingProgress progress, Book book)
//        {
//            int totalPages = _bookRepository.GetBookPageCountAsync(book.Id).GetAwaiter().GetResult();
//            int percentComplete = totalPages > 0 
//                ? (int)Math.Round((double)progress.LastPageRead / totalPages * 100) 
//                : 0;
            
//            // Cap at 100%
//            if (percentComplete > 100)
//            {
//                percentComplete = 100;
//            }
            
//            return new ReadingProgressDto
//            {
//                Id = progress.Id,
//                UserId = progress.UserId,
//                BookId = progress.BookId,
//                LastPageRead = progress.LastPageRead,
//                LastReadTime = progress.LastReadTime,
//                TimesCompleted = progress.TimesCompleted,
//                PercentComplete = percentComplete,
//                BookTitle = book.Title,
//                BookCoverUrl = book.CoverImageUrl
//            };
//        }

//        #endregion
//    }
//}
