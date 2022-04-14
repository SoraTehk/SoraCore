namespace SoraCore.Manager.Serialization
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using TypeToDataDict = System.Collections.Generic.Dictionary<string, object>;

    [Serializable]
    public class SaveData
    {
        #region Static -------------------------------------------------------------------------------------------------------
        // Simple singleton
        private static readonly object m_SyncRoot = new();
        private static SaveData m_Current;
        public static SaveData Current
        {
            get
            {
                if (m_Current == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_Current == null)
                        {
                            m_Current = new SaveData();
                        }
                    }
                }

                return m_Current;
            }
            set => m_Current = value;
        }
        #endregion

        public Dictionary<string, StateData> GUIDToState = new();
        public Dictionary<string, HashSet<RuntimeStateData>> SceneAssetPathToStateSet = new();
    }

    [Serializable]
    public struct StateData
    {
        public Scene Scene;
        public TypeToDataDict TypeToData;
    }

    [Serializable]
    public struct RuntimeStateData
    {
        public string AssetGUID;
        public TypeToDataDict TypeToData;
    }
}