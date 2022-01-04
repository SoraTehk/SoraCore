using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "EC_AudioManager", menuName = "SoraCore/Audio Manager/Audio Manager EC", order = 1)]
    public class AudioManagerEventChannelSO : ScriptableObject {
        public event UnityAction<MixerGroupSO, float> SetVolumeRequested;
        public event UnityAction<MixerGroupSO, float> SavePlayerPrefsRequested;
        public event Func<MixerGroupSO, float> LoadPlayerPrefsRequested;
        public event UnityAction LoadPlayerPrefsAllRequested;

        public void SetVolume(MixerGroupSO group, float value) {
            if (SetVolumeRequested != null)
            {
                SetVolumeRequested.Invoke(group, value);
            }
            else
            {
                Debug.LogWarning("An set volume event was requested, but no AudioManager picked it up.");
            }
        }

        public void SavePlayerPrefs(MixerGroupSO group, float value) {
            if (SavePlayerPrefsRequested != null)
            {
                SavePlayerPrefsRequested.Invoke(group, value);
            }
            else
            {
                Debug.LogWarning("A save mixer volume to PlayerPrefs event was requested, but no AudioManager picked it up.");
            }
        }
        public float LoadPlayerPrefs(MixerGroupSO group) {
            if (LoadPlayerPrefsRequested != null) return LoadPlayerPrefsRequested.Invoke(group);

            Debug.LogWarning("A load mixer volume from PlayerPrefs event was requested, but no AudioManager picked it up.");
            return 1f;
        }
        public void LoadPlayerPrefsAll() {
            if (LoadPlayerPrefsAllRequested != null)
            {
                LoadPlayerPrefsAllRequested.Invoke();
            }
            else
            {
                Debug.LogWarning("A load all from PlayerPrefs and set volume event was requested, but no AudioManager picked it up.");
            }
        }
    }
}
