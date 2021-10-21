using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Manager {
    [CreateAssetMenu(fileName = "Audio", menuName = "Sora/Data/Audio", order = 11)]
    public class AudioSO : ScriptableObject {
        [SerializeField] AudioClip[] audioClips = null;
        public AudioClip GetClip {
            get {
                switch(audioClips.Length) {
                    case 0:
                        Debug.Log("No clip found in " + name, this);
                        return null;
                    case 1:
                        return audioClips[0];
                }

                return audioClips[Random.Range(0, audioClips.Length - 1)];
            }
        }
        public bool loop = false;
        public AudioConfigurationSO audioConfigurationSO = null;
    }
}
