using System.Media;
using Krizium.KidsReadingApp.Core.Interfaces;

namespace Krizium.KidsReadingApp.Core.Services;

public class AudioService : IAudioService
{
    //private SoundPlayer? _currentPlayer;

    public Task PlayAudioAsync(string audioUrl)
    {
        throw new NotImplementedException();
    }

    public Task StopAudioAsync()
    {
        throw new NotImplementedException();
    }

    //public async Task PlayAudioAsync(string audioUrl)
    //{
    //    try
    //    {
    //        // Stop any currently playing audio
    //        await StopAudioAsync();

    //        // Create a new SoundPlayer instance
    //        _currentPlayer = new SoundPlayer(audioUrl);
    //        _currentPlayer.Play();
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle audio playback errors
    //        Console.WriteLine($"Error playing audio: {ex.Message}");
    //    }
    //}

    //public Task StopAudioAsync()
    //{
    //    if (_currentPlayer != null)
    //    {
    //        _currentPlayer.Stop();
    //        _currentPlayer.Dispose();
    //        _currentPlayer = null;
    //    }
    //    return Task.CompletedTask;
    //}
} 