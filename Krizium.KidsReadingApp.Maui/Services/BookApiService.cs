using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Core.Models;
using Page = Krizium.KidsReadingApp.Core.Models.Page;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class BookApiService : IBookApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookApiService> _logger;
        private readonly string _baseUrl;

        // JSON serialization options
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public BookApiService(HttpClient httpClient, ILogger<BookApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set the base URL for the API
            _baseUrl = "https://api.kidsreadingapp.com/api"; // Replace with your actual API URL
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Book>>($"{_baseUrl}/books", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books from API");
                throw;
            }
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Book>($"{_baseUrl}/books/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID {id} from API");
                throw;
            }
        }

        public async Task<Page> GetBookPageAsync(int bookId, int pageNumber)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Page>($"{_baseUrl}/books/{bookId}/pages/{pageNumber}", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving page {pageNumber} for book {bookId} from API");
                throw;
            }
        }

        public async Task<bool> UpdateReadingProgressAsync(int userId, int bookId, int pageNumber)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { userId, bookId, pageNumber }),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/progress", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reading progress for book {bookId}, page {pageNumber}");
                return false;
            }
        }

        public async Task<ReadingProgress> GetReadingProgressAsync(int userId, int bookId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ReadingProgress>(
                    $"{_baseUrl}/progress/{userId}/{bookId}",
                    _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving reading progress for book {bookId}");
                throw;
            }
        }

        public async Task<List<BookProgress>> GetRecentBooksAsync(int userId, int count = 5)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<BookProgress>>(
                    $"{_baseUrl}/progress/{userId}/recent?count={count}",
                    _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent books from API");
                throw;
            }
        }
    }
}
