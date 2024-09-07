using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony.Patch
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.CleanUp))]
    public static class CleanupPatch
    {
        private static readonly ILogger Logger = new Logger();

        public static void Prefix()
        {
            Logger.Debug("CustomMusicPlayerCleanup: Cleaning up resources.");

            MusicPlayerPatch.StopMusic();

            Logger.Debug("CustomMusicPlayerCleanup: Disposed of WaveOutEvent.");
        }
    }
}