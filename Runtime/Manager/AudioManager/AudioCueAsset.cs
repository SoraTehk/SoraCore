namespace SoraCore.Manager.Audio
{
    using UnityEngine;

    public enum SequenceMode
    {
        Random,
        RandomNoImmediateRepeat,
        Sequential
    }

    [CreateAssetMenu(fileName = "AudioCue", menuName = "SoraCore/Audio Manager/Audio Cue", order = 51)]
    public class AudioCueAsset : ScriptableObject
    {

        [field: SerializeField] public AudioConfigurationAsset AudioConfiguration { get; private set; }
        [field: SerializeField] public MixerGroupAsset MixerGroup { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; }

        // TODO: SequenceMode logic
        [field: SerializeField] public SequenceMode SequenceMode { get; private set; }
        [field: SerializeField] public AudioClip[] AudioClips { get; private set; }

        /// <summary>
        /// Get the next clip
        /// </summary>
        public AudioClip GetClip
        {
            get
            {
                if (AudioClips.Length == 1) return AudioClips[0];

                if (AudioClips.Length == 0) return null;

                if (SequenceMode == SequenceMode.Random)
                {
                    return AudioClips[Random.Range(0, AudioClips.Length)];
                }

                return null;
            }
        }
    }
}
