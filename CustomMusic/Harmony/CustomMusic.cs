using System;
using System.IO;
using NAudio.Wave;
using HarmonyLib;
using DynamicMusic;
using UnityEngine;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Update))]
    public static class CustomMusicPlayer
    {
        private static readonly ILogger Logger = new Logger();
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        private static int currentTrackIndex = 0;
        private static float lastVolumeSetting = -1f;

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

            // Update the volume according to the game's settings
            UpdateVolume();

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

        private static void UpdateVolume()
        {
            // Access the game's current music volume using the "dmsVol" parameter
            if (GameManager.Instance.masterAudioMixer.GetFloat("dmsVol", out float volume))
            {
                float currentVolumeSetting = Mathf.Pow(10, volume / 20);

                if (Math.Abs(currentVolumeSetting - lastVolumeSetting) > 0.01f)
                {
                    outputDevice.Volume = currentVolumeSetting;
                    lastVolumeSetting = currentVolumeSetting;
                    Logger.Info($"CustomMusicPlayer: Volume successfully updated to {currentVolumeSetting * 100}%.");
                }
                else
                {
                    Logger.Info("CustomMusicPlayer: Volume has not changed since the last update.");
                }
            }
            else
            {
                Logger.Error("CustomMusicPlayer: Failed to retrieve 'dmsVol' from masterAudioMixer.");
            }
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

        [HarmonyPatch(typeof(Conductor), nameof(Conductor.OnPauseGame))]
        public static class NoPauseOnGamePause
        {
            public static bool Prefix()
            {
                Logger.Info("NoPauseOnGamePause: Preventing music from pausing.");
                return false;
            }
        }

        [HarmonyPatch(typeof(Conductor), nameof(Conductor.OnUnPauseGame))]
        public static class NoPauseOnGameUnPause
        {
            public static bool Prefix()
            {
                Logger.Info("NoPauseOnGameUnPause: Preventing music from unpausing.");
                return false;
            }
        }
    }
}