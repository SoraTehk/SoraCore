namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;

    using TypeToDataDict = System.Collections.Generic.Dictionary<string, object>;

    [Serializable]
    public class SaveData {
        #region Static -------------------------------------------------------------------------------------------------------
        
        // Simple singleton
        private static readonly object _syncRoot = new();
        private static SaveData _current;
        public static SaveData Current
        {
            get
            {
                if (_current == null) {
                    lock (_syncRoot) {
                        if (_current == null) {
                            _current = new SaveData();
                        }
                    }
                }

                return _current;
            }
            set => _current = value;
        }

        #endregion

        public Dictionary<string, StateData> GUIDToState = new();
        public Dictionary<string, HashSet<RuntimeStateData>> SceneAssetPathToStateSet = new();
    }

    [Serializable]
    public struct StateData {
        public Scene Scene;
        public TypeToDataDict TypeToData;
    }

    [Serializable]
    public struct RuntimeStateData {
        public string AssetGUID;
        public TypeToDataDict TypeToData;
    }
}