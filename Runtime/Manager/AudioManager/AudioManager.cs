using static SoraCore.Constant;

namespace SoraCore.Manager {
    using MyBox;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class AudioManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------
        private static Action<AudioSO, Vector3, AudioConfigurationSO, MixerGroupSO> _playAudioRequested;
        public static void PlayAudio(AudioSO audio, Vector3 pos) => PlayAudio(audio, pos, audio.audioConfiguration, audio.mixerGroup);
        public static void PlayAudio(AudioSO audio, Vector3 pos, AudioConfigurationSO config, MixerGroupSO group) {
            if (_playAudioRequested != null) {
                _playAudioRequested.Invoke(audio, pos, config, group);
                return;
            }

            LogWarningForEvent(nameof(AudioManager));
        }

        private static Action<MixerGroupSO, float> _setVolumeRequested;
        public static void SetVolume(MixerGroupSO group, float value) {
            if (_setVolumeRequested != null) {
                _setVolumeRequested.Invoke(group, value);
                return;
            }

            LogWarningForEvent(nameof(AudioManager));
        }

        private static Action<MixerGroupSO, float> _savePlayerPrefsRequested;
        public static void SavePlayerPrefs(MixerGroupSO group, float value) {
            if (_savePlayerPrefsRequested != null) {
                _savePlayerPrefsRequested.Invoke(group, value);
                return;
            }

            LogWarningForEvent(nameof(AudioManager));
        }

        private static Func<MixerGroupSO, float> _loadPlayerPrefsRequested;
        public static float LoadPlayerPrefs(MixerGroupSO group) {
            if (_loadPlayerPrefsRequested != null) return _loadPlayerPrefsRequested.Invoke(group);

            LogWarningForEvent(nameof(AudioManager));
            return 1f;
        }

        private static Action _loadPlayerPrefsAllRequested;
        public static void LoadPlayerPrefsAll() {
            if (_loadPlayerPrefsAllRequested != null) {
                _loadPlayerPrefsAllRequested.Invoke();
                return;
            }

            LogWarningForEvent(nameof(AudioManager));
        }
        #endregion


        public const float MIN_VOLUME = 0.0001f;
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }
        [field: SerializeField] public float VolumeMultiplier { get; private set; } = 30f;
        [SerializeField] private PrefabSO _audioSourcePrefab;

        [field: Separator("Data")]
        [field: SerializeField] public List<MixerGroupSO> MixerGroups { get; private set; }
        [field: SerializeField] public List<AudioSO> Audios { get; private set; }
        // Parameters for scanning process
#if UNITY_EDITOR
        [SerializeField] private string[] _mixerGroupFolders;
        [SerializeField] private string[] _audioFolders;
#endif

        private void OnEnable() {
            _playAudioRequested += InnerPlayAudio;


            _setVolumeRequested += InnerSetVolume;
            _savePlayerPrefsRequested += InnerSavePlayerPrefs;
            _loadPlayerPrefsRequested += InnerLoadPlayerPrefs;
            _loadPlayerPrefsAllRequested += InnerLoadPlayerPrefsAll;
        }

        private void OnDisable() {
            _playAudioRequested -= InnerPlayAudio;

            _setVolumeRequested -= InnerSetVolume;
            _savePlayerPrefsRequested -= InnerSavePlayerPrefs;
            _loadPlayerPrefsRequested -= InnerLoadPlayerPrefs;
            _loadPlayerPrefsAllRequested -= InnerLoadPlayerPrefsAll;
        }

        /// <summary>
        /// Spawn an <see cref="AudioSource"/> that play <paramref name="audio"/> at <paramref name="position"/> using custom configuration
        /// </summary>
        private void InnerPlayAudio(AudioSO audio, Vector3 position, AudioConfigurationSO config, MixerGroupSO group) {
            /// Spawn an audio source at target position
            AudioSource audioSource = GameObjectManager.Instantiate(_audioSourcePrefab).GetComponent<AudioSource>();
            audioSource.transform.position = position;

            // Apply configuration to the audio source
            audioSource.clip = audio.GetClip;
            audioSource.loop = audio.Loop;
            audioSource.ApplyConfig(config);
            audioSource.outputAudioMixerGroup = group.Group;

            // Play
            audioSource.Play();
        }

        /// <summary>
        /// Change dB volume of <paramref name="mixerGroup"/> base on <paramref name="value"/>
        /// </summary>
        public void InnerSetVolume(MixerGroupSO mixerGroup, float value) {
            if (value > 1) Debug.LogWarning(SORA_WARNING + ": <b>" + mixerGroup.Group.name + " value</b> paramater > 1, it could be too loud.");

            // Magic number https://www.youtube.com/watch?v=MmWLK9sN3s8 (6:14)
            float dBValue = Mathf.Log10(Mathf.Max(MIN_VOLUME, value)) * VolumeMultiplier;
            AudioMixer.SetFloat(mixerGroup.VolumeParameter, dBValue);

            return;
        }

        public void InnerSavePlayerPrefs(MixerGroupSO mixerGroup, float value) {
            PlayerPrefs.SetFloat("AudioManager" + mixerGroup.VolumeParameter, value);
        }
        public float InnerLoadPlayerPrefs(MixerGroupSO mixerGroup) {
            if (!mixerGroup) return 1f;

            // TODO: Handle save/load more clearly
            return PlayerPrefs.HasKey("AudioManager" + mixerGroup.VolumeParameter)
                 ? PlayerPrefs.GetFloat("AudioManager" + mixerGroup.VolumeParameter, MIN_VOLUME)
                 : 1f;
        }
        /// <summary>
        /// Load & set all PlayerPrefs
        /// </summary>
        public void InnerLoadPlayerPrefsAll() {
            foreach (MixerGroupSO mixerGroup in MixerGroups)
                SetVolume(mixerGroup, LoadPlayerPrefs(mixerGroup));
        }

#if UNITY_EDITOR
        [ButtonMethod]
        public void ScanProject() {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            #region MixerData
            //Clear all field first
            MixerGroups.Clear();

            //Search though project asset folders
            string[] assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(MixerGroupSO).FullName, _mixerGroupFolders);
            foreach (string assetGUID in assetGUIDs) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                MixerGroupSO mixerData = AssetDatabase.LoadAssetAtPath<MixerGroupSO>(path);
                MixerGroups.Add(mixerData);
            }
            #endregion

            #region AudioData
            //Clear all field first
            Audios.Clear();

            //Search though project asset folders
            assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(AudioSO).FullName, _audioFolders);
            foreach (string assetGUID in assetGUIDs) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                Audios.Add(AssetDatabase.LoadAssetAtPath<AudioSO>(path));
            }
            #endregion

            stopwatch.Stop();
            Debug.Log("Scanned in: " + stopwatch.ElapsedMilliseconds + "ms");
        }
#endif
    }
}