using SoraCore.Extension;

namespace SoraCore.Manager {
    using MyBox;
    using UnityEngine;
    public class AudioSourceReleaser : MonoBehaviour {
        [field: SerializeField, AutoProperty] private AudioSource _audioSource;

        private void Update() {
            if (AudioListener.pause || _audioSource.isPlaying) return;

            GameObjectManager.Release(gameObject);
        }
    }
}