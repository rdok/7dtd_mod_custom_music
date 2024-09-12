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
        public void adjust_volume_based_on_decibel_difference()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock) = VolumeAdjusterFactory.Create(
                new Dictionary<string, object> { { "DynamicMusicVolumeInDecibels", .7f }, });

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, 1f, .3f);

            audioFileReaderMock.VerifySet(a => a.Volume = Mathf.Pow(10, (.7f - .3f) / 20), Times.Once());
        }

        [Test]
        public void skip_adjustment_when_precalculated_decibel_is_negative_infinity()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock) = VolumeAdjusterFactory.Create();

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, 1f, float.NegativeInfinity);

            audioFileReaderMock.VerifySet(a => a.Volume = It.IsAny<float>(), Times.Never());
        }

        [Test]
        public void do_not_adjust_volume_when_dynamic_music_volume_cannot_be_retrieved()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock) = VolumeAdjusterFactory.Create(
                new Dictionary<string, object> { { "VolumeRetrievalSuccess", false } });

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, 1f, 0f);

            audioFileReaderMock.VerifySet(a => a.Volume = It.IsAny<float>(), Times.Never());
        }

        [Test]
        public void adjust_volume_based_on_overall_sound_volume()
        {
            var (volumeAdjuster, masterAudioMixerMock, audioFileReaderMock) = VolumeAdjusterFactory.Create(
                new Dictionary<string, object> { { "DynamicMusicVolumeInDecibels", 0f } });

            volumeAdjuster.Adjust(masterAudioMixerMock.Object, audioFileReaderMock.Object, .5f, 0);

            audioFileReaderMock.VerifySet(a => a.Volume = .5f, Times.Once());
        }
    }
}