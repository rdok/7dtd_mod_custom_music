using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace UnitTests.Harmony.Volume
{
    [TestFixture]
    public class VolumeAnalyzerTests
    {
        [Test]
        public void it_handles_tracks_with_no_valid_samples()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "NumberOfSamplesRead", new[] { 0f } }
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(float.NegativeInfinity, output);
        }


        [Test]
        public void it_finds_the_largest_amplitude_from_two_samples()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "NumberOfSamplesRead", new[] { 2f, 3f } }
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(3f), output);
        }

        [Test]
        public void it_handles_negative_samples()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "NumberOfSamplesRead", new[] { -3f, -2f } }
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(3f), output);
        }

        [Test]
        public void it_finds_the_largest_amplitude_with_zeros_and_non_zeros()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "NumberOfSamplesRead", new[] { 0f, 5f, 0f } }
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(5f), output);
        }

        [Test]
        public void it_handles_identical_samples()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "NumberOfSamplesRead", new[] { 4f, 4f, 4f } }
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(4f), output);
        }

        [Test]
        public void it_finds_the_largest_amplitude_across_multiple_chunks()
        {
            var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(new Dictionary<string, object>
            {
                { "NumberOfSamplesRead", new[] { new[] { 10f, 3f }, new[] { 5f, 1f } } }
            });

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(10f), output);
        }
    }
}