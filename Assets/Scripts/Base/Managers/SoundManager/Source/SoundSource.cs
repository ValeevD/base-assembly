using System;
using UnityEngine;
using DG.Tweening;

namespace Base
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundSource : MonoBehaviour
    {
        public AudioSource AudioSource { get; private set; }
        public int HandleID { get; private set; }

        public Transform TargetTransform;
        public bool SurviveSceneLoad;

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.playOnAwake = false;

          #if UNITY_EDITOR
            gameObject.name = "<Free>";
          #endif
        }

        public void Spawn(AudioClip clip)
        {
            AudioSource.clip = clip;

            #if UNITY_EDITOR

            gameObject.name = (clip != null ? clip.name : "<Invalid>");

            #endif
        }

        public void Despawn()
        {
          #if UNITY_EDITOR
            gameObject.name = "<Free>";
          #endif

            AudioSource.DOKill(false);
            if (AudioSource.isPlaying)
                AudioSource.Stop();

            TargetTransform = null;
            AudioSource.clip = null;
            HandleID++;
        }

        public Tweener DOFade(float endValue, float time)
        {
            return DOTween.To(
                    () => AudioSource.volume,
                    (value) => AudioSource.volume = value,
                    endValue,
                    time)
                .SetOptions(false)
                .SetTarget(this);
        }
    }
}
