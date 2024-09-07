using NAudio.Wave;

namespace CustomMusic.Harmony.Adapters
{
    interface IWaveOutEventAdapter
    {
        PlaybackState PlaybackState { get; }
        void Dispose();
        void Init(IAudioFileReaderAdapter audioFileReader);
        void Play();
    }

    public class WaveOutEventAdapter : IWaveOutEventAdapter
    {
        private readonly WaveOutEvent _waveOutEvent;

        public WaveOutEventAdapter(WaveOutEvent waveOutEventEvent)
        {
            _waveOutEvent = waveOutEventEvent;
        }

        public PlaybackState PlaybackState => _waveOutEvent.PlaybackState;

        public void Dispose()
        {
            _waveOutEvent.Dispose();
        }

        public void Init(IAudioFileReaderAdapter audioFileReader)
        {
            _waveOutEvent.Init(audioFileReader);
        }

        public void Play()
        {
            _waveOutEvent.Play();
        }
    }
}