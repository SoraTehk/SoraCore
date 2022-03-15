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
        public static string EditorPrefsKey { get; private set; }

        private Data _data;

        private void RefreshData()
        {
            Dictionary<string, LevelContext> dataGUIDs = _levelContexts.ToDictionary(ctx => ctx.GUID);
            string[] projectGUIDs = EditorHelper.FindAssetsGUIDOfType<LevelSO>();

            foreach (string guid in projectGUIDs)
            {
                
                if (dataGUIDs.TryGetValue(guid, out LevelContext ctx))
                {
                    // Remove if null (changed or deleted in the project)
                    if (!ctx.Level) dataGUIDs.Remove(guid);
                }
                else
                {
                    // Only add if not contained
                    _levelContexts.Add(new LevelContext()
                    {
                        GUID = guid,
                        Level = AssetDatabase.LoadAssetAtPath<LevelSO>(AssetDatabase.GUIDToAssetPath(guid))
                    });
                }
            }
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
            }
            else
            {
                _levelContexts = new();
            }


            RefreshData();

            // Assign the correct LevelSO since ScriptableObject can't be serialized
            for (int i = 0; i < _levelContexts.Count; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(_levelContexts[i].GUID);
                _levelContexts[i].Level = AssetDatabase.LoadAssetAtPath<LevelSO>(path);
            }
        }

        [Serializable]
        private class LevelContext
        {
            public bool Visible;
            public int Order;
            public string GUID;

            [NonSerialized]
            public LevelSO Level;
        }

        [Serializable]
        private class Data
        {
            public List<LevelContext> _levelContexts;
        }
    }
}