using CustomMusic.Harmony.Adapters;

namespace CustomMusic.Harmony.Volume
{
    public interface IVolumeAnalyzer
    {
        float FindMaxDecibel(IAudioFileReaderAdapter audioFileReader);
    }
}