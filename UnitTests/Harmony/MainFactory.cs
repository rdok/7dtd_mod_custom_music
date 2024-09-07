using System;
using CustomMusic.Harmony;
using CustomMusic.Harmony.Adapters;
using Moq;
using NAudio.Wave;

namespace UnitTests.Harmony
{
    public static class MainFactory
    {
        public static (Main main, Mock<IHarmony> harmonyMock, Mod) Create()
        {
            var harmonyMock = new Mock<IHarmony>();
            var main = new Main();
            var mod = new Mod();
            Services.Add(_ => harmonyMock.Object);

            return (main, harmonyMock, mod);
        }
    }
}