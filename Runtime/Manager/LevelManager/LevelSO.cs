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
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            // If the selection is a folder
            if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID)) {
                path += "/LevelData.asset";
            }
            // Elseif the selection is not a SceneAsset file
            else if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) {
                path = ProjectWindowUtil.GetContainingFolder(path) + "/LevelData.asset";
            }
            else {
                instance.SceneReference = new AssetReferenceT<SceneAsset>(Selection.assetGUIDs[0]);
                path = path.Replace(".unity", ".asset");
            }

            // Create .asset file , select, rename
            ProjectWindowUtil.CreateAsset(instance, path);
        }
    }
#endif

    public partial class LevelSO : ScriptableObject {
        [field: SerializeField] public AssetReference SceneReference { get; private set; }

#if PREFAB_UTILITIES
        [field: SerializeField] public List<PrefabSO> PreloadPrefabList { get; private set; }
#endif

        [field: SerializeField]  public List<LevelSO> SubLevels { get; private set; }
    }


}



