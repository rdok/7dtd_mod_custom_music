using CustomMusic.Harmony.Adapters;

namespace CustomMusic.Harmony.Volume
{
    public interface IVolumeAdjuster
    {
        void Adjust(
            IAudioMixerAdapter masterAudioMixer,
            IAudioFileReaderAdapter audioFileReader,
            float preCalculatedMaxDecibel
        );
    }
}