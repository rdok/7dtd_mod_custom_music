using CustomMusic.Harmony.Adapters;
using UnityEngine;

namespace CustomMusic.Harmony.Volume
{
    public class VolumeAnalyzer : IVolumeAnalyzer
    {
        private readonly ILogger _logger;

        public VolumeAnalyzer(ILogger logger)
        {
            _logger = logger;
        }

        public float FindMaxDecibel(IAudioFileReaderAdapter audioFileReader)
        {
            var maxAmplitude = 0f;
            var sampleBuffer = new float[audioFileReader.WaveFormat.SampleRate];
            int samplesRead;

            _logger.Debug("Starting maximum decibel calculation.");

            while ((samplesRead = audioFileReader.Read(sampleBuffer, 0, sampleBuffer.Length)) > 0)
            {
                for (var i = 0; i < samplesRead; i++)
                {
                    var sample = Mathf.Abs(sampleBuffer[i]);
                    if (sample > maxAmplitude) maxAmplitude = sample;
                }
            }

            if (maxAmplitude <= 0f)
            {
                _logger.Warn("No valid samples found. Returning -infinity dB.");
                return float.NegativeInfinity;
            }

            var maxDecibels = 20 * Mathf.Log10(maxAmplitude);
            _logger.Debug($"Maximum decibel level found: {maxDecibels} dB");

            return maxDecibels;
        }
    }
}