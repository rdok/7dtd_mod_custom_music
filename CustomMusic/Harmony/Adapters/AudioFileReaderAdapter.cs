using System;
using CustomMusic.Harmony.Adapters;
using NAudio.Wave;

namespace CustomMusic.Harmony.Adapters
{
    public interface IAudioFileReaderAdapter : IDisposable, IWaveProvider
    {
        int Read(float[] buffer, int offset, int count);
        new IWaveFormatAdapter WaveFormat { get; }
        long Position { get; set; }
        float Volume { get; set; }
    }

    public class AudioFileReaderAdapter : IAudioFileReaderAdapter
    {
        private readonly AudioFileReader _audioFileReader;
        WaveFormat IWaveProvider.WaveFormat => _audioFileReader.WaveFormat;
        public IWaveFormatAdapter WaveFormat => new WaveFormatAdapter(_audioFileReader.WaveFormat);

        public AudioFileReaderAdapter(AudioFileReader audioFileReader)
        {
            _audioFileReader = audioFileReader;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            return _audioFileReader.Read(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _audioFileReader.Read(buffer, offset, count);
        }

        public long Position
        {
            get => _audioFileReader.Position;
            set => _audioFileReader.Position = value;
        }

        public float Volume
        {
            get => _audioFileReader.Volume;
            set => _audioFileReader.Volume = value;
        }

        public void Dispose()
        {
            _audioFileReader.Dispose();
        }
    }
}