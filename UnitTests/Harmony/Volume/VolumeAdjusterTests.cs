using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace UnitTests.Harmony.Volume
{
    [TestFixture]
    public class VolumeAdjusterTests
    {
        [Test]
        public void it_adjusts_volume_based_on_decibel_difference()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock, dynamicMusicVolumeInDecibels,
                preCalculatedMaxDecibel) = VolumeAdjusterFactory.Create();

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, preCalculatedMaxDecibel);

            audioFileReaderMock.VerifySet(
                a => a.Volume = Mathf.Pow(10, (dynamicMusicVolumeInDecibels - preCalculatedMaxDecibel) / 20),
                Times.Once()
            );
        }

        [Test]
        public void it_skips_adjustment_when_precalculated_decibel_is_negative_infinity()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock, _, _) = VolumeAdjusterFactory.Create();

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, float.NegativeInfinity);

            audioFileReaderMock.VerifySet(a => a.Volume = It.IsAny<float>(), Times.Never());
        }

        [Test]
        public void it_does_not_adjust_volume_when_dynamic_music_volume_cannot_be_retrieved()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock, _, _) = VolumeAdjusterFactory.Create(
                new Dictionary<string, object>
                {
                    { "VolumeRetrievalSuccess", false }
                }
            );

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, 13f);

            audioFileReaderMock.VerifySet(a => a.Volume = It.IsAny<float>(), Times.Never());
        }
    }
}