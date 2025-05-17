using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Core.Interfaces
{

    public interface IFileService
    {
        /// <summary>
        /// Gets the path for cached audio for a word
        /// </summary>
        Task<string> GetCachedAudioPathAsync(string word);

        /// <summary>
        /// Gets the path for cached audio for a sentence
        /// </summary>
        Task<string> GetCachedSentenceAudioPathAsync(string hash);

        /// <summary>
        /// Saves a file to the specified path
        /// </summary>
        Task<bool> SaveFileAsync(string path, byte[] data);

        /// <summary>
        /// Reads a file from the specified path
        /// </summary>
        Task<byte[]> ReadFileAsync(string path);

        /// <summary>
        /// Deletes a file at the specified path
        /// </summary>
        Task<bool> DeleteFileAsync(string path);

        /// <summary>
        /// Checks if a file exists at the specified path
        /// </summary>
        Task<bool> FileExistsAsync(string path);

        /// <summary>
        /// Gets the path for book content
        /// </summary>
        Task<string> GetBookContentPathAsync(int bookId);

    }

}
