using System.IO;
using DynamicMusic;
using HarmonyLib;
using NAudio.Wave;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Update))]
    public static class CustomMusicPlayer
    {
        private static readonly ILogger Logger = new Logger();
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        private static int currentTrackIndex = 0;

        public static bool Prefix(Conductor __instance)
        {
            Logger.Info("CustomMusicPlayer: Starting Update function override.");

            var customTracks = CustomMusicPlayerInit.GetCustomTracks();

            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Info("CustomMusicPlayer: No custom music loaded, skipping Update.");
                return false;
            }

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                Logger.Info("CustomMusicPlayer: Initialized WaveOutEvent output device.");
            }

            if (outputDevice.PlaybackState != PlaybackState.Playing)
            {
                Logger.Info("CustomMusicPlayer: No music currently playing, starting next track.");
                PlayNextTrack(customTracks);
            }

            Logger.Info("CustomMusicPlayer: Completed Update function override.");
            return false;
        }

        private static void PlayNextTrack(string[] customTracks)
        {
            Logger.Info($"CustomMusicPlayer: Playing track {currentTrackIndex + 1} of {customTracks.Length}.");

            if (audioFile != null)
            {
                audioFile.Dispose();
                Logger.Info("CustomMusicPlayer: Disposed of previous AudioFileReader.");
            }

            audioFile = new AudioFileReader(customTracks[currentTrackIndex]);
            outputDevice.Init(audioFile);
            outputDevice.Play();
            Logger.Info($"CustomMusicPlayer: Started playing {Path.GetFileName(customTracks[currentTrackIndex])}.");

            currentTrackIndex = (currentTrackIndex + 1) % customTracks.Length;
            Logger.Info($"CustomMusicPlayer: Next track index set to {currentTrackIndex}.");
        }

        public static void Cleanup()
        {
            Logger.Info("CustomMusicPlayer: Cleaning up resources.");

            if (audioFile != null)
            {
                audioFile.Dispose();
                Logger.Info("CustomMusicPlayer: Disposed of AudioFileReader.");
            }

            if (outputDevice != null)
            {
                outputDevice.Dispose();
                Logger.Info("CustomMusicPlayer: Disposed of WaveOutEvent.");
            }
        }
    }
}