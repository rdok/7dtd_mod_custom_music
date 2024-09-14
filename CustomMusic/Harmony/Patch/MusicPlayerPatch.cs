using System.IO;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using DynamicMusic;
using HarmonyLib;
using MusicUtils.Enums;
using NAudio.Wave;
using UnityEngine;
using Random = System.Random;

namespace CustomMusic.Harmony.Patch
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Update))]
    public static class MusicPlayerPatch
    {
        private static readonly ILogger Logger = new Logger();
        public static WaveOutEventAdapter OutputDevice;
        private static int _currentTrackIndex = -1;
        private static int _previousTrackIndex = -1;
        private static readonly Random Random = new Random();
        private static IAudioFileReaderAdapter _audioFileReader;
        public static bool IsMusicEnabled { get; set; } = true;
        private static IVolumeAdjuster VolumeAdjuster;
        private static string _currentTrackPath;

        public static bool Prefix(Conductor __instance)
        {
            // Ensure __instance is not null
            if (__instance == null)
            {
                Logger.Error("MusicPlayerPatch: __instance (Conductor) is null.");
                return true; // Let the original method run
            }

            // Get the current section type directly
            var sectionType = __instance.CurrentSectionType;
            Logger.Debug($"Game wants to play section: {sectionType}");

            if (__instance.sectionSelector != null)
            {
                var key = __instance.sectionSelector.Select();
                Logger.Debug($"__instance.sectionSelector.Select(): {key}");
            }

            // Now you can use 'sectionType' to decide which music to play
            if (!IsMusicEnabled)
            {
                StopMusic();
                Logger.Debug("Dynamic music is disabled. Skipping custom music playback.");
                return false;
            }

            var customTracks = LoadTracksPatch.GetTracksForSection(sectionType);
            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Debug($"No custom music loaded for section {sectionType}, skipping Update.");
                return false;
            }

            if (OutputDevice == null)
            {
                OutputDevice = new WaveOutEventAdapter(new WaveOutEvent());
                Logger.Debug("Initialized WaveOutEvent output device.");
            }

            if (OutputDevice.PlaybackState == PlaybackState.Playing) return false;

            Logger.Debug("No music currently playing, starting next track.");

            PlayRandomTrackBasedOnSection(customTracks, sectionType);

            return false;
        }

        public static void StopMusic()
        {
            _audioFileReader?.Dispose();

            OutputDevice?.Dispose();
            OutputDevice = null;

            Logger.Debug("Stopped output device.");
        }

        public static void PlayRandomTrackBasedOnSection(string[] customTracks, SectionType sectionType)
        {
            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Debug($"No custom tracks for section {sectionType}, skipping playback.");
                return;
            }

            do
            {
                _currentTrackIndex = Random.Next(customTracks.Length);
            } while (_currentTrackIndex == _previousTrackIndex && customTracks.Length > 1);

            Logger.Debug(
                $"Selected track {_currentTrackIndex + 1} of {customTracks.Length} for section {sectionType}.");

            _audioFileReader = new AudioFileReaderAdapter(
                new AudioFileReader(customTracks[_currentTrackIndex])
            );

            _currentTrackPath = customTracks[_currentTrackIndex]; // Store the current track path

            UpdateVolume();

            OutputDevice.Init(_audioFileReader);
            OutputDevice.Play();
            Logger.Info(
                $"Started playing {Path.GetFileName(customTracks[_currentTrackIndex])} for section {sectionType}.");

            _previousTrackIndex = _currentTrackIndex;
        }

        public static void UpdateVolume()
        {
            if (_audioFileReader == null)
            {
                Logger.Debug("No audio file loaded. Skipping volume update.");
                return;
            }

            if (string.IsNullOrEmpty(_currentTrackPath))
            {
                Logger.Debug("No track path available for the currently playing track. Skipping volume update.");
                return;
            }

            var preCalculatedMaxDecibel = LoadTracksPatch.GetTrackMaxDecibel(_currentTrackPath);
            Logger.Debug($"preCalculatedMaxDecibel {_currentTrackPath}: {preCalculatedMaxDecibel}");

            var masterAudioMixer = new AudioMixerAdapter(GameManager.Instance.masterAudioMixer);
            if (VolumeAdjuster == null) VolumeAdjuster = Services.Get<IVolumeAdjuster>();

            var overallAudioVolumeLevel =
                Mathf.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
            VolumeAdjuster.Adjust(masterAudioMixer, _audioFileReader, overallAudioVolumeLevel, preCalculatedMaxDecibel);

            Logger.Debug("Volume successfully updated for the currently playing track.");
        }
    }
}