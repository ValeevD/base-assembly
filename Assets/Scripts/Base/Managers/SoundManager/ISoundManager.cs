using UnityEngine;

namespace Base
{
    public interface ISoundManager {
        ISoundChannel SFX { get; }
        ISoundChannel Music { get; }

        ISoundChannel GetChannel(string name);

        void PlayMusic(AudioClip clip, float volume = 1.0f);
        void StopMusic();
    }
}
