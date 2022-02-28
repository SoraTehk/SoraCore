using UnityEngine;
using UnityEngine.Events;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "EC_PlayAudio", menuName = "SoraCore/Audio Manager/Play Audio EC", order = 2)]
    public class PlayAudioEventChannelSO : ScriptableObject {
        public event UnityAction<AudioSO, Vector3, AudioConfigurationSO, MixerGroupSO> Requested;

        public void Raise(AudioSO audio, Vector3 pos) => Raise(audio, pos, audio.audioConfiguration, audio.mixerGroup);
        public void Raise(AudioSO audio, Vector3 pos, AudioConfigurationSO config, MixerGroupSO group) {
            if (Requested != null) {
                Requested.Invoke(audio, pos, config, group);
            }
            else {
                Debug.LogWarning("A sfx playing event was requested, but no AudioManager picked it up.");
            }
        }
    }
}
