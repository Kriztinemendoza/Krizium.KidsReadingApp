using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Core.Interfaces;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class MauiFileService : IFileService
    {
        private readonly ILogger<MauiFileService> _logger;
        private readonly string _audioCacheDirectory;
        private readonly string _bookContentDirectory;

        public MauiFileService(ILogger<MauiFileService> logger)
        {
            _logger = logger;

            // Set up cache directories
            _audioCacheDirectory = Path.Combine(FileSystem.CacheDirectory, "AudioCache");
            _bookContentDirectory = Path.Combine(FileSystem.AppDataDirectory, "Books");

            // Ensure directories exist
            Directory.CreateDirectory(_audioCacheDirectory);
            Directory.CreateDirectory(_bookContentDirectory);
        }

        public async Task<string> GetCachedAudioPathAsync(string word)
        {
            try
            {
                string sanitizedWord = SanitizeFileName(word);
                return Path.Combine(_audioCacheDirectory, $"{sanitizedWord}.mp3");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cached audio path for word: {word}");
                return string.Empty;
            }
        }

        public async Task<string> GetCachedSentenceAudioPathAsync(string hash)
        {
            try
            {
                return Path.Combine(_audioCacheDirectory, $"sentence_{hash}.mp3");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cached sentence audio path for hash: {hash}");
                return string.Empty;
            }
        }

        public async Task<bool> SaveFileAsync(string path, byte[] data)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(path, data);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving file to path: {path}");
                return false;
            }
        }

        public async Task<byte[]> ReadFileAsync(string path)
        {
            try
            {
                if (await FileExistsAsync(path))
                {
                    return await File.ReadAllBytesAsync(path);
                }
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading file from path: {path}");
                return Array.Empty<byte>();
            }
        }

        public async Task<bool> DeleteFileAsync(string path)
        {
            try
            {
                if (await FileExistsAsync(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file at path: {path}");
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            try
            {
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if file exists at path: {path}");
                return false;
            }
        }

        public async Task<string> GetBookContentPathAsync(int bookId)
        {
            try
            {
                string bookDirectory = Path.Combine(_bookContentDirectory, bookId.ToString());

                if (!Directory.Exists(bookDirectory))
                {
                    Directory.CreateDirectory(bookDirectory);
                }

                return bookDirectory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting book content path for book ID: {bookId}");
                return string.Empty;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters from filename
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.ToLower();
        }
    }
}
