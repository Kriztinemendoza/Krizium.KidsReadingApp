using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;

namespace Krizium.KidsReadingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ITtsService _ttsService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(
            IBookRepository bookRepository,
            ITtsService ttsService,
            ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _ttsService = ttsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                return StatusCode(500, "An error occurred while retrieving books");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with id {id}");
                return StatusCode(500, "An error occurred while retrieving the book");
            }
        }

        [HttpGet("{bookId}/pages/{pageNumber}")]
        public async Task<IActionResult> GetBookPage(int bookId, int pageNumber)
        {
            try
            {
                var page = await _bookRepository.GetBookPageAsync(bookId, pageNumber);

                if (page == null)
                {
                    return NotFound();
                }

                // Prepare audio URLs for each word if available offline
                foreach (var paragraph in page.Paragraphs)
                {
                    foreach (var word in paragraph.Words)
                    {
                        if (word.IsAvailableOffline && !string.IsNullOrEmpty(word.AudioCacheKey))
                        {
                            // Set URL to cached audio
                            word.AudioCacheKey = $"/audio/{word.AudioCacheKey}.mp3";
                        }
                    }
                }

                return Ok(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving page {pageNumber} of book {bookId}");
                return StatusCode(500, "An error occurred while retrieving the book page");
            }
        }

        [HttpGet("word-audio/{word}")]
        public async Task<IActionResult> GetWordAudio(string word)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(word))
                {
                    return BadRequest("Word cannot be empty");
                }

                // Get audio bytes for the word
                byte[] audioBytes = await _ttsService.GetAudioBytesForWordAsync(word);

                // Return as audio file
                return File(audioBytes, "audio/mpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating audio for word: {word}");
                return StatusCode(500, "An error occurred while generating audio");
            }
        }

        [HttpGet("sentence-audio")]
        public async Task<IActionResult> GetSentenceAudio([FromQuery] string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return BadRequest("Text cannot be empty");
                }

                // Generate and get URL to the cached audio
                string audioUrl = await _ttsService.GenerateAndCacheSentenceAudioAsync(text);

                return Ok(new { audioUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating audio for sentence: {text}");
                return StatusCode(500, "An error occurred while generating audio");
            }
        }

        [HttpPost("{bookId}/cache-words")]
        public async Task<IActionResult> CacheBookWords(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(bookId);

                if (book == null)
                {
                    return NotFound();
                }

                // Track progress
                int totalWords = 0;
                int cachedWords = 0;
                HashSet<string> uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Collect all unique words in the book
                foreach (var page in book.Pages)
                {
                    foreach (var paragraph in page.Paragraphs)
                    {
                        foreach (var word in paragraph.Words)
                        {
                            totalWords++;
                            if (!string.IsNullOrWhiteSpace(word.Text))
                            {
                                uniqueWords.Add(word.Text);
                            }
                        }
                    }
                }

                // Cache each unique word
                foreach (string word in uniqueWords)
                {
                    if (!_ttsService.IsWordCached(word))
                    {
                        await _ttsService.GenerateAndCacheWordAudioAsync(word);
                    }
                    cachedWords++;
                }

                return Ok(new
                {
                    message = $"Successfully cached {cachedWords} unique words out of {totalWords} total words",
                    uniqueWordCount = uniqueWords.Count,
                    totalWordCount = totalWords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error caching words for book {bookId}");
                return StatusCode(500, "An error occurred while caching book words");
            }
        }

        [HttpPost("{bookId}/reading-progress")]
        public async Task<IActionResult> UpdateReadingProgress(
            int bookId,
            [FromBody] ReadingProgressUpdateRequest request)
        {
            try
            {
                if (request == null || bookId != request.BookId)
                {
                    return BadRequest("Invalid request");
                }

                // In a real app, you'd get the user from authentication
                int userId = 1; // Placeholder

                await _bookRepository.UpdateReadingProgressAsync(
                    userId,
                    request.BookId,
                    request.PageNumber);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reading progress for book {bookId}");
                return StatusCode(500, "An error occurred while updating reading progress");
            }
        }
    }

    public class ReadingProgressUpdateRequest
    {
        public int BookId { get; set; }
        public int PageNumber { get; set; }
    }
}

