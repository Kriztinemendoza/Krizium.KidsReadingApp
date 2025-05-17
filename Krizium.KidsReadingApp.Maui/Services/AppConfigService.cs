using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class AppConfigService
    {
        private readonly ILogger<AppConfigService> _logger;
        private AppConfig _config;
        private readonly string _configPath;

        public AppConfigService(ILogger<AppConfigService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set up config path
            _configPath = Path.Combine(FileSystem.AppDataDirectory, "app_config.json");

            // Load initial config
            LoadConfigAsync().ConfigureAwait(false);
        }

        public string ApiBaseUrl => _config?.ApiBaseUrl ?? "https://api.kidsreadingapp.com/api";

        public bool UseLocalStorageFirst => _config?.UseLocalStorageFirst ?? false;

        public int OfflineWordCacheLimit => _config?.OfflineWordCacheLimit ?? 1000;

        public int MaxOfflineBooks => _config?.MaxOfflineBooks ?? 10;

        public async Task UpdateApiUrlAsync(string newApiUrl)
        {
            if (string.IsNullOrWhiteSpace(newApiUrl))
            {
                throw new ArgumentException("API URL cannot be empty", nameof(newApiUrl));
            }

            _config.ApiBaseUrl = newApiUrl;
            await SaveConfigAsync();
        }

        public async Task SetUseLocalStorageFirstAsync(bool value)
        {
            _config.UseLocalStorageFirst = value;
            await SaveConfigAsync();
        }

        public async Task SetOfflineWordCacheLimitAsync(int limit)
        {
            if (limit < 0)
            {
                throw new ArgumentException("Cache limit cannot be negative", nameof(limit));
            }

            _config.OfflineWordCacheLimit = limit;
            await SaveConfigAsync();
        }

        public async Task SetMaxOfflineBooksAsync(int max)
        {
            if (max < 0)
            {
                throw new ArgumentException("Max offline books cannot be negative", nameof(max));
            }

            _config.MaxOfflineBooks = max;
            await SaveConfigAsync();
        }

        private async Task LoadConfigAsync()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _config = new AppConfig
                    {
                        ApiBaseUrl = "https://api.kidsreadingapp.com/api",
                        UseLocalStorageFirst = false,
                        OfflineWordCacheLimit = 1000,
                        MaxOfflineBooks = 10
                    };

                    await SaveConfigAsync();
                    return;
                }

                var json = await File.ReadAllTextAsync(_configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json);

                if (_config == null)
                {
                    throw new InvalidOperationException("Failed to deserialize configuration");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading application configuration");

                // Create default config
                _config = new AppConfig
                {
                    ApiBaseUrl = "https://api.kidsreadingapp.com/api",
                    UseLocalStorageFirst = false,
                    OfflineWordCacheLimit = 1000,
                    MaxOfflineBooks = 10
                };

                await SaveConfigAsync();
            }
        }

        private async Task SaveConfigAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(_configPath, json);
                _logger.LogInformation("Application configuration saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving application configuration");
            }
        }
    }

    public class AppConfig
    {
        public string ApiBaseUrl { get; set; }
        public bool UseLocalStorageFirst { get; set; }
        public int OfflineWordCacheLimit { get; set; }
        public int MaxOfflineBooks { get; set; }
    }
}