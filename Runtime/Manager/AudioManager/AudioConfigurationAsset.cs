namespace SoraCore.Manager.Audio
{
    using MyBox;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "SoraCore/Audio Manager/Audio Configuration", order = 52)]
    public class AudioConfigurationAsset : ScriptableObject
    {
        enum PriorityLevel
        {
            Highest = 0,
            High = 64,
            Standard = 128,
            Low = 194,
            VeryLow = 256,
        }

        // Simplified management of priority levels (values are counterintuitive, see enum below)
        [SerializeField] private PriorityLevel m_Priority = PriorityLevel.Standard;
        [HideInInspector] public int Priority => (int)m_Priority;

        [field: Separator("Properties")]
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(-3f, 3f)] public float Pitch { get; private set; } = 1f;
        [field: SerializeField, Range(-1f, 1f)] public float StereoPan { get; private set; } = 0f;
        [field: SerializeField, Range(0f, 1.1f)] public float ReverbZoneMix { get; private set; } = 1f;

        [field: Separator("Spatialisation")]
        [field: SerializeField, Range(0f, 1f)] public float SpatialBlend { get; private set; } = 1f;
        [field: SerializeField] public AudioRolloffMode RolloffMode { get; private set; } = AudioRolloffMode.Logarithmic;
        [field: SerializeField, Range(0.1f, 5f)] public float MinDistance { get; private set; } = 0.1f;
        [field: SerializeField, Range(5f, 100f)] public float MaxDistance { get; private set; } = 50f;
        [field: SerializeField, Range(0, 360)] public int Spread { get; private set; } = 0;
        [field: SerializeField, Range(0f, 5f)] public float DopplerLevel { get; private set; } = 1f;

        [field: Separator("Ignores")]
        [field: SerializeField] public bool BypassEffects { get; private set; } = false;
        [field: SerializeField] public bool BypassListenerEffects { get; private set; } = false;
        [field: SerializeField] public bool BypassReverbZones { get; private set; } = false;
        [field: SerializeField] public bool IgnoreListenerVolume { get; private set; } = false;
        [field: SerializeField] public bool IgnoreListenerPause { get; private set; } = false;
    }

    public static partial class Extension
    {
        /// <summary>
        /// Apply <paramref name="config"/> to this <see cref="AudioSource"/>
        /// </summary>
        public static void ApplyConfig(this AudioSource audioSrc, AudioConfigurationAsset config)
        {
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