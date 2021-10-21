using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Sora.Extension;

namespace Sora.Manager {
    public class AudioSource_Pooled : MonoBehaviour
    {
        [ReadOnly] public AudioSource audioSource;

        private void Awake() {
            transform.GetComponentNullCheck<AudioSource>(ref audioSource);
        }

        private void Update() {
            if(AudioListener.pause || audioSource.isPlaying) return;

            PrefabManager.Despawn(this);
        }
    }
}