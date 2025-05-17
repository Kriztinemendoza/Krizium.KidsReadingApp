using System.Text.Json;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Core.Services;

public class BookService : IBookService
{
    private readonly IAudioService _audioService;
    private List<Book> _books = new();

    public BookService(IAudioService audioService)
    {
        _audioService = audioService;
        LoadSampleData();
    }

    public Task<Book?> GetBookByIdAsync(int id)
    {
        return Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
    }

    public Task<List<Book>> GetAllBooksAsync()
    {
        return Task.FromResult(_books);
    }

    public Task<Page?> GetPageAsync(int bookId, int pageNumber)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId);
        return Task.FromResult(book?.Pages.FirstOrDefault(p => p.PageNumber == pageNumber));
    }

    public async Task PlayWordAudioAsync(string audioUrl)
    {
        await _audioService.PlayAudioAsync(audioUrl);
    }

    public async Task PlayPageAudioAsync(int bookId, int pageNumber)
    {
        var page = await GetPageAsync(bookId, pageNumber);
        if (page != null)
        {
            //foreach (var word in page.Words)
            //{
            //    await PlayWordAudioAsync(word.AudioUrl);
            //    await Task.Delay(500); // Add a small delay between words
            //}
        }
    }

    public async Task PlayBookAudioAsync(int bookId)
    {
        var book = await GetBookByIdAsync(bookId);
        if (book != null)
        {
            foreach (var page in book.Pages.OrderBy(p => p.PageNumber))
            {
                await PlayPageAudioAsync(bookId, page.PageNumber);
                await Task.Delay(1000); // Add a delay between pages
            }
        }
    }

    private void LoadSampleData()
    {
        //var sampleBook = new Book
        //{
        //    Id = 1,
        //    Title = "Obedience to Parents",
        //    Author = "Krizium",
        //    CoverImageUrl = "images/book1-cover.jpg",
        //    Pages = new List<Page>
        //    {
        //        new Page
        //        {
        //            Id = 1,
        //            PageNumber = 1,
        //            ImageUrl = "images/book1-page1.jpg",
        //            Words = new List<Word>
        //            {
        //                new Word { Id = 1, Text = "Listen", AudioUrl = "audio/listen.mp3", PositionX = 100, PositionY = 100 },
        //                new Word { Id = 2, Text = "to", AudioUrl = "audio/to.mp3", PositionX = 200, PositionY = 100 },
        //                new Word { Id = 3, Text = "your", AudioUrl = "audio/your.mp3", PositionX = 300, PositionY = 100 },
        //                new Word { Id = 4, Text = "parents", AudioUrl = "audio/parents.mp3", PositionX = 400, PositionY = 100 }
        //            }
        //        },
        //        new Page
        //        {
        //            Id = 2,
        //            PageNumber = 2,
        //            ImageUrl = "images/book1-page2.jpg",
        //            Words = new List<Word>
        //            {
        //                new Word { Id = 5, Text = "They", AudioUrl = "audio/they.mp3", PositionX = 100, PositionY = 100 },
        //                new Word { Id = 6, Text = "love", AudioUrl = "audio/love.mp3", PositionX = 200, PositionY = 100 },
        //                new Word { Id = 7, Text = "you", AudioUrl = "audio/you.mp3", PositionX = 300, PositionY = 100 }
        //            }
        //        }
        //    }
        //};

        //_books.Add(sampleBook);
    }
} 