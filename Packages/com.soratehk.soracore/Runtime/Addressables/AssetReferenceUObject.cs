using System;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace SoraTehk.AddressablesAddons {
    [Serializable]
    public class AssetReferenceUObject<T> : AssetReference where T : UObject {
        public AssetReferenceUObject() { }
        public AssetReferenceUObject(string guid) : base(guid) { }
        
        public new T? Asset => base.Asset as T;
#if UNITY_EDITOR
        public new T? editorAsset => base.editorAsset as T;
#endif
        
        public override bool ValidateAsset(UObject obj) {
#if UNITY_EDITOR
            return obj is T;
#else
            return false;
#endif
        }
        public override bool ValidateAsset(string path) {
#if UNITY_EDITOR
            return typeof(T).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(path));
#else
            return false;
#endif
        }
    }
}