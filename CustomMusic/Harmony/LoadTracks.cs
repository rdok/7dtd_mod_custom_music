using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Init))]
    public static class LoadTracks
    {
        private static readonly ILogger Logger = new Logger();
        private static string[] _tracks;

        public static bool Prefix()
        {
            Logger.Debug("CustomMusicPlayerInit: Starting Init function override.");

            var modPath = Assembly.GetExecutingAssembly().Location;
            var modDirectory = Path.GetDirectoryName(modPath);
            var missingModPathError = new InvalidOperationException(
                $"Missing tracks directory: {modPath}"
            );
            var musicDirectory = Path
                .Combine(modDirectory ?? throw missingModPathError, "AmbientTracks");

            Logger.Debug(
                "CustomMusicPlayerInit: Checking for music directory at " +
                $"{musicDirectory}.");

            string[] validExtensions = { ".mp3", ".wav", ".aiff", ".flac" };

            _tracks = Directory
                .GetFiles(musicDirectory, "*.*")
                .Where(file => validExtensions
                    .Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                ).ToArray();

            Logger.Debug("CustomMusicPlayerInit: Found "
                        + $"{_tracks.Length} supported audio files in "
                        + $"{musicDirectory}.");

            if (_tracks.Length == 0)
            {
                Logger.Debug(
                    $"CustomMusicPlayerInit: No supported audio files found in {musicDirectory}. " +
                    "Please add audio files to this directory.");
                return false;
            }

            Logger.Debug("CustomMusicPlayerInit: Custom Music Player Initialized.");
            return false;
        }

        public static string[] GetCustomTracks()
        {
            return _tracks;
        }
    }
}