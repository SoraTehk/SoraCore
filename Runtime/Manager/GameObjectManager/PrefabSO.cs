namespace SoraCore.Manager {
    using UnityEngine;
    using MyBox;
#if UNITY_EDITOR
    using UnityEditor;
    public partial class PrefabSO : ScriptableObject {
        void OnValidate() {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(GetInstanceID(), out string assetguid, out long _)) {
                AssetGuid = assetguid;
            }
        }

    }
#endif

    [CreateAssetMenu(fileName = "PrefabData", menuName = "SoraCore/GameObject Manager/Prefab Data")]
    public partial class PrefabSO : ScriptableObject {
        [ReadOnly] public string AssetGuid;
        [Space(20)]
        public GameObject prefab;
        public bool enablePooling;
        [ReadOnly("enablePooling", true)] public int preload = 10;
        [ReadOnly("enablePooling", true)] public int capacity = 10;

        // Inherit this class and override these 3 for specific needs
        #region ObjectPool<T> delegate methods
        public virtual void OnGameObjGet(GameObject gObj) => gObj.SetActive(true);
        public virtual void OnGameObjRelease(GameObject gObj) => gObj.SetActive(false);
        public virtual void OnGameObjDestroy(GameObject gObj) => Destroy(gObj);
        #endregion
    }
}

