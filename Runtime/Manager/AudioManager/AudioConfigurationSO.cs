using MyBox;
using UnityEngine;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "SoraCore/Audio Manager/AudioConfigurationData", order = 52)]
    public class AudioConfigurationSO : ScriptableObject {
        enum PriorityLevel {
            Highest = 0,
            High = 64,
            Standard = 128,
            Low = 194,
            VeryLow = 256,
        }
        // Simplified management of priority levels (values are counterintuitive, see enum below)
        [SerializeField] PriorityLevel _priorityLevel = PriorityLevel.Standard;
        [HideInInspector]
        public int Priority
        {
            get { return (int)_priorityLevel; }
            set { _priorityLevel = (PriorityLevel)value; }
        }

        [Separator("Sound properties")]
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;
        [Range(-1f, 1f)] public float StereoPan = 0f;
        [Range(0f, 1.1f)] public float ReverbZoneMix = 1f;

        [Separator("Spatialisation")]
        [Range(0f, 1f)] public float SpatialBlend = 1f;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
        [Range(0.1f, 5f)] public float MinDistance = 0.1f;
        [Range(5f, 100f)] public float MaxDistance = 50f;
        [Range(0, 360)] public int Spread = 0;
        [Range(0f, 5f)] public float DopplerLevel = 1f;

        [Separator("Ignores")]
        public bool BypassEffects = false;
        public bool BypassListenerEffects = false;
        public bool BypassReverbZones = false;
        public bool IgnoreListenerVolume = false;
        public bool IgnoreListenerPause = false;
    }

    public static class ExtensionMethod {
        /// <summary>
        /// Apply <paramref name="config"/> to this <see cref="AudioSource"/>
        /// </summary>
        public static void ApplyConfig(this AudioSource audioSrc, AudioConfigurationSO config) {
            audioSrc.bypassEffects = config.BypassEffects;
            audioSrc.bypassListenerEffects = config.BypassListenerEffects;
            audioSrc.bypassReverbZones = config.BypassReverbZones;
            audioSrc.priority = config.Priority;
            audioSrc.volume = config.Volume;
            audioSrc.pitch = config.Pitch;
            audioSrc.panStereo = config.StereoPan;
            audioSrc.spatialBlend = config.SpatialBlend;
            audioSrc.reverbZoneMix = config.ReverbZoneMix;
            audioSrc.dopplerLevel = config.DopplerLevel;
            audioSrc.spread = config.Spread;
            audioSrc.rolloffMode = config.RolloffMode;
            audioSrc.minDistance = config.MinDistance;
            audioSrc.maxDistance = config.MaxDistance;
            audioSrc.ignoreListenerVolume = config.IgnoreListenerVolume;
            audioSrc.ignoreListenerPause = config.IgnoreListenerPause;
        }
    }
}