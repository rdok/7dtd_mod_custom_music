using CustomMusic.Harmony.Adapters;

namespace CustomMusic.Harmony.Volume
{
    public interface IVolumeAdjuster
    {
        void Adjust(IAudioFileReaderAdapter audioFileReader, float preCalculatedMaxDecibel);
    }
}