using CustomMusic.Harmony.Adapters;
using UnityEngine;

namespace CustomMusic.Harmony.Volume
{
    public class VolumeAdjuster : IVolumeAdjuster
    {
        private readonly ILogger _logger;

        public VolumeAdjuster(ILogger logger)
        {
            _logger = logger;
        }

        public void Adjust(
            IAudioMixerAdapter masterAudioMixer,
            IAudioFileReaderAdapter audioFileReader,
            float preCalculatedMaxDecibel
        )
        {
            if (!masterAudioMixer.GetFloat("dmsVol", out var dynamicMusicVolumeInDecibels))
            {
                _logger.Error("Failed to retrieve 'dmsVol' from masterAudioMixer.");
                return;
            }

            _logger.Debug($"Dynamic music volume (in decibels): {dynamicMusicVolumeInDecibels}");

            if (float.IsNegativeInfinity(preCalculatedMaxDecibel))
            {
                _logger.Warn("No valid pre-calculated max decibel value. Skipping volume adjustment.");
                return;
            }

            _logger.Debug($"Pre-calculated max decibel level: {preCalculatedMaxDecibel} dB");

            var decibelDifference = dynamicMusicVolumeInDecibels - preCalculatedMaxDecibel;
            _logger.Debug($"Decibel difference: {decibelDifference} dB");

            var adjustmentFactor = Mathf.Pow(10, decibelDifference / 20);
            _logger.Debug($"Adjustment factor (linear scale): {adjustmentFactor}");

            audioFileReader.Volume = adjustmentFactor;
            _logger.Debug($"Final volume applied: {audioFileReader.Volume}");
        }
    }
}