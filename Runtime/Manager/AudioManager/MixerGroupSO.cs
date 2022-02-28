using UnityEngine;
using UnityEngine.Audio;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "MixerGroup", menuName = "SoraCore/Audio Manager/MixerGroupData", order = 53)]
    public class MixerGroupSO : ScriptableObject {
        public AudioMixerGroup Group = null;
        public string VolumeParameter = null;
        public string PitchParameter = null;
        public string[] Parameters = null;
    }
}