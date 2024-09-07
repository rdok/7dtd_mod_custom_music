using NAudio.Wave;

namespace CustomMusic.Harmony.Adapters
{
    public interface IWaveFormatAdapter
    {
        int SampleRate { get; }
    }

    public class WaveFormatAdapter : IWaveFormatAdapter
    {
        private readonly WaveFormat _waveFormat;

        public WaveFormatAdapter(WaveFormat waveFormat)
        {
            _waveFormat = waveFormat;
        }

        public int SampleRate => _waveFormat.SampleRate;
    }
}