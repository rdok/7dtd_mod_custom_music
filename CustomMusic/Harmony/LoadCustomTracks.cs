using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Init))]
    public static class LoadCustomTracks
    {
        private static readonly ILogger Logger = new Logger();
        private static string[] customTracks;

        public static bool Prefix()
        {
            Logger.Info("CustomMusicPlayerInit: Starting Init function override.");

            var modPath = Assembly.GetExecutingAssembly().Location;
            var modDirectory = Path.GetDirectoryName(modPath);
            var musicDirectory = Path.Combine(modDirectory, "Tracks");
            Logger.Info(
                "CustomMusicPlayerInit: Checking for music directory at " +
                $"{musicDirectory}.");

            string[] validExtensions = { ".mp3", ".wav", ".aiff", ".flac" };

            customTracks = Directory
                .GetFiles(musicDirectory, "*.*")
                .Where(file => validExtensions
                    .Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                ).ToArray();

            Logger.Info("CustomMusicPlayerInit: Found "
                        + $"{customTracks.Length} supported audio files in "
                        + $"{musicDirectory}.");

            if (customTracks.Length == 0)
            {
                Logger.Info(
                    $"CustomMusicPlayerInit: No supported audio files found in {musicDirectory}. " +
                    "Please add audio files to this directory.");
                return false;
            }

            Logger.Info("CustomMusicPlayerInit: Custom Music Player Initialized.");
            return false;
        }

        public static string[] GetCustomTracks()
        {
            return customTracks;
        }
    }
}