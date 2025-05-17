using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Maui.Services.Logging
{
    /// <summary>
    /// Logger implementation that writes to a file
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;
        private readonly LogLevel _minLevel;
        private readonly object _lock;

        public FileLogger(string categoryName, string filePath, LogLevel minLevel, object lockObject)
        {
            _categoryName = categoryName;
            _filePath = filePath;
            _minLevel = minLevel;
            _lock = lockObject;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null; // Not implemented for file logger
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLevel;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {message}";

            if (exception != null)
            {
                logEntry += $"{Environment.NewLine}{exception}";
            }

            // Write to file in a thread-safe manner
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_filePath, logEntry + Environment.NewLine);
                }
                catch
                {
                    // If we can't write to the file, we can't really log this error anywhere
                    // In a more robust implementation, we might queue failed messages
                }
            }
        }
    }

    /// <summary>
    /// Logger extensions for easily adding file logging
    /// </summary>
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(
            this ILoggingBuilder builder,
            string filePath,
            LogLevel minLevel = LogLevel.Information)
        {
            builder.AddProvider(new FileLoggerProvider(filePath, minLevel));
            return builder;
        }
    }
}
