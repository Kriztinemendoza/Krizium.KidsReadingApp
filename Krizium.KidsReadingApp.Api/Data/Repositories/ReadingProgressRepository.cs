using Krizium.KidsReadingApp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Data.Repositories
{
    public class ReadingProgressRepository : IReadingProgressRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReadingProgressRepository> _logger;

        public ReadingProgressRepository(AppDbContext context, ILogger<ReadingProgressRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ReadingProgress?> GetReadingProgressAsync(int userId, int bookId)
        {
            try
            {
                return await _context.ReadingProgress
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rp => rp.UserId == userId && rp.BookId == bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reading progress for user {userId} and book {bookId}");
                throw;
            }
        }

        public async Task<List<ReadingProgress>> GetUserReadingProgressAsync(int userId)
        {
            try
            {
                return await _context.ReadingProgress
                    .Include(rp => rp.Book)
                    .Where(rp => rp.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reading progress for user {userId}");
                throw;
            }
        }

        public async Task<List<ReadingProgress>> GetRecentBooksAsync(int userId, int count = 5)
        {
            try
            {
                return await _context.ReadingProgress
                    .Include(rp => rp.Book)
                    .Where(rp => rp.UserId == userId && rp.Book.IsActive)
                    .OrderByDescending(rp => rp.LastReadTime)
                    .Take(count)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting recent books for user {userId}");
                throw;
            }
        }

        public async Task<ReadingProgress> UpsertReadingProgressAsync(ReadingProgress progress)
        {
            try
            {
                var existingProgress = await _context.ReadingProgress
                    .FirstOrDefaultAsync(rp => rp.UserId == progress.UserId && rp.BookId == progress.BookId);

                if (existingProgress == null)
                {
                    // Add new progress
                    _context.ReadingProgress.Add(progress);
                }
                else
                {
                    // Update existing progress
                    existingProgress.LastPageRead = progress.LastPageRead;
                    existingProgress.LastReadTime = progress.LastReadTime;
                    existingProgress.TimesCompleted = progress.TimesCompleted;
                    existingProgress.TotalTimeSpentSeconds = progress.TotalTimeSpentSeconds;
                    
                    // Use the existing ID
                    progress = existingProgress;
                }

                await _context.SaveChangesAsync();
                return progress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error upserting reading progress for user {progress.UserId} and book {progress.BookId}");
                throw;
            }
        }

        public async Task<bool> DeleteReadingProgressAsync(int userId, int bookId)
        {
            try
            {
                var progress = await _context.ReadingProgress
                    .FirstOrDefaultAsync(rp => rp.UserId == userId && rp.BookId == bookId);

                if (progress == null)
                {
                    return false;
                }

                _context.ReadingProgress.Remove(progress);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reading progress for user {userId} and book {bookId}");
                throw;
            }
        }
    }
}
