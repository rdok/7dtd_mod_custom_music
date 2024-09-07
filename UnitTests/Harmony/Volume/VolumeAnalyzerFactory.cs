using System;
using System.Collections.Generic;
using System.Linq;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using Moq;
using UnityEngine;
using ILogger = CustomMusic.Harmony.ILogger;

namespace UnitTests.Harmony.Volume
{
    public static class VolumeAnalyzerFactory
    {
        private static readonly Dictionary<string, object> DefaultParameters = new()
        {
            { "SampleRate", 3 },
            { "NumberOfSamplesRead", new[] { 1f } },
            { "MaxAmplitude", 1f },
            { "ExpectedDecibels", 20 * Mathf.Log10(1f) }
        };

        public static (
            VolumeAnalyzer volumeAnalyzer,
            Mock<IAudioFileReaderAdapter> audioFileReaderMock,
            float expectedDecibels
            ) Create(Dictionary<string, object> parameters)
        {
            var loggerMock = new Mock<ILogger>();
            var audioFileReaderMock = new Mock<IAudioFileReaderAdapter>();
            var waveFormatAdapter = new Mock<IWaveFormatAdapter>();

            var combinedParameters = new Dictionary<string, object>(DefaultParameters);
            foreach (var param in parameters)
            {
                combinedParameters[param.Key] = param.Value;
            }

            var sampleRate = (int)combinedParameters["SampleRate"];
            var maxAmplitude = (float)combinedParameters["MaxAmplitude"];
            var numberOfSamplesRead = (float[])combinedParameters["NumberOfSamplesRead"];
            var expectedDecibels = (float)combinedParameters["ExpectedDecibels"];

            waveFormatAdapter.Setup(w => w.SampleRate).Returns(sampleRate);

            audioFileReaderMock.Setup(m => m.Read(It.IsAny<float[]>(), 0, It.IsAny<int>()))
                .Returns((float[] buffer, int offset, int count) =>
                {
                    if (numberOfSamplesRead.Length <= 0) return 0;
                    var length = Math.Min(numberOfSamplesRead.Length, count);
                    Array.Copy(numberOfSamplesRead, 0, buffer, 0, length);
                    numberOfSamplesRead = numberOfSamplesRead.Skip(length).ToArray();

                    return length;
                });

            audioFileReaderMock.Setup(m => m.WaveFormat).Returns(waveFormatAdapter.Object);

            var volumeAnalyzer = new VolumeAnalyzer(loggerMock.Object);

            return (volumeAnalyzer, audioFileReaderMock, expectedDecibels);
        }
    }
}