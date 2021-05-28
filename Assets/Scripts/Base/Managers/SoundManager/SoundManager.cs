using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

namespace Base
{
    public sealed class SoundManager : AbstractService<ISoundManager>, ISoundManager, IOnCurrentSceneUnload
    {
        public float MusicFadeTime = 5.0f;

        public AudioMixer Mixer;
        public AudioListener Listener;
        public SoundChannel[] Channels;

        SoundHandle currentMusic;
        Dictionary<string, ISoundChannel> channelDict;

        public ISoundChannel SFX { get; private set; }
        public ISoundChannel Music { get; private set; }

        void Awake()
        {
            channelDict = new Dictionary<string, ISoundChannel>();
            foreach (var channel in Channels) {
                DebugOnly.Check(!channelDict.ContainsKey(channel.Name), $"Duplicate channel name: '{channel.Name}'.");
                channelDict[channel.Name] = channel;
                channel.InternalInit(Mixer);
            }

            SFX = GetChannel("SFX");
            Music = GetChannel("Music");
        }

        public ISoundChannel GetChannel(string name)
        {
            if (channelDict.TryGetValue(name, out var channel))
                return channel;
            DebugOnly.Error($"Sound channel '{name}' was not found.");
            return null;
        }

        public void PlayMusic(AudioClip clip, float volume)
        {
            if (currentMusic.IsPlaying) {
                if (currentMusic.AudioClip == clip) {
                    currentMusic.Volume = volume;
                    return;
                }

                currentMusic.DOFadeToStop(MusicFadeTime);
            }

            currentMusic = Music.Play(clip, true, true, 0.0f);
            currentMusic.DOKill(false);
            currentMusic.DOFade(1.0f, MusicFadeTime);
        }

        public void StopMusic()
        {
            if (currentMusic.IsPlaying)
                currentMusic.DOFadeToStop(MusicFadeTime);

            currentMusic = new SoundHandle();
        }

        void OnEnable()
        {
            eventBus.Subscribe<IOnCurrentSceneUnload>(this);
        }

        void IOnCurrentSceneUnload.Do()
        {
            foreach (var channel in Channels)
                channel.StopAllSounds(false);
        }

        void Update()
        {
            foreach (var channel in Channels)
                channel.InternalUpdate(Mixer, Listener);
        }
    }
}
