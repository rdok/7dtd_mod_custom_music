using System;
using System.IO;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using DynamicMusic;
using HarmonyLib;
using NAudio.Wave;

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
        private static readonly IVolumeAdjuster VolumeAdjuster = Services.Get<IVolumeAdjuster>();

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

            // Use the pre-calculated max decibel value for this track
            var preCalculatedMaxDecibel = LoadTracksPatch.GetTrackMaxDecibel(customTracks[_currentTrackIndex]);

            // Adjust volume based on pre-calculated max decibel
            VolumeAdjuster.Adjust(_audioFileReader, preCalculatedMaxDecibel);

            OutputDevice.Init(_audioFileReader);
            OutputDevice.Play();
            Logger.Info($"Started playing {Path.GetFileName(customTracks[_currentTrackIndex])}.");

            _previousTrackIndex = _currentTrackIndex;
        }

        public static void UpdateVolume()
        {
            if (_audioFileReader == null)
            {
                Logger.Error("No audio file loaded. Skipping volume update.");
                return;
            }

            var customTracks = LoadTracksPatch.GetTracks();
            if (customTracks == null || customTracks.Length == 0) return;

            // Reuse the pre-calculated max decibel value
            var preCalculatedMaxDecibel = LoadTracksPatch.GetTrackMaxDecibel(customTracks[_currentTrackIndex]);

            // Reuse the VolumeAdjuster to update the volume dynamically
            VolumeAdjuster.Adjust(_audioFileReader, preCalculatedMaxDecibel);
            Logger.Info("Volume successfully updated for the currently playing track.");
        }
    }
}