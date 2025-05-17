using Krizium.KidsReadingApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public class MauiTtsService : ITtsService
    {
        private readonly ILogger<MauiTtsService> _logger;
        private readonly IFileService _fileService;
        private CancellationTokenSource _cancelSpeechCts;

        public MauiTtsService(ILogger<MauiTtsService> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
            _cancelSpeechCts = new CancellationTokenSource();
        }

        public async Task<string> GetAudioUrlForWordAsync(string word)
        {
            try
            {
                string cachePath = await _fileService.GetCachedAudioPathAsync(word);

                if (File.Exists(cachePath))
                {
                    return $"file://{cachePath}";
                }

                // Generate and cache if not exists
                return await GenerateAndCacheWordAudioAsync(word);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting audio URL for word: {word}");
                return string.Empty;
            }
        }

        public async Task<byte[]> GetAudioBytesForWordAsync(string word)
        {
            try
            {
                string cachePath = await _fileService.GetCachedAudioPathAsync(word);

                if (File.Exists(cachePath))
                {
                    return await File.ReadAllBytesAsync(cachePath);
                }

                // Generate audio - on MAUI this might involve platform-specific code
                // For now, we'll just return empty bytes
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting audio bytes for word: {word}");
                return Array.Empty<byte>();
            }
        }

        public async Task<string> GenerateAndCacheWordAudioAsync(string word)
        {
            // This is simplified - in a real app you might use platform APIs to generate audio files
            string cachePath = await _fileService.GetCachedAudioPathAsync(word);

            // For MAUI, we don't actually need to cache audio files since we can use the platform TTS
            // However, we'll maintain this API for consistency and future extensibility

            return $"tts://{Uri.EscapeDataString(word)}";
        }

        public async Task<string> GenerateAndCacheSentenceAudioAsync(string sentence)
        {
            // Similar to word audio, but for sentences
            string hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(sentence))
            ).Replace("/", "_").Replace("+", "-").Substring(0, 20);

            string cachePath = await _fileService.GetCachedSentenceAudioPathAsync(hash);

            // For MAUI, return a special URI scheme that our app can interpret
            return $"tts-sentence://{Uri.EscapeDataString(sentence)}";
        }

        public bool IsWordCached(string word)
        {
            // Check if audio for this word is already cached
            string cachePath = _fileService.GetCachedAudioPathAsync(word).Result;
            return File.Exists(cachePath);
        }

        public async Task SpeakWordAsync(string word, float volume = 1.0f, float pitch = 1.0f, float rate = 1.0f)
        {
            try
            {
                // Cancel any ongoing speech
                CancelSpeech();
                _cancelSpeechCts = new CancellationTokenSource();

                // Use the platform's text-to-speech capabilities
                var settings = new Microsoft.Maui.Media.SpeechOptions
                {
                    Volume = volume,
                    Pitch = pitch,
                    //Locale = "en-US" //TODO
                };

                // Convert rate to MAUI compatible range (typically 0.0 to 2.0)
                // where 1.0 is normal speed
                //settings.Rate = rate; TODO

                await TextToSpeech.Default.SpeakAsync(word, settings, _cancelSpeechCts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error speaking word: {word}");
            }
        }

        public async Task SpeakSentenceAsync(string sentence, float volume = 1.0f, float pitch = 1.0f, float rate = 1.0f)
        {
            try
            {
                // Cancel any ongoing speech
                CancelSpeech();
                _cancelSpeechCts = new CancellationTokenSource();

                var settings = new Microsoft.Maui.Media.SpeechOptions
                {
                    Volume = volume,
                    Pitch = pitch,
                    //Locale = "en-US", //TODO
                    //Rate = rate
                };

                await TextToSpeech.Default.SpeakAsync(sentence, settings, _cancelSpeechCts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error speaking sentence: {sentence}");
            }
        }

        public void CancelSpeech()
        {
            try
            {
                if (_cancelSpeechCts != null && !_cancelSpeechCts.IsCancellationRequested)
                {
                    _cancelSpeechCts.Cancel();
                    _cancelSpeechCts.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling speech");
            }
        }
    }

    // MAUI-specific speech options
    public class SpeechOptions : IDisposable
    {
        public float Volume { get; set; } = 1.0f;
        public float Pitch { get; set; } = 1.0f;
        public float Rate { get; set; } = 1.0f;
        public string Locale { get; set; } = "en-US";

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
