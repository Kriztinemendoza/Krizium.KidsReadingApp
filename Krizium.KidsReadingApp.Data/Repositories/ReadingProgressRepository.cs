using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Data.Repositories
{
    public class ReadingProgressRepository : IReadingProgressRepository
    {
        private readonly AppDbContext _dbContext;

        public ReadingProgressRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<ReadingProgress>> GetUserProgressAsync(int userId)
        {
            var progressEntities = await _dbContext.ReadingProgress
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.LastReadTime)
                .ToListAsync();

            return progressEntities.Select(MapToReadingProgress).ToList();
        }

        public async Task<ReadingProgress> GetProgressForBookAsync(int userId, int bookId)
        {
            var progressEntity = await _dbContext.ReadingProgress
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

            return progressEntity != null ? MapToReadingProgress(progressEntity) : null;
        }

        public async Task<bool> SaveProgressAsync(ReadingProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            try
            {
                var entity = await _dbContext.ReadingProgress
                    .FirstOrDefaultAsync(p => p.UserId == progress.UserId && p.BookId == progress.BookId);

                if (entity == null)
                {
                    // Create new progress record
                    entity = new ReadingProgressEntity
                    {
                        UserId = progress.UserId,
                        BookId = progress.BookId,
                        LastPageRead = progress.LastPageRead,
                        LastReadTime = progress.LastReadTime,
                        TimesCompleted = progress.TimesCompleted
                    };

                    _dbContext.ReadingProgress.Add(entity);
                }
                else
                {
                    // Update existing progress
                    entity.LastPageRead = progress.LastPageRead;
                    entity.LastReadTime = progress.LastReadTime;
                    entity.TimesCompleted = progress.TimesCompleted;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ResetProgressAsync(int userId, int bookId)
        {
            try
            {
                var entity = await _dbContext.ReadingProgress
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

                if (entity != null)
                {
                    entity.LastPageRead = 1;
                    entity.TimesCompleted = 0;
                    entity.LastReadTime = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BookProgress>> GetRecentBooksAsync(int userId, int count = 5)
        {
            var query = from progress in _dbContext.ReadingProgress
                        join book in _dbContext.Books on progress.BookId equals book.Id
                        where progress.UserId == userId
                        orderby progress.LastReadTime descending
                        select new { Book = book, Progress = progress };

            var results = await query.Take(count).AsNoTracking().ToListAsync();

            return results.Select(r => new BookProgress
            {
                Book = MapToBook(r.Book),
                Progress = MapToReadingProgress(r.Progress)
            }).ToList();
        }

        #region Mapping Methods

        private ReadingProgress MapToReadingProgress(ReadingProgressEntity entity)
        {
            if (entity == null) return null;

            return new ReadingProgress
            {
                Id = entity.Id,
                UserId = entity.UserId,
                BookId = entity.BookId,
                LastPageRead = entity.LastPageRead,
                LastReadTime = entity.LastReadTime,
                TimesCompleted = entity.TimesCompleted
            };
        }

        private Book MapToBook(BookEntity entity)
        {
            if (entity == null) return null;

            return new Book
            {
                Id = entity.Id,
                Title = entity.Title,
                Author = entity.Author,
                CoverImageUrl = entity.CoverImageUrl,
                AgeRangeMin = entity.AgeRangeMin,
                AgeRangeMax = entity.AgeRangeMax
            };
        }

        #endregion
    }
}