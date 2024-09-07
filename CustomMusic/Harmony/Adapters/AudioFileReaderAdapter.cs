using System;
using CustomMusic.Harmony.Adapters;
using NAudio.Wave;

namespace CustomMusic.Harmony.Adapters
{
    public interface IAudioFileReaderAdapter : IDisposable
    {
        int Read(float[] buffer, int offset, int count);
        IWaveFormatAdapter WaveFormat { get; }
        long Position { get; set; }
    }

    public class AudioFileReaderAdapter : IAudioFileReaderAdapter
    {
        private readonly AudioFileReader _audioFileReader;

        public AudioFileReaderAdapter(AudioFileReader audioFileReader)
        {
            _audioFileReader = audioFileReader;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            return _audioFileReader.Read(buffer, offset, count);
        }

        public IWaveFormatAdapter WaveFormat => new WaveFormatAdapter(_audioFileReader.WaveFormat);

        public long Position
        {
            get => _audioFileReader.Position;
            set => _audioFileReader.Position = value;
        }

        public void Dispose()
        {
            _audioFileReader.Dispose();
        }
    }
}