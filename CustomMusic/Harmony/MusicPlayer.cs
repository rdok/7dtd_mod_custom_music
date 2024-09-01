using System;
using System.IO;
using NAudio.Wave;
using HarmonyLib;
using DynamicMusic;
using UnityEngine;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Update))]
    public static class MusicPlayer
    {
        public static bool IsCustomMusicEnabled { get; set; } = true;

        private static readonly ILogger Logger = new Logger();
        public static WaveOutEvent outputDevice;
        public static AudioFileReader audioFile;
        private static int currentTrackIndex = -1;
        private static int previousTrackIndex = -1;
        private static float lastVolumeSetting = -1f;
        private static float customMusicVolume = 1.0f; // Base volume control for custom music
        private static readonly System.Random random = new System.Random();

        public static bool Prefix(Conductor __instance)
        {
            if (!IsCustomMusicEnabled)
            {
                StopMusic();
                Logger.Info("CustomMusicPlayer: Custom music is disabled. Skipping custom music playback.");
                return false; // Allow the original Update method to proceed
            }

            var customTracks = LoadTracks.GetCustomTracks();
            if (customTracks == null || customTracks.Length == 0)
            {
                Logger.Info("CustomMusicPlayer: No custom music loaded, skipping Update.");
                return false; // Allow the original Update method to proceed if no tracks are available
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

            return false; // Skip the original Update method since we've handled music playback
        }

        private static void StopMusic()
        {
            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
                Logger.Info("CustomMusicPlayer: Stopped and disposed of custom music.");
            }

            if (outputDevice == null) return;

            outputDevice.Stop();
            Logger.Info("CustomMusicPlayer: Stopped output device.");
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

            audioFile = new AudioFileReader(customTracks[currentTrackIndex]);
            outputDevice.Init(audioFile);
            UpdateVolume(); // Adjust volume immediately based on game master volume
            outputDevice.Play();
            Logger.Info($"CustomMusicPlayer: Started playing {Path.GetFileName(customTracks[currentTrackIndex])}.");

            previousTrackIndex = currentTrackIndex;
        }

        public static void UpdateVolume()
        {
            if (!GameManager.Instance.masterAudioMixer.GetFloat("dmsVol", out var masterVolume))
            {
                Logger.Error("CustomMusicPlayer: Failed to retrieve 'dmsVol' from masterAudioMixer.");
                return;
            }

            var linearVolumeFromDecibel = Mathf.Pow(10, masterVolume / 20);
            var adjustedVolumeToGameMasterVolume = customMusicVolume * linearVolumeFromDecibel;

            if (Math.Abs(adjustedVolumeToGameMasterVolume - lastVolumeSetting) <= 0.01f)
            {
                return;
            }

            if (audioFile != null)
            {
                audioFile.Volume = adjustedVolumeToGameMasterVolume; 
            }

            lastVolumeSetting = adjustedVolumeToGameMasterVolume;
            Logger.Info($"CustomMusicPlayer: Volume successfully updated to {adjustedVolumeToGameMasterVolume * 100}%.");
        }
    }
}
