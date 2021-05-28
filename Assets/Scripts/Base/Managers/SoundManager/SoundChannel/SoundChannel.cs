
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

namespace Base
{
    public sealed class SoundChannel : MonoBehaviour, ISoundChannel
    {
        [SerializeField] string channelID;
        public string Name => channelID;

        public float MixerSilent = -80.0f;
        public float MixerNormal = 0.0f;

        [Inject] ISoundSourceFactory soundSourceFactory;
        private bool channelEnabled = true;
        AudioMixerGroup mixerGroup;
        List<SoundSource> soundSources = new List<SoundSource>();
        bool volumeChanged = false;

        public bool Enabled { get {
                return channelEnabled;
            } set {
                if (channelEnabled != value) {
                    channelEnabled = value;
                    volumeChanged = true;
                    PlayerPrefs.SetInt($"{Name}Enabled", value ? 1 : 0);
                    PlayerPrefs.Save();
                }
            } }

        private float volume = 1.0f;
        public float Volume { get {
                return volume;
            } set {
                if (!Mathf.Approximately(volume, value)) {
                    volume = value;
                    volumeChanged = true;
                    PlayerPrefs.SetFloat($"{Name}Volume", value);
                    PlayerPrefs.Save();
                }
            } }

        public SoundHandle Play(AudioClip clip, bool loop, bool surviveSceneLoad, float volume)
        {
            return PlayAt((Transform)null, clip, loop, surviveSceneLoad, volume);
        }

        public SoundHandle PlayAt(GameObject gameObject, AudioClip clip, bool loop, bool surviveSceneLoad, float volume)
        {
            Transform transform = (gameObject != null ? gameObject.transform : null);
            return PlayAt(transform, clip, loop, surviveSceneLoad, volume);
        }

        public SoundHandle PlayAt(Component component, AudioClip clip, bool loop, bool surviveSceneLoad, float volume)
        {
            Transform transform = (component != null ? component.transform : null);
            return PlayAt(transform, clip, loop, surviveSceneLoad, volume);
        }

        public SoundHandle PlayAt(Transform transform, AudioClip clip, bool loop, bool surviveSceneLoad, float volume)
        {
            var source = soundSourceFactory.Spawn(clip);
            source.AudioSource.volume = volume;
            source.AudioSource.loop = loop;
            source.AudioSource.outputAudioMixerGroup = mixerGroup;
            source.AudioSource.spatialBlend = (transform == null ? 0.0f : 1.0f);
            source.AudioSource.ignoreListenerPause = surviveSceneLoad;
            source.SurviveSceneLoad = surviveSceneLoad;
            source.TargetTransform = transform;
            soundSources.Add(source);

            if (clip != null && (channelEnabled || loop)) {
                source.AudioSource.clip = clip;
                source.AudioSource.Play();
            }

            return new SoundHandle(this, source);
        }

        public void Stop(SoundHandle handle)
        {
            if (!handle.IsValid) {
                DebugOnly.Error("Attempted to stop sound with an invalid SoundHandle.");
                return;
            }

            int n = soundSources.Count;
            while (n-- > 0) {
                if (soundSources[n] == handle.Source) {
                    soundSources.RemoveAt(n);
                    soundSourceFactory.Despawn(handle.Source);
                    return;
                }
            }

            DebugOnly.Error("Attempted to stop sound that is not playing in this channel.");
        }

        public void StopAllSounds(bool includingSurviveSceneLoad)
        {
            int n = soundSources.Count;
            while (n-- > 0) {
                var source = soundSources[n];
                if (!includingSurviveSceneLoad && source.SurviveSceneLoad)
                    continue;
                soundSources.RemoveAt(n);
                soundSourceFactory.Despawn(source);
            }
        }

        internal void InternalInit(AudioMixer mixer)
        {
            channelEnabled = PlayerPrefs.GetInt($"{Name}Enabled", 1) != 0;
            volume = PlayerPrefs.GetFloat($"{Name}Volume", 1.0f);
            volumeChanged = true;

            var groups = mixer.FindMatchingGroups(Name);
            DebugOnly.Check(groups.Length > 0, $"Didn't find mixer group for \"{Name}\".");
            mixerGroup = groups[0];
        }

        internal void InternalUpdate(AudioMixer mixer, AudioListener listener)
        {
            if (volumeChanged) {
                volumeChanged = false;
                float targetVolume = (channelEnabled ? Mathf.Lerp(MixerSilent, MixerNormal, volume) : MixerSilent);
                mixer.SetFloat($"{Name}Volume", targetVolume);
            }

            int n = soundSources.Count;
            while (n-- > 0) {
                var source = soundSources[n];
                if (source == null) {
                    soundSources.RemoveAt(n);
                    continue;
                }

                if (!source.AudioSource.isPlaying) {
                    soundSources.RemoveAt(n);
                    soundSourceFactory.Despawn(source);
                    continue;
                }

                if (!channelEnabled && !source.AudioSource.loop) {
                    soundSources.RemoveAt(n);
                    soundSourceFactory.Despawn(source);
                    continue;
                }

                var sourceTransform = source.TargetTransform;
                if (sourceTransform == null)
                    sourceTransform = listener.transform;

                source.transform.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
            }
        }
    }
}
