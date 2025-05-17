using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Core.Interfaces
{

    public interface ITtsService
    {
        /// <summary>
        /// Gets the URL for a cached audio file for a word
        /// </summary>
        Task<string> GetAudioUrlForWordAsync(string word);

        /// <summary>
        /// Gets the audio bytes for a word
        /// </summary>
        Task<byte[]> GetAudioBytesForWordAsync(string word);

        /// <summary>
        /// Generates and caches audio for a word
        /// </summary>
        Task<string> GenerateAndCacheWordAudioAsync(string word);

        /// <summary>
        /// Generates and caches audio for a sentence
        /// </summary>
        Task<string> GenerateAndCacheSentenceAudioAsync(string sentence);

        /// <summary>
        /// Checks if audio for a word is already cached
        /// </summary>
        bool IsWordCached(string word);

        /// <summary>
        /// Speaks a word using the device's text-to-speech capabilities
        /// </summary>
        Task SpeakWordAsync(string word, float volume = 1.0f, float pitch = 1.0f, float rate = 1.0f);

        /// <summary>
        /// Speaks a sentence using the device's text-to-speech capabilities
        /// </summary>
        Task SpeakSentenceAsync(string sentence, float volume = 1.0f, float pitch = 1.0f, float rate = 1.0f);

        /// <summary>
        /// Cancels any ongoing speech
        /// </summary>
        void CancelSpeech();
    }

}
