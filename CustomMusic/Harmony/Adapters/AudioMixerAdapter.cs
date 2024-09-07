using NAudio.Wave;
using UnityEngine.Audio;

namespace CustomMusic.Harmony.Adapters
{
    public interface IAudioMixerAdapter
    {
        bool GetFloat(string name, out float value);
    }

    public class AudioMixerAdapter : IAudioMixerAdapter
    {
        private readonly AudioMixer _audioMixer;

        public AudioMixerAdapter(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
        }

        public bool GetFloat(string name, out float value)
        {
            return _audioMixer.GetFloat(name, out value);
        }
    }
}