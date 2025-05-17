namespace Krizium.KidsReadingApp.Core.Interfaces;

public interface IAudioService
{
    Task PlayAudioAsync(string audioUrl);
    Task StopAudioAsync();
} 