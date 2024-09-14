using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using DynamicMusic;
using HarmonyLib;
using MusicUtils.Enums;
using NAudio.Wave;

namespace CustomMusic.Harmony.Patch
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.Init))]
    public static class LoadTracksPatch
    {
        private static readonly ILogger Logger = new Logger();

        private static string[] _ambientTracks;
        private static string[] _combatTracks;

        private static readonly Dictionary<string, float> TrackMaxDecibels = new Dictionary<string, float>();

        public static bool Prefix()
        {
            Logger.Debug("LoadTracks: Starting Conductor.Init function override.");

            var modPath = Assembly.GetExecutingAssembly().Location;
            var modDirectory = Path.GetDirectoryName(modPath);
            if (modDirectory == null)
            {
                Logger.Error("LoadTracks: Could not determine mod directory.");
                return false;
            }

            string[] validExtensions = { ".mp3", ".wav", ".aiff", ".flac" };

            // Load Ambient Tracks
            var ambientDirectory = Path.Combine(modDirectory, "AmbientTracksDebug");
#if RELEASE
            musicDirectory = Path.Combine(modDirectory ?? throw missingModPathError, "AmbientTracks");
#endif
            
            Logger.Debug($"LoadTracks: Checking for ambient music directory at {ambientDirectory}.");
            _ambientTracks = Directory.GetFiles(ambientDirectory, "*.*")
                .Where(file => validExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            Logger.Debug(
                $"LoadTracks: Found {_ambientTracks.Length} supported ambient audio files in {ambientDirectory}.");

            // If no ambient tracks are found, throw an error
            if (_ambientTracks.Length == 0)
            {
                Logger.Error(
                    $"LoadTracks: No supported ambient audio files found in {ambientDirectory}. Please add audio files to this directory.");
                throw new FileNotFoundException($"No ambient tracks found in {ambientDirectory}.");
            }

            // Load Combat Tracks
            var combatDirectory = Path.Combine(modDirectory, "CombatTracksDebug");
#if RELEASE
            musicDirectory = Path.Combine(modDirectory ?? throw missingModPathError, "CombatTracks");
#endif
            Logger.Debug($"LoadTracks: Checking for combat music directory at {combatDirectory}.");
            _combatTracks = Directory.GetFiles(combatDirectory, "*.*")
                .Where(file => validExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            Logger.Debug(
                $"LoadTracks: Found {_combatTracks.Length} supported combat audio files in {combatDirectory}.");

            Logger.Debug("LoadTracks: Calculating max decibels for all tracks asynchronously.");
            Task.Run(CalculateMaxDecibelsForAllTracks);

            Logger.Debug("LoadTracks: Custom Music Player Initialized.");
            return false;
        }

        private static async Task CalculateMaxDecibelsForAllTracks()
        {
            var allTracks = _ambientTracks.Concat(_combatTracks).ToArray();
            var tasks = allTracks.Select(track => Task.Run(() => CalculateAndStoreMaxDecibel(track))).ToArray();
            await Task.WhenAll(tasks);
        }

        private static void CalculateAndStoreMaxDecibel(string trackPath)
        {
            if (TrackMaxDecibels.ContainsKey(trackPath)) return;

            var peakVolumeAnalyzer = Services.Get<IVolumeAnalyzer>();
            using (var audioFileReader = new AudioFileReaderAdapter(new AudioFileReader(trackPath)))
            {
                var maxDecibel = peakVolumeAnalyzer.FindMaxDecibel(audioFileReader);
                TrackMaxDecibels[trackPath] = maxDecibel;

                Logger.Debug($"Calculated max decibel for track {Path.GetFileName(trackPath)}: {maxDecibel} dB");
            }
        }

        public static string[] GetTracksForSection(SectionType sectionType)
        {
            switch (sectionType)
            {
                case SectionType.Combat:
                    return _combatTracks;
                default:
                    return _ambientTracks; // Default to ambient tracks if section type is unknown
            }
        }

        public static float GetTrackMaxDecibel(string trackPath)
        {
            if (TrackMaxDecibels.TryGetValue(trackPath, out var decibel)) return decibel;

            CalculateAndStoreMaxDecibel(trackPath);

            return TrackMaxDecibels[trackPath];
        }
    }
}