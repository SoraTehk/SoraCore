namespace SoraCore.EditorTools
{
    using Manager.Level;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;

    public partial class LevelSelectorWindow
    {
        public static string EditorPrefsKey { get; private set; } // OnEnable();

        private Data m_Data;

        private void RefreshData()
        {
            Dictionary<string, LevelContext> dataGUIDs = m_LevelContexts.ToDictionary(ctx => ctx.GUID);
            string[] projectGUIDs = EditorHelper.FindAssetsGUIDOfType<LevelAsset>();

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
                m_LevelContexts.Add(new LevelContext()
                {
                    GUID = guid,
                    Level = AssetDatabase.LoadAssetAtPath<LevelAsset>(AssetDatabase.GUIDToAssetPath(guid))
                });
            }

            // Remove non matching GUIDs (null/deleted/changed)
            foreach (LevelContext ctx in dataGUIDs.Values) { m_LevelContexts.Remove(ctx); }
        }

        private void SaveData()
        {
            string ctxAsJson = EditorJsonUtility.ToJson(m_Data);
            EditorPrefs.SetString(EditorPrefsKey, ctxAsJson);
        }

        private void LoadData()
        {
            m_Data = new();
            if (EditorPrefs.HasKey(EditorPrefsKey))
            {
                string ctxAsJson = EditorPrefs.GetString(EditorPrefsKey);
                EditorJsonUtility.FromJsonOverwrite(ctxAsJson, m_Data);

                // Assign the correct asset instance since ScriptableObject can't be serialized
                for (int i = m_LevelContexts.Count - 1; i >= 0; i--)
                {
                    string path = AssetDatabase.GUIDToAssetPath(m_LevelContexts[i].GUID);
                    m_LevelContexts[i].Level = AssetDatabase.LoadAssetAtPath<LevelAsset>(path);

                    if (m_LevelContexts[i].Level == null) m_LevelContexts.RemoveAt(i);
                }
            }
            else
            {
                m_LevelContexts = new();
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
            public LevelAsset Level;
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