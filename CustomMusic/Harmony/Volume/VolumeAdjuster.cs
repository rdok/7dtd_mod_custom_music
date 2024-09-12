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
            float overallAudioVolumeLevel,
            float preCalculatedMaxDecibel
        )
        {
            if (!masterAudioMixer.GetFloat("dmsVol", out var dynamicMusicVolumeInDecibels))
            {
                _logger.Error("Failed to retrieve 'dmsVol' from masterAudioMixer.");
                return;
            }

            _logger.Debug($"Dynamic music volume (in decibels): {dynamicMusicVolumeInDecibels} dB");

            if (float.IsNegativeInfinity(preCalculatedMaxDecibel))
            {
                _logger.Warn("No valid pre-calculated max decibel value. Skipping volume adjustment.");
                return;
            }

            _logger.Debug($"Pre-calculated max decibel level: {preCalculatedMaxDecibel} dB");

            var decibelDifference = dynamicMusicVolumeInDecibels - preCalculatedMaxDecibel;
            _logger.Debug($"Decibel difference: {decibelDifference} dB");

            var adjustmentFactor = Mathf.Pow(10, decibelDifference / 20);

            var adjustmentPercentage = adjustmentFactor * 100;
            _logger.Debug($"Adjustment factor: {adjustmentPercentage:F2}%");

            var filledBars = Mathf.Clamp((int)(adjustmentPercentage / 10), 0, 10);
            string adjustmentBar;

            if (adjustmentPercentage > 100)
            {
                var overflowBars = Mathf.Clamp((int)((adjustmentPercentage - 100) / 10), 0, 10);
                adjustmentBar = new string('|', 10) + new string('+', overflowBars);
            }
            else
            {
                adjustmentBar = new string('|', filledBars).PadRight(10, '.');
            }

            _logger.Debug($"Adjustment bar: [{adjustmentBar}]");

            audioFileReader.Volume = adjustmentFactor * overallAudioVolumeLevel;
            _logger.Debug($"Final volume applied: {audioFileReader.Volume:F2}");
        }
    }
}