using NAudio.Wave;
using System.IO;
using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Update))]
    public static class MusicPlayer
    {
        private static readonly ILogger Logger = new Logger();
        public static WaveOutEvent OutputDevice;
        private static int _currentTrackIndex = -1;
        private static int _previousTrackIndex = -1;
        private static readonly System.Random Random = new System.Random();
        private static readonly VolumeAdjuster VolumeAdjuster = new VolumeAdjuster();
        public static bool IsMusicEnabled { get; set; } = true;
        private static AudioFileReader _audioFileReader;

        public static bool Prefix(Conductor __instance)
        {
            if (!IsMusicEnabled)
            {
                StopMusic();
                Logger.Debug("Dynamic music is disabled. Skipping custom music playback.");
                return false;
            }

            var customTracks = LoadTracks.GetCustomTracks();
            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Debug("No custom music loaded, skipping Update.");
                return false;
            }

            if (OutputDevice == null)
            {
                OutputDevice = new WaveOutEvent();
                Logger.Debug("Initialized WaveOutEvent output device.");
            }

            if (OutputDevice.PlaybackState == PlaybackState.Playing) return false;

            Logger.Debug("No music currently playing, starting next track.");

            PlayRandomTrack(customTracks);

            return false;
        }

        private static void StopMusic()
        {
            if (OutputDevice == null) return;

            OutputDevice.Stop();

            Logger.Debug("Stopped output device.");
        }

        private static void PlayRandomTrack(string[] customTracks)
        {
            do
            {
                _currentTrackIndex = Random.Next(customTracks.Length);
            } while (_currentTrackIndex == _previousTrackIndex && customTracks.Length > 1);

            Logger.Debug($"Selected track {_currentTrackIndex + 1} of {customTracks.Length}.");

            _audioFileReader = new AudioFileReader(customTracks[_currentTrackIndex]);
            VolumeAdjuster.Adjust(_audioFileReader);
            OutputDevice.Init(_audioFileReader);
            OutputDevice.Play();
            Logger.Info($"Started playing {Path.GetFileName(customTracks[_currentTrackIndex])}.");

            _previousTrackIndex = _currentTrackIndex;
        }
    }
}