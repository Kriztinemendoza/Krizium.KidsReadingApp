using Microsoft.Extensions.Logging;
using System.Text.Json;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Page = Krizium.KidsReadingApp.Core.Models.Page;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class OfflineStorageService
    {
        private readonly IFileService _fileService;
        private readonly IBookRepository _bookRepository;
        private readonly ITtsService _ttsService;
        private readonly ILogger<OfflineStorageService> _logger;
        private readonly string _offlineBooksDirectory;

        public OfflineStorageService(
            IFileService fileService,
            IBookRepository bookRepository,
            ITtsService ttsService,
            ILogger<OfflineStorageService> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _ttsService = ttsService ?? throw new ArgumentNullException(nameof(ttsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create offline books directory
            _offlineBooksDirectory = Path.Combine(FileSystem.AppDataDirectory, "OfflineBooks");
            Directory.CreateDirectory(_offlineBooksDirectory);
        }

        public async Task<bool> MakeBookAvailableOfflineAsync(int bookId)
        {
            try
            {
                // 1. Get the book with all content
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null)
                    return false;

                // 2. Create directory for the book
                string bookDirectory = Path.Combine(_offlineBooksDirectory, bookId.ToString());
                Directory.CreateDirectory(bookDirectory);

                // 3. Save book metadata
                var bookMetadataPath = Path.Combine(bookDirectory, "metadata.json");
                var bookJson = JsonSerializer.Serialize(book, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(bookMetadataPath, bookJson);

                // 4. Process each page
                foreach (var page in book.Pages)
                {
                    // Get detailed page content with paragraphs and words
                    var pageDetails = await _bookRepository.GetBookPageAsync(bookId, page.PageNumber);
                    if (pageDetails == null) continue;

                    // Save page data
                    var pagePath = Path.Combine(bookDirectory, $"page_{page.PageNumber}.json");
                    var pageJson = JsonSerializer.Serialize(pageDetails, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(pagePath, pageJson);

                    // Download and cache page image if it exists
                    if (!string.IsNullOrEmpty(pageDetails.ImageUrl))
                    {
                        // This would be a placeholder for actual image downloading
                        // In a real app, you'd implement a more robust image downloader
                        _logger.LogInformation($"Image would be downloaded: {pageDetails.ImageUrl}");
                    }

                    // Process all words for TTS caching
                    var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var paragraph in pageDetails.Paragraphs)
                    {
                        foreach (var word in paragraph.Words)
                        {
                            if (!string.IsNullOrWhiteSpace(word.Text))
                            {
                                uniqueWords.Add(word.Text);
                            }
                        }
                    }

                    // Generate and cache TTS for each unique word
                    foreach (var word in uniqueWords)
                    {
                        if (!_ttsService.IsWordCached(word))
                        {
                            await _ttsService.GenerateAndCacheWordAudioAsync(word);
                        }
                    }
                }

                // 5. Mark book as available offline in the database
                await _bookRepository.MarkBookForOfflineAccessAsync(bookId, true);

                return true;
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
                // 1. Delete the book directory
                string bookDirectory = Path.Combine(_offlineBooksDirectory, bookId.ToString());
                if (Directory.Exists(bookDirectory))
                {
                    Directory.Delete(bookDirectory, true);
                }

                // 2. Mark book as not available offline in the database
                await _bookRepository.MarkBookForOfflineAccessAsync(bookId, false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing book {bookId} from offline storage");
                return false;
            }
        }

        public async Task<List<Book>> GetOfflineBooksAsync()
        {
            try
            {
                var result = new List<Book>();

                // Check if directory exists
                if (!Directory.Exists(_offlineBooksDirectory))
                    return result;

                // Get all book directories
                var bookDirectories = Directory.GetDirectories(_offlineBooksDirectory);
                foreach (var bookDir in bookDirectories)
                {
                    var metadataPath = Path.Combine(bookDir, "metadata.json");
                    if (File.Exists(metadataPath))
                    {
                        var json = await File.ReadAllTextAsync(metadataPath);
                        var book = JsonSerializer.Deserialize<Book>(json);
                        if (book != null)
                        {
                            result.Add(book);
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offline books");
                return new List<Book>();
            }
        }

        public async Task<Page> GetOfflinePageAsync(int bookId, int pageNumber)
        {
            try
            {
                string bookDirectory = Path.Combine(_offlineBooksDirectory, bookId.ToString());
                if (!Directory.Exists(bookDirectory))
                    return null;

                string pagePath = Path.Combine(bookDirectory, $"page_{pageNumber}.json");
                if (!File.Exists(pagePath))
                    return null;

                var json = await File.ReadAllTextAsync(pagePath);
                return JsonSerializer.Deserialize<Page>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting offline page {pageNumber} for book {bookId}");
                return null;
            }
        }

        public async Task<bool> IsBookAvailableOfflineAsync(int bookId)
        {
            string bookDirectory = Path.Combine(_offlineBooksDirectory, bookId.ToString());
            return Directory.Exists(bookDirectory) &&
                   File.Exists(Path.Combine(bookDirectory, "metadata.json"));
        }
    }
}