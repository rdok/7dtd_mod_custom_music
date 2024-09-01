namespace CustomMusic.Harmony
{
    public class Init : IModApi
    {
        private static readonly ILogger Logger = new Logger();

        public void InitMod(Mod modInstance)
        {
            var harmony = new HarmonyLib.Harmony("uk.co.rdok.7daystodie.mod.custom_music");
            harmony.PatchAll();
        }
    }
}