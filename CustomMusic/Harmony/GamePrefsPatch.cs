using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomMusic.Harmony
{
    [HarmonyPatch(typeof(GamePrefs))]
    public static class GamePrefsPatch
    {
        [HarmonyPatch(nameof(GamePrefs.Save), new Type[] { })]
        [HarmonyPatch(nameof(GamePrefs.Save), new Type[] { typeof(string) })]
        [HarmonyPatch(nameof(GamePrefs.Save), new Type[] { typeof(string), typeof(List<EnumGamePrefs>) })]
        public static void Postfix()
        {
            Debug.Log("GamePrefsPatch: Postfix executed after Save.");

            var isEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled);

            MusicPlayer.IsCustomMusicEnabled = isEnabled;
            MusicPlayer.UpdateVolume();
            ;
        }
    }
}