using SoraCore.Manager;

namespace SoraCore.EditorTools
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class LevelSelectorWindow
    {
        public static string EditorPrefsKey { get; private set; } // OnEnable();

        private Data _data;

        private void RefreshData()
        {
            Dictionary<string, LevelContext> dataGUIDs = _levelContexts.ToDictionary(ctx => ctx.GUID);
            string[] projectGUIDs = EditorHelper.FindAssetsGUIDOfType<LevelSO>();

            // Iterating through all the GUIDs in the project
            foreach (string guid in projectGUIDs)
            {
                // If GUID is not in the dictionary, it means it's a new GUID
                if (dataGUIDs.TryGetValue(guid, out LevelContext ctx))
                {
                    dataGUIDs.Remove(guid);
                    continue;
                }

                // Only add if not contained
                _levelContexts.Add(new LevelContext()
                {
                    GUID = guid,
                    Level = AssetDatabase.LoadAssetAtPath<LevelSO>(AssetDatabase.GUIDToAssetPath(guid))
                });
            }

            // Remove non matching GUIDs (null/deleted/changed)
            foreach (LevelContext ctx in dataGUIDs.Values) { _levelContexts.Remove(ctx); }
        }

        private void SaveData()
        {
            string ctxAsJson = EditorJsonUtility.ToJson(_data);
            EditorPrefs.SetString(EditorPrefsKey, ctxAsJson);
        }

        private void LoadData()
        {
            _data = new();
            if (EditorPrefs.HasKey(EditorPrefsKey))
            {
                string ctxAsJson = EditorPrefs.GetString(EditorPrefsKey);
                EditorJsonUtility.FromJsonOverwrite(ctxAsJson, _data);

                // Assign the correct asset instance since ScriptableObject can't be serialized
                for (int i = _levelContexts.Count - 1; i >= 0; i--)
                {
                    string path = AssetDatabase.GUIDToAssetPath(_levelContexts[i].GUID);
                    _levelContexts[i].Level = AssetDatabase.LoadAssetAtPath<LevelSO>(path);

                    Debug.Log(_levelContexts[i].Level.name + $" is null: {_levelContexts[i].Level == null}");
                    if (_levelContexts[i].Level == null) _levelContexts.RemoveAt(i);
                }
            }
            else
            {
                _levelContexts = new();
            }

            RefreshData();
        }

        [Serializable]
        private class LevelContext
        {
            public bool IsVisible;
            public bool IsPersistent;
            public string GUID;

            [NonSerialized]
            public LevelSO Level;
        }

        [Serializable]
        private class Data
        {
            public List<LevelContext> LevelContexts;
            public InteractingType InteractBehaviourType;
            public bool AdditiveToggle;
        }
    }
}