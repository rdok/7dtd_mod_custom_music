using DynamicMusic;
using HarmonyLib;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(Conductor), nameof(Conductor.OnPauseGame))]
    public static class NoPauseOnGamePause
    {
        private static readonly ILogger Logger = new Logger();

        public static bool Prefix()
        {
            Logger.Debug("NoPauseOnGamePause: Preventing music from pausing.");
            return false;
        }
    }

    [HarmonyPatch(typeof(Conductor), nameof(Conductor.OnUnPauseGame))]
    public static class NoPauseOnGameUnPause
    {
        private static readonly ILogger Logger = new Logger();

        public static bool Prefix()
        {
            Logger.Debug("NoPauseOnGameUnPause: Preventing music from unpausing.");
            return false;
        }
    }
}