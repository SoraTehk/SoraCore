using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.Diagnostics;
using MyBox;

using Debug = UnityEngine.Debug;

namespace Sora.Manager {
    [CreateAssetMenu(fileName = "AudioManager", menuName = "Sora/Manager/AudioManager")]
    public class AudioManager : SingletonScriptableObject<AudioManager> {
        public const float MIN_VOLUME = 0.0001f;
        public AudioMixer audioMixer;
        public float volumeMultiplier = 30f;
        public PrefabDataSO prefabData;

        [SerializeField]string musicMixerGroupLookupName;
        [SerializeField]string unsortedMixerGroupLookupName;
        [SerializeField]string[] mixerGroupFolders;
        [SerializeField]string[] audioFolders;
        [Space]
        [Header("◀GENERATING FIELDS▶")]
        public MixerGroupSO musicMixerGroup;
        public MixerGroupSO unsortedMixerGroup;
        public List<MixerGroupSO> mixerGroups;
        public List<AudioSO> audios;

        public static void Play(AudioSO audio) => Play(audio, audio.audioConfigurationSO);
        public static void Play(AudioSO audio, AudioConfigurationSO config) {
            AudioSource audioSource = PrefabManager.Spawn(SharedInstance.prefabData, null).GetComponent<AudioSource>();

            //TODO: Optimize this if needed
            audioSource.clip = audio.GetClip;
            audioSource.loop = audio.loop;
            config?.ApplyTo(audioSource);

            audioSource.Play();
        }

        #region Volume
        public static void LoadPlayerPrefsAll() {
            foreach(MixerGroupSO mixerGroup in SharedInstance.mixerGroups) {
                float value = LoadPlayerPrefs(mixerGroup);
                #region EDITOR
#if UNITY_EDITOR
                if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": <b>" + mixerGroup.volumeParameter + "</b> load and set to " + value);
#endif
                #endregion
                AudioManager.SetVolume(mixerGroup, value);
            }
        }

        public static void SavePlayerPrefs(MixerGroupSO mixerGroup, float value) {
            PlayerPrefs.SetFloat(typeName + mixerGroup.volumeParameter, value);
        }
        public static float LoadPlayerPrefs(MixerGroupSO mixerGroup) {
            if(mixerGroup) {
                return PlayerPrefs.GetFloat(typeName + mixerGroup.volumeParameter, MIN_VOLUME);
            }
            #region EDITOR
#if UNITY_EDITOR
            if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": <b>mixerGroup</b> paramater are null.\n\n<b>LoadPlayerPrefs(MixerGroupSO mixerGroup)</b>");
#endif
            #endregion
            return 1f;
        }

        public static void SetVolume(MixerGroupSO mixerGroup, float value) {
            #region EDITOR
#if UNITY_EDITOR
            const string METHOD_NAME = "<b>SetVolume(MixerGroupSO mixerGroup, float value)</b>";
#endif
            #endregion
            if(!mixerGroup) {
                #region EDITOR
#if UNITY_EDITOR
                if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": <b>mixerGroup</b> paramater are null.\n\n" + METHOD_NAME);
#endif
                #endregion
                return;
            }

            #region EDITOR
#if UNITY_EDITOR
            if(SharedInstance.debugMode) {
                if      (value <=0) Debug.Log(SORA_MANAGER_LOG + ": <b>" + mixerGroup.group.name + " value</b> paramater should be at least 0.0001f. \n\n" + METHOD_NAME);
                else if (value > 1) Debug.Log(SORA_MANAGER_LOG + ": <b>" + mixerGroup.group.name + " value</b> paramater > 1, it could be too loud. \n\n" + METHOD_NAME);
            }
#endif
            #endregion
            //Magic math lol
            float dBValue = Mathf.Log10(Mathf.Max(MIN_VOLUME, value)) * SharedInstance.volumeMultiplier;
            SharedInstance.audioMixer.SetFloat(mixerGroup.volumeParameter, dBValue);
            return;

        }
        #endregion

    #region EDITOR
#if UNITY_EDITOR
        public void Generate() {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region MixerData
            //Clear all field first
            musicMixerGroup = null;
            unsortedMixerGroup = null;
            mixerGroups.Clear();
            
            //What to call in debug log
            string musicIdentifier = nameof(MixerGroupSO) + " with MixerGroup as " + musicMixerGroupLookupName;
            string unsortedIdentifier = nameof(MixerGroupSO) + " with MixerGroup as " + unsortedMixerGroupLookupName;

            //Search though project asset folders
            string[] assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(MixerGroupSO).FullName, mixerGroupFolders);
            foreach(string assetGUID in assetGUIDs) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                MixerGroupSO mixerData = AssetDatabase.LoadAssetAtPath<MixerGroupSO>(path);
                mixerGroups.Add(mixerData);

                //Assign music/unsorted mixer data
                if(mixerData.group.ToString() == musicMixerGroupLookupName) {
                    if(!musicMixerGroup) {
                        musicMixerGroup = mixerData;
                    } else {
                        Debug.LogWarning("More than 1 " + musicIdentifier + " was found");
                        Debug.LogWarning("Using " + musicIdentifier + " at: " + path, musicMixerGroup);
                    }
                }
                if(mixerData.group.ToString() == unsortedMixerGroupLookupName) {
                    if(!unsortedMixerGroup) {
                        unsortedMixerGroup = mixerData;
                    } else {
                        Debug.LogWarning("More than 1 " + unsortedIdentifier + " was found");
                        Debug.LogWarning("Using " + unsortedIdentifier + " at: " + path, unsortedMixerGroup);
                    }
                }
            }

            //If cant find any
            if(!musicMixerGroup) {
                Debug.LogWarning("Cant find any " + musicIdentifier);
            }
            if(!unsortedMixerGroup) {
                Debug.LogWarning("Cant find any " + unsortedIdentifier);
            }
            #endregion
            #region AudioData
            //Clear all field first
            audios.Clear();

            //Search though project asset folders
            assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(AudioSO).FullName, audioFolders);
            foreach (string assetGUID in assetGUIDs) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                audios.Add(AssetDatabase.LoadAssetAtPath<AudioSO>(path));
            }
            #endregion

            stopwatch.Stop();
            Debug.Log("Generated in: " + stopwatch.ElapsedMilliseconds + "ms");
        }
#endif
    #endregion
    }
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(AudioManager), editorForChildClasses: true)]
    public class AudioManagerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUI.enabled = true;

            AudioManager e = target as AudioManager;
            if (GUILayout.Button("Generate"))
                e?.Generate();
        }
    }
#endif
    #endregion
}
