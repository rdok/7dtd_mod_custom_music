using Moq;
using NUnit.Framework;

namespace UnitTests.Harmony
{
    [TestFixture]
    public class MainTests
    {
        [Test]
        public void it_initializes_harmony()
        {
            var (main, harmonyMock, mod) = MainFactory.Create();

            main.InitMod(mod);

            harmonyMock.Verify(
                x => x.Init("uk.co.rdok.7daystodie.mod.custom_music"),
                Times.Once
            );
        }
    }
}