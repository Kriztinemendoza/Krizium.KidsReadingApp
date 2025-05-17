using Krizium.KidsReadingApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Krizium.KidsReadingApp.Core.Interfaces;

namespace Krizium.KidsReadingApp.Core.Services
{
    public class MicrosoftTtsService : ITtsService
    {
        private readonly string _subscriptionKey;
        private readonly string _region;
        private readonly string _cacheDirectory;
        private readonly SpeechConfig _speechConfig;

        public MicrosoftTtsService(IConfiguration configuration)
        {
            _subscriptionKey = configuration["Speech:SubscriptionKey"];
            _region = configuration["Speech:Region"];
            _cacheDirectory = configuration["Speech:CacheDirectory"];

            // Ensure cache directory exists
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }

            // Initialize Speech SDK config
            _speechConfig = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            // Use a child-friendly voice
            _speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
        }

        public async Task<string> GetAudioUrlForWordAsync(string word)
        {
            string sanitizedWord = SanitizeFileName(word);
            string cacheFilePath = Path.Combine(_cacheDirectory, $"{sanitizedWord}.mp3");

            if (File.Exists(cacheFilePath))
            {
                // Return relative URL to cached file
                return $"/audio/{sanitizedWord}.mp3";
            }

            // Generate and cache if not exists
            return await GenerateAndCacheWordAudioAsync(word);
        }

        public async Task<byte[]> GetAudioBytesForWordAsync(string word)
        {
            string sanitizedWord = SanitizeFileName(word);
            string cacheFilePath = Path.Combine(_cacheDirectory, $"{sanitizedWord}.mp3");

            if (File.Exists(cacheFilePath))
            {
                return await File.ReadAllBytesAsync(cacheFilePath);
            }

            // Generate speech and cache
            using (var synthesizer = new SpeechSynthesizer(_speechConfig))
            {
                // Synthesize the word to audio
                var result = await synthesizer.SpeakTextAsync(word);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    await File.WriteAllBytesAsync(cacheFilePath, result.AudioData);
                    return result.AudioData;
                }
                else
                {
                    throw new Exception($"Speech synthesis failed: {result.Reason}");
                }
            }
        }

        public async Task<string> GenerateAndCacheWordAudioAsync(string word)
        {
            string sanitizedWord = SanitizeFileName(word);
            string cacheFilePath = Path.Combine(_cacheDirectory, $"{sanitizedWord}.mp3");

            using (var synthesizer = new SpeechSynthesizer(_speechConfig))
            {
                var result = await synthesizer.SpeakTextAsync(word);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    await File.WriteAllBytesAsync(cacheFilePath, result.AudioData);
                    return $"/audio/{sanitizedWord}.mp3";
                }
                else
                {
                    throw new Exception($"Speech synthesis failed: {result.Reason}");
                }
            }
        }

        public async Task<string> GenerateAndCacheSentenceAudioAsync(string sentence)
        {
            // Generate a unique hash for the sentence
            string hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                    .ComputeHash(Encoding.UTF8.GetBytes(sentence))
            ).Replace("/", "_").Replace("+", "-").Substring(0, 20);

            string cacheFilePath = Path.Combine(_cacheDirectory, $"sentence_{hash}.mp3");

            if (File.Exists(cacheFilePath))
            {
                return $"/audio/sentence_{hash}.mp3";
            }

            using (var synthesizer = new SpeechSynthesizer(_speechConfig))
            {
                var result = await synthesizer.SpeakTextAsync(sentence);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    await File.WriteAllBytesAsync(cacheFilePath, result.AudioData);
                    return $"/audio/sentence_{hash}.mp3";
                }
                else
                {
                    throw new Exception($"Speech synthesis failed: {result.Reason}");
                }
            }
        }

        public bool IsWordCached(string word)
        {
            string sanitizedWord = SanitizeFileName(word);
            string cacheFilePath = Path.Combine(_cacheDirectory, $"{sanitizedWord}.mp3");
            return File.Exists(cacheFilePath);
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
