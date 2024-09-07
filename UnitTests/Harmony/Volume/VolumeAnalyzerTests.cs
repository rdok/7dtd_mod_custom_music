using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace UnitTests.Harmony.Volume
{
    [TestFixture]
    public class VolumeAnalyzerTests
    {
        // [Test]
        // public void it_handles_tracks_with_no_valid_samples()
        // {
        //     var (volumeAnalyzer, audioFileReaderMock) = VolumeAnalyzerFactory.Create(new Dictionary<string, object>
        //     {
        //         { "ReadReturns", new List<int> { 0 } }
        //     });
        //
        //     var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);
        //
        //     Assert.AreEqual(output, float.NegativeInfinity);
        // }
        //
        [Test]
        public void it_handles_tracks_with_a_single_sample()
        {
            var (volumeAnalyzer, audioFileReaderMock, _) = VolumeAnalyzerFactory.Create(
                new Dictionary<string, object>
                {
                    { "SampleRate", 5 },
                    { "NumberOfSamplesRead", new[] { 2f } },
                }
            );

            var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);

            Assert.AreEqual(20 * Mathf.Log10(2f), output);
        }

        // [Test]
        // public void it_handles_tracks_with_multiple_reads()
        // {
        //     var (volumeAnalyzer, audioFileReaderMock, expectedDecibels) = VolumeAnalyzerFactory.Create(
        //         new Dictionary<string, object>
        //         {
        //             { "HasMultipleReadValues", true }
        //         }
        //     );
        //
        //     var output = volumeAnalyzer.FindMaxDecibel(audioFileReaderMock.Object);
        //
        //     Assert.AreEqual(expectedDecibels, output);
        // }
    }
}