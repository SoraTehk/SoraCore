using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using SoraCore.Extension;

namespace SoraCore.Manager {
    public class AudioSourceReleaser : MonoBehaviour
    {
        [SerializeField] private PrefabSO _prefabData;
        [SerializeField] private GameObjectManagerEventChannelSO _goManagerEC;
        [ReadOnly] public AudioSource AudioSource;

        private void Awake() {
            transform.GetComponentNullCheck(ref AudioSource);
        }

        private void Update() {
            if(AudioListener.pause || AudioSource.isPlaying) return;

            _goManagerEC.Destroy(gameObject, _prefabData);
        }
    }
}