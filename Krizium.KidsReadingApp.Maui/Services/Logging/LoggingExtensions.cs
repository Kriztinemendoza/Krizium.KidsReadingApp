using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Maui.Services.Logging
{
    /// <summary>
    /// Extensions for enhanced logging with method names and line numbers
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs informational message with caller information
        /// </summary>
        public static void LogInfoDetail(
            this ILogger logger,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogInformation(
                "[{MemberName}() in {FileName}:{LineNumber}] {Message}",
                memberName,
                Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                message);
        }

        /// <summary>
        /// Logs warning message with caller information
        /// </summary>
        public static void LogWarningDetail(
            this ILogger logger,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogWarning(
                "[{MemberName}() in {FileName}:{LineNumber}] {Message}",
                memberName,
                Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                message);
        }

        /// <summary>
        /// Logs error message with caller information
        /// </summary>
        public static void LogErrorDetail(
            this ILogger logger,
            Exception exception,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogError(
                exception,
                "[{MemberName}() in {FileName}:{LineNumber}] {Message}",
                memberName,
                Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                message);
        }

        /// <summary>
        /// Logs critical error with caller information
        /// </summary>
        public static void LogCriticalDetail(
            this ILogger logger,
            Exception exception,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogCritical(
                exception,
                "[{MemberName}() in {FileName}:{LineNumber}] {Message}",
                memberName,
                Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                message);
        }
    }
}
