using System;
using HarmonyLib;

namespace CustomMusic.Harmony.Patch
{
    [HarmonyPatch(typeof(GamePrefs))]
    public static class GamePrefsPatch
    {
        private static readonly ILogger Logger = new Logger();

        [HarmonyPatch(nameof(GamePrefs.Save), new Type[] { })]
        public static void Postfix(GamePrefs __instance)
        {
            Logger.Debug($"GamePrefsPatch: Postfix executed after Save: {__instance.GetType().Name}");

            var isEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled);

            MusicPlayerPatch.IsMusicEnabled = isEnabled;

            MusicPlayerPatch.UpdateVolume();
        }
    }
}