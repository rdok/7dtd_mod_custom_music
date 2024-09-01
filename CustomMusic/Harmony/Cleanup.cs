using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.CleanUp))]
    public static class Cleanup
    {
        private static readonly ILogger Logger = new Logger();

        public static void Prefix()
        {
            Logger.Info("CustomMusicPlayerCleanup: Cleaning up resources.");

            if (CustomMusicPlayer.audioFile != null)
            {
                CustomMusicPlayer.audioFile.Dispose();
                Logger.Info("CustomMusicPlayerCleanup: Disposed of AudioFileReader.");
            }
            
            if (CustomMusicPlayer.outputDevice == null) return;
            
            CustomMusicPlayer.outputDevice.Dispose();
            
            Logger.Info("CustomMusicPlayerCleanup: Disposed of WaveOutEvent.");
        }
    }
}