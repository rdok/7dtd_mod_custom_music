using System;

namespace CustomMusic.Harmony.Adapters
{
    public interface IHarmonyAdapter
    {
        void PatchAll();
        void Init(string id);
    }

    public class HarmonyAdapter : IHarmonyAdapter
    {
        private HarmonyLib.Harmony _harmony;

        public void PatchAll()
        {
            _harmony.PatchAll();
        }

        public void Init(string id)
        {
            _harmony = new HarmonyLib.Harmony(id);
        }
    }
}