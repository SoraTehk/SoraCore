using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Serialization
{
    using MyBox;
    using System;
    using UnityEngine;
    using TypeToDataDict = System.Collections.Generic.Dictionary<string, object>;

#if UNITY_EDITOR
    using UnityEditor;
    public partial class SaveableController
    {
        [ButtonMethod]
        private void GenerateAssetGUID()
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Prefab.GetInstanceID(), out string guid, out long _);
            Undo.RecordObject(this, "Generate AssetGUID");
            AssetGUID = guid;
        }

        [ButtonMethod]
        private void GenerateGUID()
        {
            Undo.RecordObject(this, "Generate GUID");
            GUID = Guid.NewGuid().ToString();
        }
    }
#endif

    public partial class SaveableController : MonoBehaviour
    {
        [field: SerializeField] public bool IsRuntimeInstantiate { get; private set; } = false;

        [field: SerializeField] public BlueprintAsset Prefab { get; private set; }
        [field: SerializeField] public string AssetGUID { get; private set; }

        [field: SerializeField] public string GUID { get; private set; }

        // Loop thought all ISaveable on the GameObject & save it
        public TypeToDataDict SaveStates()
        {
            var typeToData = new TypeToDataDict();

            foreach (var saveable in GetComponents<ISaveable>())
            {
                string typeName = saveable.GetType().ToString();

                if (typeToData.TryGetValue(typeName, out _))
                {
                    Debug.LogWarning($"Detecting multi components of the same type on {saveable.gameObject.name}", saveable.gameObject);
                }
                else
                {
                    typeToData[typeName] = saveable.SaveState();
                }
            }

            return typeToData;
        }

        // Loop thought all ISaveable on the GameObject & load it
        public void LoadStates(TypeToDataDict typeToData)
        {

            foreach (var saveable in GetComponents<ISaveable>())
            {
                string typeName = saveable.GetType().ToString();
                if (typeToData.TryGetValue(typeName, out object data))
                {
                    saveable.LoadState(data);
                }
            }
        }
    }
}