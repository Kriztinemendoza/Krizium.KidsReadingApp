using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class NetworkService
    {
        private readonly ILogger<NetworkService> _logger;
        private readonly HttpClient _httpClient;
        private bool _isConnected;

        public NetworkService(ILogger<NetworkService> logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            // Default to assuming we're connected until proven otherwise
            _isConnected = true;

            // Subscribe to connectivity changes
            Connectivity.ConnectivityChanged += OnConnectivityChanged;

            // Check initial connectivity
            CheckConnectivityAsync().ConfigureAwait(false);
        }

        public bool IsConnected => _isConnected;

        public event EventHandler<bool> ConnectivityChanged;

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var newConnected = e.NetworkAccess == NetworkAccess.Internet;
            if (_isConnected != newConnected)
            {
                _isConnected = newConnected;
                _logger.LogInformation($"Network connectivity changed: {(_isConnected ? "Connected" : "Disconnected")}");
                ConnectivityChanged?.Invoke(this, _isConnected);
            }
        }

        public async Task<bool> CheckConnectivityAsync()
        {
            try
            {
                var networkAccess = Connectivity.NetworkAccess;

                // Fast check first
                _isConnected = networkAccess == NetworkAccess.Internet;

                // If MAUI says we're connected, do a more reliable check with an actual HTTP request
                if (_isConnected)
                {
                    try
                    {
                        // Use a reliable endpoint for connectivity check
                        var timeoutTask = Task.Delay(3000); // 3 second timeout
                        var checkTask = _httpClient.GetAsync("https://api.kidsreadingapp.com/health");
                        var completedTask = await Task.WhenAny(checkTask, timeoutTask);

                        _isConnected = completedTask == checkTask && checkTask.Result.IsSuccessStatusCode;
                    }
                    catch
                    {
                        _isConnected = false;
                    }
                }

                _logger.LogInformation($"Network connectivity check: {(_isConnected ? "Connected" : "Disconnected")}");
                ConnectivityChanged?.Invoke(this, _isConnected);
                return _isConnected;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking network connectivity");
                _isConnected = false;
                ConnectivityChanged?.Invoke(this, _isConnected);
                return false;
            }
        }
    }
}
