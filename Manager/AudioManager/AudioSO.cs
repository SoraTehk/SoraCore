using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "Audio", menuName = "SoraCore/Audio Manager/AudioData", order = 51)]
    public class AudioSO : ScriptableObject {
        public enum SequenceMode {
            Random,
            RandomNoImmediateRepeat,
            Sequential
        }
        public AudioConfigurationSO audioConfiguration;
        public MixerGroupSO mixerGroup;
        public bool Loop = false;
        // TODO: Code SequenceMode logic
#pragma warning disable CS0414 // Remove unused private members
        [SerializeField] private SequenceMode sequenceMode = SequenceMode.RandomNoImmediateRepeat;
#pragma warning restore CS0414 // Remove unused private members
        [SerializeField] private AudioClip[] audioClips = null;

        /// <summary>
        /// Get the next clip (random)
        /// </summary>
        public AudioClip GetClip {
            get {
                if (audioClips.Length == 1) return audioClips[0];
                if (audioClips.Length > 1) return audioClips[Random.Range(0, audioClips.Length)];

                Assert.AreEqual(audioClips.Length, 0);
                return null;
            }
        }
    }
}
