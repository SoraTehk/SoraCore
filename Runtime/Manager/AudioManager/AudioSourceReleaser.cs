using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Audio
{
    using MyBox;
    using UnityEngine;

    public class AudioSourceReleaser : MonoBehaviour
    {
        [field: SerializeField, AutoProperty] public AudioSource AudioSource { get; private set; }

        private void Update()
        {
            if (AudioListener.pause || AudioSource.isPlaying) return;

            InstantiateManager.ReleaseInstance(gameObject);
        }
    }
}