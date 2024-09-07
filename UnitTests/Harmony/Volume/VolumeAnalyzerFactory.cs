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
            { "SampleRate", 3 }, // Default buffer size
            { "NumberOfSamplesRead", new[] { 1f } },
            { "MaxAmplitude", 1f }
        };

        public static (
            VolumeAnalyzer volumeAnalyzer,
            Mock<IAudioFileReaderAdapter> audioFileReaderMock
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

            var sampleRate = (int)combinedParameters["SampleRate"]; // Buffer size is based on sample rate
            var numberOfSamplesRead = (float[])combinedParameters["NumberOfSamplesRead"];

            waveFormatAdapter.Setup(w => w.SampleRate).Returns(sampleRate);

            audioFileReaderMock.Setup(m => m.Read(It.IsAny<float[]>(), 0, It.IsAny<int>()))
                .Returns((float[] buffer, int offset, int count) =>
                {
                    var length = Math.Min(numberOfSamplesRead.Length, sampleRate); // Use sampleRate as buffer size
                    if (length > 0)
                    {
                        Array.Copy(numberOfSamplesRead, 0, buffer, 0, length); // Copy to buffer based on sample rate
                        numberOfSamplesRead = numberOfSamplesRead.Skip(length).ToArray();
                    }
                    return length;
                });

            audioFileReaderMock.Setup(m => m.WaveFormat).Returns(waveFormatAdapter.Object);

            var volumeAnalyzer = new VolumeAnalyzer(loggerMock.Object);

            return (volumeAnalyzer, audioFileReaderMock);
        }
    }
}
