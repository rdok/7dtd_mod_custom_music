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
        public static WaveOutEvent outputDevice;
        public static AudioFileReader audioFile;
        private static int currentTrackIndex = -1;
        private static int previousTrackIndex = -1;
        private static float lastVolumeSetting = -1f;
        private static float customMusicVolume = 1.0f;  // Base volume control for custom music
        private static readonly System.Random random = new System.Random();

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
                PlayRandomTrack(customTracks);
            }

            UpdateVolume();
            Logger.Info("CustomMusicPlayer: Completed Update function override.");
            return false;
        }

        private static void PlayRandomTrack(string[] customTracks)
        {
            do
            {
                currentTrackIndex = random.Next(customTracks.Length);
            } while (currentTrackIndex == previousTrackIndex && customTracks.Length > 1);

            Logger.Info($"CustomMusicPlayer: Selected track {currentTrackIndex + 1} of {customTracks.Length}.");

            if (audioFile != null)
            {
                audioFile.Dispose();
                Logger.Info("CustomMusicPlayer: Disposed of previous AudioFileReader.");
            }

            audioFile = new AudioFileReader(customTracks[currentTrackIndex])
            {
                Volume = customMusicVolume // Set initial custom music volume here
            };
            outputDevice.Init(audioFile);
            outputDevice.Play();
            Logger.Info($"CustomMusicPlayer: Started playing {Path.GetFileName(customTracks[currentTrackIndex])}.");

            previousTrackIndex = currentTrackIndex;
        }

        private static void UpdateVolume()
        {
            if (!GameManager.Instance.masterAudioMixer.GetFloat("dmsVol", out float masterVolume))
            {
                Logger.Error("CustomMusicPlayer: Failed to retrieve 'dmsVol' from masterAudioMixer.");
                return;
            }

            float gameVolume = Mathf.Pow(10, masterVolume / 20);  // Convert dB to linear volume
            float adjustedVolume = customMusicVolume * gameVolume;  // Adjust custom music volume by game's master volume

            if (Math.Abs(adjustedVolume - lastVolumeSetting) <= 0.01f)
            {
                Logger.Info("CustomMusicPlayer: Volume has not changed since the last update.");
                return;
            }

            if (audioFile != null)
            {
                audioFile.Volume = adjustedVolume;  // Apply adjusted volume to the specific audio file
            }

            lastVolumeSetting = adjustedVolume;
            Logger.Info($"CustomMusicPlayer: Volume successfully updated to {adjustedVolume * 100}%.");
        }

        public static void SetCustomMusicVolume(float volume)
        {
            customMusicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
            if (audioFile != null)
            {
                float masterVolume = Mathf.Pow(10, lastVolumeSetting / 20);  // Assume lastVolumeSetting is the last known game volume in dB
                audioFile.Volume = customMusicVolume * masterVolume;
                Logger.Info($"CustomMusicPlayer: Custom music volume updated to {customMusicVolume * 100}%.");
            }
        }
    }
}
