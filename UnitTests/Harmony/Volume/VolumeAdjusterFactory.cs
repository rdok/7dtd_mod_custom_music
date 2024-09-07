using System;
using System.Collections.Generic;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using Moq;
using ILogger = CustomMusic.Harmony.ILogger;

namespace UnitTests.Harmony.Volume
{
    public static class VolumeAdjusterFactory
    {
        private static readonly Dictionary<string, object> DefaultParameters = new()
        {
            { "DynamicMusicVolumeInDecibels", -6f },
            { "PreCalculatedMaxDecibel", 0f },
            { "VolumeRetrievalSuccess", true }
        };

        public static (VolumeAdjuster volumeAdjuster, Mock<IAudioMixerAdapter>, Mock<IAudioFileReaderAdapter>
            audioFileReaderMock, float dynamicMusicVolumeInDecibels, float preCalculatedMaxDecibel) Create(
                Dictionary<string, object> parameters = null)
        {
            var loggerMock = new Mock<ILogger>();
            var masterAudioMixerMock = new Mock<IAudioMixerAdapter>();
            var audioFileReaderMock = new Mock<IAudioFileReaderAdapter>();

            var combinedParameters = new Dictionary<string, object>(DefaultParameters);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    combinedParameters[param.Key] = param.Value;
                }
            }

            var dynamicMusicVolumeInDecibels = (float)combinedParameters["DynamicMusicVolumeInDecibels"];
            var preCalculatedMaxDecibel = (float)combinedParameters["PreCalculatedMaxDecibel"];
            var volumeRetrievalSuccess = (bool)combinedParameters["VolumeRetrievalSuccess"];

            masterAudioMixerMock.Setup(m => m.GetFloat("dmsVol", out dynamicMusicVolumeInDecibels))
                .Returns(volumeRetrievalSuccess);

            var volumeAdjuster = new VolumeAdjuster(loggerMock.Object);

            return (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock, dynamicMusicVolumeInDecibels,
                preCalculatedMaxDecibel);
        }
    }
}