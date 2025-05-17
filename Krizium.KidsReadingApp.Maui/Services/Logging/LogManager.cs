using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Maui.Services.Logging
{
    /// <summary>
    /// Manages application logging, including file rotation and cleanup
    /// </summary>
    public class LogManager
    {
        private readonly ILogger<LogManager> _logger;
        private readonly string _logDirectory;
        private readonly int _maxLogFiles;
        private readonly int _maxLogSizeInMB;

        public LogManager(ILogger<LogManager> logger)
        {
            _logger = logger;

            // Set up log directory in the app's data folder
            _logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Logs");
            Directory.CreateDirectory(_logDirectory);

            // Default settings
            _maxLogFiles = 5; // Keep 5 log files
            _maxLogSizeInMB = 5; // 5 MB per log file
        }

        /// <summary>
        /// Gets the current log file path
        /// </summary>
        public string GetCurrentLogFilePath()
        {
            return Path.Combine(_logDirectory, $"app_{DateTime.Now:yyyyMMdd}.log");
        }

        /// <summary>
        /// Cleans up old log files
        /// </summary>
        public async Task CleanupOldLogsAsync()
        {
            try
            {
                var logFiles = Directory.GetFiles(_logDirectory, "app_*.log")
                    .OrderByDescending(f => f)
                    .Skip(_maxLogFiles);

                foreach (var file in logFiles)
                {
                    File.Delete(file);
                    _logger.LogInformation($"Deleted old log file: {file}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old log files");
            }
        }

        /// <summary>
        /// Exports log files for user to access
        /// </summary>
        public async Task<string> ExportLogsAsync()
        {
            try
            {
                var exportPath = Path.Combine(FileSystem.CacheDirectory, "Logs");
                Directory.CreateDirectory(exportPath);

                // Copy all log files to the export directory
                foreach (var file in Directory.GetFiles(_logDirectory))
                {
                    var destFile = Path.Combine(exportPath, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }

                // Create a zip file
                var zipPath = Path.Combine(FileSystem.CacheDirectory, $"logs_{DateTime.Now:yyyyMMddHHmmss}.zip");

                // Note: For actual implementation, use a zip library from NuGet
                // such as SharpZipLib or System.IO.Compression
                // This is just a placeholder
                _logger.LogInformation($"Logs would be zipped to: {zipPath}");

                return zipPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting log files");
                return null;
            }
        }

        /// <summary>
        /// Gets logs content as text
        /// </summary>
        public async Task<string> GetLogsContentAsync(int maxLines = 1000)
        {
            try
            {
                var logFile = GetCurrentLogFilePath();
                if (!File.Exists(logFile))
                {
                    return "No log file found.";
                }

                // For large files, read only the last N lines
                var lines = File.ReadLines(logFile).Reverse().Take(maxLines).Reverse();
                return string.Join(Environment.NewLine, lines);
            }
            catch (Exception ex)
            {
                return $"Error reading log file: {ex.Message}";
            }
        }
    }
}
