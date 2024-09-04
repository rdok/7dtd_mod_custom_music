using NAudio.Wave;
using UnityEngine;

namespace CustomMusic.Harmony
{
    public class VolumeAdjuster
    {
        private readonly ILogger _logger = new Logger();

        /// <summary>
        /// Adjusts the volume of the given AudioFileReader based on game settings.
        /// </summary>
        public void Adjust(AudioFileReader audioFileReader)
        {
            // Retrieve the dynamic music volume in decibels
            if (!GameManager.Instance.masterAudioMixer.GetFloat("dmsVol", out var dynamicMusicVolumeInDecibels))
            {
                _logger.Error("Failed to retrieve 'dmsVol' from masterAudioMixer.");
                return;
            }

            _logger.Debug($"Dynamic music volume (in decibels): {dynamicMusicVolumeInDecibels}");

            // Retrieve and cap the master volume at 1.0 (100%)
            var masterVolumeLevel = Mathf.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
            _logger.Debug($"Master volume (capped at 1f): {masterVolumeLevel}");

            // Convert dynamic music volume from decibels to a linear scale
            var linearDynamicMusicVolume = Mathf.Pow(10, dynamicMusicVolumeInDecibels / 20);
            _logger.Debug($"Linear dynamic music volume: {linearDynamicMusicVolume}");

            // Calculate the target linear volume as the product of linear dynamic music volume and master volume level
            var targetLinearVolume = linearDynamicMusicVolume * masterVolumeLevel;
            _logger.Debug($"Target linear volume: {targetLinearVolume}");

            // Apply the final volume to the audio file reader (clamped to 0f - 1f)
            audioFileReader.Volume = Mathf.Clamp(targetLinearVolume, 0f, 1f);
            _logger.Debug($"Final volume applied: {audioFileReader.Volume}");
        }
    }
}