#if UNITY_EDITOR
using System.IO;
using SoraTehk.AddressablesAddons;
using UnityEditor;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class AssetDatabaseX {
        public static bool TryFindFirstPrefab<TAssetRef, TAssetType>(out TAssetRef? assetRef, string? assetName = null)
            where TAssetRef : AssetReferenceComponent<TAssetType>, new()
            where TAssetType : Component {
            //
            assetRef = null;
            
            GUID[] guids = AssetDatabase.FindAssetGUIDs("t:Prefab");
            foreach (GUID guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Contains(assetName)) continue;
                }
                // Check matching asset
                var prefab = AssetDatabase.LoadAssetByGUID<GameObject>(guid);
                if (prefab.TryGetComponent(out TAssetType _)) {
                    assetRef = new TAssetRef();
                    assetRef.SetEditorAsset(prefab);
                    return true;
                }
            }
            return false;
        }
        public static bool TryFindFirstSceneAsset(out AssetReferenceScene? assetRef, string? assetName = null) {
            //
            assetRef = null;
            
            GUID[] guids = AssetDatabase.FindAssetGUIDs($"t:SceneAsset");
            foreach (var guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Contains(assetName)) continue;
                }
                
                assetRef = new AssetReferenceScene();
                assetRef.SetEditorAsset(AssetDatabase.LoadAssetByGUID<SceneAsset>(guid));
                return true;
            }
            return false;
        }
        public static bool TryFindFirstAsset<TAssetRef, TAssetType>(out TAssetRef? assetRef, string? assetName = null)
            where TAssetRef : AssetReferenceUObject<TAssetType>, new()
            where TAssetType : UObject {
            //
            assetRef = null;
            
            GUID[] guids = AssetDatabase.FindAssetGUIDs($"t:{typeof(TAssetType)}");
            foreach (var guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Contains(assetName)) continue;
                }
                
                assetRef = new TAssetRef();
                assetRef.SetEditorAsset(AssetDatabase.LoadAssetByGUID<TAssetType>(guid));
                return true;
            }
            return false;
        }
    }
}
#endif