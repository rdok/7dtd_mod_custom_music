using CustomMusic.Harmony.Adapters;

namespace CustomMusic.Harmony
{
    public class Main : IModApi
    {
        public Main()
        {
            Services.Initialise();
        }

        public void InitMod(Mod modInstance)
        {
            var harmony = Services.Resolve<IHarmony>();
            harmony.Init("uk.co.rdok.7daystodie.mod.custom_music");
            harmony.PatchAll();
        }
    }
}