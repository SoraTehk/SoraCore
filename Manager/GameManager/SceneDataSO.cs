using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using Sora.Extension;

using static Sora.Constant;


namespace Sora.Manager {
    [CreateAssetMenu(fileName = "SceneData", menuName = "Sora/Data/SceneData", order = 0)]
    public class SceneDataSO : ScriptableObject {
        [ReadOnly]
        [SerializeField]    private string  _sceneName;
                            public  string  SceneName {
                                get { return _sceneName; }
                            }
        [ReadOnly]
#region EDITOR
#if UNITY_EDITOR
        [SerializeField]    private string  _path;
                            public  string  Path {
                                get { return _path; }
                            }
                            [ReadOnly]
        [SerializeField]    private SceneAsset  _asset;
                            public  SceneAsset  Asset {
                                get { return _asset; }
                            }
#endif
#endregion
        [Range(0, 20)]
        [SerializeField]    private int     _buildIndex;
                            public  int     BuildIndex {
                                get { return _buildIndex; }
                            }

        [SerializeField]    private List<PrefabDataSO> _preloadPrefabList;
                            public  List<PrefabDataSO> PreloadPrefabList {
                                get { return _preloadPrefabList; }
                            }

#if UNITY_EDITOR
        void OnValidate() {  
            EditorBuildSettingsScene buildScene = default;
            
            if(_buildIndex >= 0 && _buildIndex < EditorBuildSettings.scenes.Length) {
                buildScene = EditorBuildSettings.scenes[_buildIndex];
                this._sceneName = buildScene.GetName();
#region EDITOR
#if UNITY_EDITOR
                this._path = buildScene.path;
                this._asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path);
#endif
#endregion
            } else {
                this._sceneName = default;
#region EDITOR
#if UNITY_EDITOR
                this._path = default;
                this._asset = default;
#endif
#endregion
                Debug.LogWarning(SORA_NULL + "Cant find any scene associate with the <b>buildIndex of</b> " + _buildIndex, this);
            }
        }

        void OnEnable() {
            EditorBuildSettings.sceneListChanged += OnValidate;
        }

        void OnDisable() {
            EditorBuildSettings.sceneListChanged -= OnValidate;
        }
#endif
    }
}