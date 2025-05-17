using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Krizium.KidsReadingApp.Maui.Services
{
    /// <summary>
    /// Handles API requests with better error handling and retry logic
    /// </summary>
    public class ApiRequestHandler
    {
        private readonly HttpClient _httpClient;
        private readonly AppConfigService _configService;
        private readonly NetworkService _networkService;
        private readonly ILogger<ApiRequestHandler> _logger;

        // Default retry configuration
        private const int DefaultMaxRetries = 3;
        private readonly TimeSpan[] _defaultBackoffDelays = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(7)
        };

        // JSON serialization options
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiRequestHandler(
            HttpClient httpClient,
            AppConfigService configService,
            NetworkService networkService,
            ILogger<ApiRequestHandler> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Makes a GET request to the specified endpoint
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint, int maxRetries = DefaultMaxRetries)
        {
            // Check if network is available
            if (!_networkService.IsConnected)
            {
                throw new NetworkUnavailableException("Network is unavailable");
            }

            string url = BuildUrl(endpoint);
            _logger.LogInformation($"Making GET request to {url}");

            Exception lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        // Wait before retry with exponential backoff
                        TimeSpan delay = attempt <= _defaultBackoffDelays.Length
                            ? _defaultBackoffDelays[attempt - 1]
                            : TimeSpan.FromSeconds(15);

                        _logger.LogInformation($"Retrying request to {url} after {delay.TotalSeconds} seconds (attempt {attempt} of {maxRetries})");
                        await Task.Delay(delay);
                    }

                    // Make the request
                    var response = await _httpClient.GetAsync(url);

                    // Ensure successful status code
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response
                    return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"HTTP request failed: {ex.Message}");

                    // Don't retry if it's a client error (4xx)
                    if (ex.StatusCode.HasValue && (int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                    {
                        _logger.LogError(ex, $"Client error - not retrying: {ex.StatusCode}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"Request failed: {ex.Message}");
                }
            }

            // If we got here, all retries failed
            _logger.LogError(lastException, $"All retry attempts failed for {url}");
            throw new ApiException($"Failed to complete request after {maxRetries} attempts", lastException);
        }

        /// <summary>
        /// Makes a POST request to the specified endpoint
        /// </summary>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, int maxRetries = DefaultMaxRetries)
        {
            // Check if network is available
            if (!_networkService.IsConnected)
            {
                throw new NetworkUnavailableException("Network is unavailable");
            }

            string url = BuildUrl(endpoint);
            _logger.LogInformation($"Making POST request to {url}");

            Exception lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        // Wait before retry with exponential backoff
                        TimeSpan delay = attempt <= _defaultBackoffDelays.Length
                            ? _defaultBackoffDelays[attempt - 1]
                            : TimeSpan.FromSeconds(15);

                        _logger.LogInformation($"Retrying request to {url} after {delay.TotalSeconds} seconds (attempt {attempt} of {maxRetries})");
                        await Task.Delay(delay);
                    }

                    // Make the request
                    var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);

                    // Ensure successful status code
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response
                    return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"HTTP request failed: {ex.Message}");

                    // Don't retry if it's a client error (4xx)
                    if (ex.StatusCode.HasValue && (int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                    {
                        _logger.LogError(ex, $"Client error - not retrying: {ex.StatusCode}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"Request failed: {ex.Message}");
                }
            }

            // If we got here, all retries failed
            _logger.LogError(lastException, $"All retry attempts failed for {url}");
            throw new ApiException($"Failed to complete request after {maxRetries} attempts", lastException);
        }

        /// <summary>
        /// Makes a POST request to the specified endpoint with no response data
        /// </summary>
        public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data, int maxRetries = DefaultMaxRetries)
        {
            // Check if network is available
            if (!_networkService.IsConnected)
            {
                throw new NetworkUnavailableException("Network is unavailable");
            }

            string url = BuildUrl(endpoint);
            _logger.LogInformation($"Making POST request to {url}");

            Exception lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        // Wait before retry with exponential backoff
                        TimeSpan delay = attempt <= _defaultBackoffDelays.Length
                            ? _defaultBackoffDelays[attempt - 1]
                            : TimeSpan.FromSeconds(15);

                        _logger.LogInformation($"Retrying request to {url} after {delay.TotalSeconds} seconds (attempt {attempt} of {maxRetries})");
                        await Task.Delay(delay);
                    }

                    // Make the request
                    var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);

                    // Ensure successful status code
                    response.EnsureSuccessStatusCode();

                    return true;
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"HTTP request failed: {ex.Message}");

                    // Don't retry if it's a client error (4xx)
                    if (ex.StatusCode.HasValue && (int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                    {
                        _logger.LogError(ex, $"Client error - not retrying: {ex.StatusCode}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"Request failed: {ex.Message}");
                }
            }

            // If we got here, all retries failed
            _logger.LogError(lastException, $"All retry attempts failed for {url}");
            throw new ApiException($"Failed to complete request after {maxRetries} attempts", lastException);
        }

        private string BuildUrl(string endpoint)
        {
            string baseUrl = _configService.ApiBaseUrl;

            // Ensure base URL ends with a slash and endpoint doesn't start with one
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            if (endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Substring(1);
            }

            return baseUrl + endpoint;
        }
    }

    /// <summary>
    /// Exception for API errors
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for network unavailability
    /// </summary>
    public class NetworkUnavailableException : Exception
    {
        public NetworkUnavailableException(string message) : base(message) { }
    }
}