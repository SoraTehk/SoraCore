namespace SoraCore.Manager {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
    using UnityEditor;

    public partial class LevelSO : ScriptableObject {
        [MenuItem("Assets/Create/SoraCore/Game Manager/Level Data")]
        private static void CreateAsset() {
            // Create ScriptableObject instance
            var instance = CreateInstance<LevelSO>();
            instance._editorSceneReference = new AssetReferenceT<SceneAsset>(Selection.assetGUIDs[0]);

            // Create .asset file , select, rename
            var path = AssetDatabase.GetAssetPath(Selection.activeObject).Replace(".unity", ".asset");
            ProjectWindowUtil.CreateAsset(instance, path);
        }

        [SerializeField] private AssetReferenceT<SceneAsset> _editorSceneReference;

        private void OnValidate() {
            sceneReference = _editorSceneReference;
        }
    }
#endif

    public partial class LevelSO : ScriptableObject {
        [HideInInspector] public AssetReference sceneReference;
        [field: SerializeField] public List<PrefabSO> PreloadPrefabList { get; private set; }
        public List<LevelSO> subLevels;
    }

}



