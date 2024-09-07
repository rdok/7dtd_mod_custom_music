using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using DynamicMusic;
using HarmonyLib;
using NAudio.Wave;

namespace CustomMusic.Harmony.Patch
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Init))]
    public static class LoadTracksPatch
    {
        private static readonly ILogger Logger = new Logger();
        private static string[] _tracks;
        private static readonly Dictionary<string, float> TrackMaxDecibels = new Dictionary<string, float>();

        public static bool Prefix()
        {
            Logger.Debug("LoadTracks: Starting Conductor.Init function override.");

            var modPath = Assembly.GetExecutingAssembly().Location;
            var modDirectory = Path.GetDirectoryName(modPath);
            var missingModPathError = new InvalidOperationException($"Missing tracks directory: {modPath}");
            var musicDirectory = Path.Combine(modDirectory ?? throw missingModPathError, "DebugAmbientTracks");
             
#if RELEASE
            var musicDirectory = Path.Combine(modDirectory ?? throw missingModPathError, "AmbientTracks");
#endif
            Logger.Debug($"LoadTracks: Checking for music directory at {musicDirectory}. ");

            string[] validExtensions = { ".mp3", ".wav", ".aiff", ".flac" };

            _tracks = Directory.GetFiles(musicDirectory, "*.*")
                .Where(file => validExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            Logger.Debug($"LoadTracks: Found {_tracks.Length} supported audio files in {musicDirectory}.");

            if (_tracks.Length == 0)
            {
                Logger.Debug($"LoadTracks: No supported audio files found in {musicDirectory}. " +
                             "Please add audio files to this directory.");
                return false;
            }

            Logger.Debug("LoadTracks: Custom Music Player Initialized.");
            return false;
        }

        public static string[] GetTracks()
        {
            return _tracks;
        }

        public static float GetTrackMaxDecibel(string trackPath)
        {
            if (TrackMaxDecibels.TryGetValue(trackPath, out var decibel)) return decibel;

            var peakVolumeAnalyzer = Services.Get<IVolumeAnalyzer>();
            using (var audioFileReader = new AudioFileReaderAdapter(new AudioFileReader(trackPath)))
            {
                var maxDecibel = peakVolumeAnalyzer.FindMaxDecibel(audioFileReader);
                TrackMaxDecibels[trackPath] = maxDecibel;

                Logger.Debug(
                    $"Calculated max decibel for track {Path.GetFileName(trackPath)} on demand: {maxDecibel} dB");
            }

            return TrackMaxDecibels[trackPath];
        }
    }
}