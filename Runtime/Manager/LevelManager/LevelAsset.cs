using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Level
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public partial class LevelAsset : ScriptableObject
    {
        [field: AssetReferenceUILabelRestriction("Scene")]
        [field: SerializeField] public AssetReference SceneReference { get; private set; }
        [field: SerializeField] public List<BlueprintAsset> PreloadBlueprints { get; private set; }
        [field: SerializeField] public List<LevelAsset> SubLevels { get; private set; }
    }
}

#if UNITY_EDITOR
namespace SoraCore.Manager.Level
{
    using UnityEditor;
    using UnityEngine.AddressableAssets;
    
    public partial class LevelAsset
    {
        [MenuItem("Assets/Create/SoraCore/Level Manager/Level")]
        private static void CreateAsset()
        {
            // Create ScriptableObject instance
            var instance = CreateInstance<LevelAsset>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            // If the selection is a folder
            if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
            {
                path += "/LevelData.asset";
            }
            // Elseif the selection is not a SceneAsset file
            else if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
            {
                path = ProjectWindowUtil.GetContainingFolder(path) + "/LevelData.asset";
            }
            else
            {
                instance.SceneReference = new AssetReferenceT<SceneAsset>(Selection.assetGUIDs[0]);
                path = path.Replace(".unity", ".asset");
            }

            // Create .asset file , select, rename
            ProjectWindowUtil.CreateAsset(instance, path);
        }
    }
}
#endif