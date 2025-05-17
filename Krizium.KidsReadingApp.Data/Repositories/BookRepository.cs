using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Krizium.KidsReadingApp.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            var bookEntities = await _dbContext.Books
                .AsNoTracking()
                .OrderBy(b => b.Title)
                .ToListAsync();

            return bookEntities.Select(MapToBook).ToList();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            var bookEntity = await _dbContext.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return bookEntity != null ? MapToBook(bookEntity) : null;
        }

        public async Task<Page> GetBookPageAsync(int bookId, int pageNumber)
        {
            var pageEntity = await _dbContext.Pages
                .AsNoTracking()
                .Include(p => p.Paragraphs)
                .ThenInclude(p => p.Words)
                .FirstOrDefaultAsync(p => p.BookId == bookId && p.PageNumber == pageNumber);

            return pageEntity != null ? MapToPage(pageEntity) : null;
        }

        public async Task<bool> UpdateReadingProgressAsync(int userId, int bookId, int pageNumber)
        {
            try
            {
                var progressEntity = await _dbContext.ReadingProgress
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

                if (progressEntity == null)
                {
                    // Create new progress record
                    progressEntity = new ReadingProgressEntity
                    {
                        UserId = userId,
                        BookId = bookId,
                        LastPageRead = pageNumber,
                        LastReadTime = DateTime.UtcNow,
                        TimesCompleted = 0
                    };

                    _dbContext.ReadingProgress.Add(progressEntity);
                }
                else
                {
                    // Update existing progress
                    progressEntity.LastPageRead = pageNumber;
                    progressEntity.LastReadTime = DateTime.UtcNow;

                    // Check if book was completed
                    var book = await _dbContext.Books
                        .Include(b => b.Pages)
                        .FirstOrDefaultAsync(b => b.Id == bookId);

                    if (book != null &&
                        pageNumber == book.Pages.Max(p => p.PageNumber) &&
                        progressEntity.LastPageRead < pageNumber)
                    {
                        progressEntity.TimesCompleted++;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> MarkBookForOfflineAccessAsync(int bookId, bool available)
        {
            try
            {
                var book = await _dbContext.Books
                    .Include(b => b.Pages)
                    .ThenInclude(p => p.Paragraphs)
                    .ThenInclude(p => p.Words)
                    .FirstOrDefaultAsync(b => b.Id == bookId);

                if (book == null)
                {
                    return false;
                }

                book.IsAvailableOffline = available;

                // If marking available offline, also mark all words
                if (available)
                {
                    foreach (var page in book.Pages)
                    {
                        foreach (var paragraph in page.Paragraphs)
                        {
                            foreach (var word in paragraph.Words)
                            {
                                word.IsAvailableOffline = true;
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ReadingProgress> GetReadingProgressAsync(int userId, int bookId)
        {
            var progressEntity = await _dbContext.ReadingProgress
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

            return progressEntity != null ? MapToReadingProgress(progressEntity) : null;
        }

        #region Mapping Methods

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
                AgeRangeMax = entity.AgeRangeMax,
                Pages = entity.Pages?.Select(MapToPage).ToList() ?? new List<Page>()
            };
        }

        private Page MapToPage(PageEntity entity)
        {
            if (entity == null) return null;

            return new Page
            {
                Id = entity.Id,
                BookId = entity.BookId,
                PageNumber = entity.PageNumber,
                ImageUrl = entity.ImageUrl,
                Paragraphs = entity.Paragraphs?.OrderBy(p => p.Order).Select(MapToParagraph).ToList() ?? new List<Paragraph>()
            };
        }

        private Paragraph MapToParagraph(ParagraphEntity entity)
        {
            if (entity == null) return null;

            return new Paragraph
            {
                Id = entity.Id,
                PageId = entity.PageId,
                Words = entity.Words?.OrderBy(w => w.Order).Select(MapToWord).ToList() ?? new List<Word>()
            };
        }

        private Word MapToWord(WordEntity entity)
        {
            if (entity == null) return null;

            return new Word
            {
                Id = entity.Id,
                ParagraphId = entity.ParagraphId,
                Text = entity.Text,
                AudioCacheKey = entity.AudioCacheKey,
                IsAvailableOffline = entity.IsAvailableOffline
            };
        }

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

        #endregion
    }
}