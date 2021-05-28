using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class SoundSourceFactory : AbstractService<ISoundSourceFactory>, ISoundSourceFactory
    {
        public int poolSize = 32;
        public SoundSource soundSourcePrefab;

        public Transform sourceGroup;

        private List<SoundSource> soundSourcesPool;

        private void Awake() {
            soundSourcesPool = new List<SoundSource>();

            int n = poolSize;

            while(--n >= 0)
            {

            }
        }

        private SoundSource CreateSoundSource()
        {
            SoundSource newSource;

            if(soundSourcePrefab != null)
                newSource = Instantiate(soundSourcePrefab);
            else
            {
                var go = new GameObject();
                newSource = go.AddComponent<SoundSource>();
            }

            return newSource;
        }

        public SoundSource Spawn(AudioClip clip)
        {
            SoundSource newSource;

            if(soundSourcePrefab != null)
                newSource = Instantiate(soundSourcePrefab);
            else
            {
                var go = new GameObject();
                newSource = go.AddComponent<SoundSource>();
            }

            newSource.Spawn(clip);
            return newSource;
        }

        public void Despawn(SoundSource source)
        {
            Destroy(source.gameObject);
        }
    }
}
