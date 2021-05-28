using UnityEngine;

namespace Base
{
    public interface ISoundSourceFactory {
        SoundSource Spawn(AudioClip clip);
        void Despawn(SoundSource source);
    }
}
