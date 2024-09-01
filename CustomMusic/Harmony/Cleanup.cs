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
            Logger.Debug("CustomMusicPlayerCleanup: Cleaning up resources.");

            if (MusicPlayer.audioFile != null)
            {
                MusicPlayer.audioFile.Dispose();
                Logger.Debug("CustomMusicPlayerCleanup: Disposed of AudioFileReader.");
            }
            
            if (MusicPlayer.outputDevice == null) return;
            
            MusicPlayer.outputDevice.Dispose();
            
            Logger.Debug("CustomMusicPlayerCleanup: Disposed of WaveOutEvent.");
        }
    }
}