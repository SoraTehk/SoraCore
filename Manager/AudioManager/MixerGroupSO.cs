using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Sora.Manager {
    [CreateAssetMenu(fileName = "MixerGroup", menuName = "Sora/Data/MixerGroup", order = 13)]
    public class MixerGroupSO : ScriptableObject {
        public AudioMixerGroup group = null;
        public string pitchParameter = null;
        public string volumeParameter = null;
        public string[] otherParameter = null;
    }
}