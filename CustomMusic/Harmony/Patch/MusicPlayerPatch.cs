using System.IO;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using DynamicMusic;
using HarmonyLib;
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

        public static bool Prefix(Conductor __instance)
        {
            if (!IsMusicEnabled)
            {
                StopMusic();
                Logger.Debug("Dynamic music is disabled. Skipping custom music playback.");
                return false;
            }

            var customTracks = LoadTracksPatch.GetTracks();
            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Debug("No custom music loaded, skipping Update.");
                return false;
            }

            if (OutputDevice == null)
            {
                OutputDevice = new WaveOutEventAdapter(new WaveOutEvent());
                Logger.Debug("Initialized WaveOutEvent output device.");
            }

            if (OutputDevice.PlaybackState == PlaybackState.Playing) return false;

            Logger.Debug("No music currently playing, starting next track.");

            PlayRandomTrack();

            return false;
        }

        public static void StopMusic()
        {
            _audioFileReader?.Dispose();

            OutputDevice?.Dispose();

            Logger.Debug("Stopped output device.");
        }

        public static void PlayRandomTrack()
        {
            var customTracks = LoadTracksPatch.GetTracks();

            do
            {
                _currentTrackIndex = Random.Next(customTracks.Length);
            } while (_currentTrackIndex == _previousTrackIndex && customTracks.Length > 1);

            Logger.Debug($"Selected track {_currentTrackIndex + 1} of {customTracks.Length}.");

            _audioFileReader = new AudioFileReaderAdapter(
                new AudioFileReader(customTracks[_currentTrackIndex])
            );

            UpdateVolume();

            OutputDevice.Init(_audioFileReader);
            OutputDevice.Play();
            Logger.Info($"Started playing {Path.GetFileName(customTracks[_currentTrackIndex])}.");

            _previousTrackIndex = _currentTrackIndex;
        }

        public static void UpdateVolume()
        {
            if (_audioFileReader == null)
            {
                Logger.Debug("No audio file loaded. Skipping volume update.");
                return;
            }

            var customTracks = LoadTracksPatch.GetTracks();
            if (customTracks == null || customTracks.Length == 0) return;

            var preCalculatedMaxDecibel = LoadTracksPatch.GetTrackMaxDecibel(customTracks[_currentTrackIndex]);
            Logger.Debug($"preCalculatedMaxDecibel {preCalculatedMaxDecibel}");

            var masterAudioMixer = new AudioMixerAdapter(GameManager.Instance.masterAudioMixer);
            if (VolumeAdjuster == null) VolumeAdjuster = Services.Get<IVolumeAdjuster>();

            var overallAudioVolumeLevel =
                Mathf.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
            VolumeAdjuster.Adjust(masterAudioMixer, _audioFileReader, overallAudioVolumeLevel, preCalculatedMaxDecibel);

            Logger.Debug("Volume successfully updated for the currently playing track.");
        }
    }
}