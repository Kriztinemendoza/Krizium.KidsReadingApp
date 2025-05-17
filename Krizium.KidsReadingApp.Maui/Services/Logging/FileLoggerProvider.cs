using Microsoft.Extensions.Logging;

namespace Krizium.KidsReadingApp.Maui.Services.Logging
{
    /// <summary>
    /// Custom logger provider that logs to a file
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly LogLevel _minLevel;
        private readonly object _lock = new object();

        public FileLoggerProvider(string filePath, LogLevel minLevel)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _minLevel = minLevel;

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create empty log file or append to existing one
            if (!File.Exists(filePath))
            {
                using (var writer = File.CreateText(filePath))
                {
                    writer.WriteLine($"Log file created: {DateTime.Now}");
                }
            }
            else
            {
                // Truncate file if it's too large (over 5 MB)
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    // Create a backup of the old log
                    string backupPath = $"{filePath}.{DateTime.Now:yyyyMMddHHmmss}.bak";
                    File.Copy(filePath, backupPath);

                    // Truncate the file
                    using (var writer = File.CreateText(filePath))
                    {
                        writer.WriteLine($"Log file truncated: {DateTime.Now}");
                    }
                }
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _filePath, _minLevel, _lock);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
