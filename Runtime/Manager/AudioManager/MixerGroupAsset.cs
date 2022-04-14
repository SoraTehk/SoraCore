namespace SoraCore.Manager.Audio
{
    using UnityEngine;
    using UnityEngine.Audio;

    [CreateAssetMenu(fileName = "MixerGroup", menuName = "SoraCore/Audio Manager/Mixer Group", order = 53)]
    public class MixerGroupAsset : ScriptableObject
    {
        [field: SerializeField] public AudioMixerGroup Group { get; private set; }
        [field: SerializeField] public string VolumeParameter { get; private set; }
        [field: SerializeField] public string PitchParameter { get; private set; }
    }
}