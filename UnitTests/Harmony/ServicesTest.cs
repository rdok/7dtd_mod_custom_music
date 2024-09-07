using CustomMusic.Harmony;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using NUnit.Framework;

namespace UnitTests.Harmony
{
    [TestFixture]
    public class ServicesTest
    {
        [Test]
        public void it_adds_the_harmony_adapter()
        {
            Services.Initialise();
            var harmonyAdapter = Services.Get<IHarmonyAdapter>();
            Assert.AreEqual(typeof(HarmonyAdapter), harmonyAdapter.GetType());
        }

        [Test]
        public void it_adds_the_volume_analyzer()
        {
            Services.Initialise();
            var volumeAnalyzer = Services.Get<IVolumeAnalyzer>();
            Assert.AreEqual(typeof(VolumeAnalyzer), volumeAnalyzer.GetType());
        }
    }
}