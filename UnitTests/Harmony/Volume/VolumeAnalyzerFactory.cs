using System;
using System.Collections.Generic;
using System.Linq;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;
using Moq;
using NAudio.Wave;
using UnityEngine;
using ILogger = CustomMusic.Harmony.ILogger;

namespace UnitTests.Harmony.Volume
{
    public static class VolumeAnalyzerFactory
    {
        private static readonly Dictionary<string, object> DefaultParameters = new()
        {
            { "NumberOfSamplesRead", new float[][] { new[] { 1f } } }, // Default single chunk
            { "SampleRate", 44100 } // Default sample rate
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

            var numberOfSamplesRead = (float[][])combinedParameters["NumberOfSamplesRead"];
            var sampleRate = (int)combinedParameters["SampleRate"];
            var chunkIndex = 0;

            // Mock the WaveFormat.SampleRate to match the requested sample rate
            waveFormatAdapter.Setup(w => w.SampleRate).Returns(sampleRate);
            audioFileReaderMock.Setup(m => m.WaveFormat).Returns(waveFormatAdapter.Object);

            // Mock the Read method to simulate passing the sample buffer
            audioFileReaderMock.Setup(m => m.Read(It.IsAny<float[]>(), 0, It.IsAny<int>()))
                .Returns((float[] buffer, int offset, int count) =>
                {
                    if (chunkIndex >= numberOfSamplesRead.Length)
                        return 0; // No more chunks to process

                    var chunk = numberOfSamplesRead[chunkIndex];
                    chunkIndex++;

                    // Copy the entire chunk into the buffer, up to the buffer's size
                    var length = Math.Min(chunk.Length, buffer.Length);
                    Array.Copy(chunk, 0, buffer, offset, length);

                    return length; // Return the number of samples read (the chunk size)
                });

            var volumeAnalyzer = new VolumeAnalyzer(loggerMock.Object);

            return (volumeAnalyzer, audioFileReaderMock);
        }
    }
}
