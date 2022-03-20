namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "AudioData", menuName = "SoraCore/Sound Manager/Audio Data", order = 51)]
    public class AudioCueSO : ScriptableObject {
        public enum SequenceMode {
            Random,
            RandomNoImmediateRepeat,
            Sequential
        }
        [field: SerializeField] public AudioConfigurationSO AudioConfiguration { get; private set; }
        [field: SerializeField] public MixerGroupSO MixerGroup { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; } = false;

        // TODO: SequenceMode logic
        [SerializeField] private SequenceMode _sequenceMode = SequenceMode.RandomNoImmediateRepeat;
        [SerializeField] private AudioClip[] _audioClips = null;

        /// <summary>
        /// Get the next clip
        /// </summary>
        public AudioClip GetClip
        {
            get
            {
                if (_audioClips.Length == 1) return _audioClips[0];

                if (_audioClips.Length == 0) return null;

                if (_sequenceMode == SequenceMode.Random) {
                    return _audioClips[Random.Range(0, _audioClips.Length)];
                }

                return null;
            }
        }
    }
}
