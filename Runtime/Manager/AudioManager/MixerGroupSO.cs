namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.Audio;

    [CreateAssetMenu(fileName = "MixerGroup", menuName = "SoraCore/Sound Manager/MixerGroup Data", order = 53)]
    public class MixerGroupSO : ScriptableObject {
        [field: SerializeField] public AudioMixerGroup Group { get; private set; }
        [field: SerializeField] public string VolumeParameter { get; private set; }
        [field: SerializeField] public string PitchParameter { get; private set; }
    }
}