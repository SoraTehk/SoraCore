using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Serialization
{
    using MyBox;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;

    public class DataManager : SoraManager<DataManager>
    {
        #region Static -------------------------------------------------------------------------------------------------------
        public static void Save(string fileName) => GetInstance().InnerSave(fileName);
        public static void Load(string fileName) => GetInstance().InnerLoad(fileName);
        #endregion

        [field: SerializeField, ReadOnly] public string SaveFolderPath { get; private set; }

        private void Awake()
        {
            SaveFolderPath = Application.persistentDataPath + "/saves/";
        }

        // REFACTOR: Look messy
        #region SaveableController, ISaveable
        public void InnerSave(string fileName)
        {
            // Don't fully override save because we might loaded on different scene
            SaveData saveData = SaveData.Current ?? new SaveData();
            SaveStates(ref saveData);
            WriteFile(fileName, saveData);
        }
        public void InnerLoad(string fileName)
        {
            SaveData saveData = ReadFile(fileName);
            LoadStates(saveData);
            SaveData.Current = saveData;
        }

        private bool WriteFile(string fileName, SaveData saveData)
        {
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);

            // Writing data to file
            using (FileStream stream = File.Create(SaveFolderPath + fileName))
            {
                BinaryFormatter formatter = GetBinaryFormatter();
                formatter.Serialize(stream, saveData);
            }

            return true;
        }
        private SaveData ReadFile(string fileName)
        {
            string saveFilePath = SaveFolderPath + fileName;

            if (!File.Exists(saveFilePath))
            {
                SoraCore.LogError($"File not existed at {saveFilePath}", nameof(DataManager));
                return null;
            }

            // Reading data from file
            try
            {
                using (FileStream stream = File.Open(saveFilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = GetBinaryFormatter();
                    return (SaveData)formatter.Deserialize(stream);
                }

            }
            catch (Exception e)
            {
                SoraCore.LogError($"Failed to load file at {saveFilePath}\n{e.GetType()}", nameof(DataManager));
                return null;
            }
        }
        public static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new();

            SurrogateSelector selector = new();

            Vector3SerializationSurrogate vector3Surrogate = new();
            QuaternionSerializationSurrogate quaternionSurrogate = new();

            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

            formatter.SurrogateSelector = selector;

            return formatter;
        }

        private void SaveStates(ref SaveData saveData)
        {
            // Clear previous runtime data (only for loaded scenes)
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                if (saveData.SceneAssetPathToStateSet.ContainsKey(scene.path)) saveData.SceneAssetPathToStateSet[scene.path].Clear();
            }

            // Loop & start saving
            foreach (var saveable in FindObjectsOfType<SaveableController>())
            {
                // Runtime
                if (saveable.IsRuntimeInstantiate)
                {
                    // Create new set if not already existed
                    if (!saveData.SceneAssetPathToStateSet.ContainsKey(saveable.gameObject.scene.path))
                    {
                        saveData.SceneAssetPathToStateSet[saveable.gameObject.scene.path] = new HashSet<RuntimeStateData>();
                    }

                    // Add data to the related HashSet
                    saveData.SceneAssetPathToStateSet[saveable.gameObject.scene.path].Add(new RuntimeStateData
                    {
                        AssetGUID = saveable.AssetGUID,
                        TypeToData = saveable.SaveStates()
                    });

                    continue;
                }

                // Build-time
                saveData.GUIDToState[saveable.GUID] = new StateData
                {
                    Scene = saveable.gameObject.scene,
                    TypeToData = saveable.SaveStates()
                };
            }
        }
        private void LoadStates(SaveData saveData)
        {
            // Runtime
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                if (!saveData.SceneAssetPathToStateSet.TryGetValue(scene.path, out HashSet<RuntimeStateData> stateDataSet)) continue;

                foreach (var stateData in stateDataSet)
                {
                    // Try to find & load the conresponding asset prefab data
                    var pdRef = new AssetReferenceT<BlueprintAsset>(stateData.AssetGUID);

                    var op = pdRef.LoadAssetAsync();
                    op.WaitForCompletion();

                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        // Instantiate
                        var gameObj = InstantiateManager.Get(op.Result);
                        //SceneManager.MoveGameObjectToScene(gameObj, scene);
                        SaveableController saveable = gameObj.GetComponent<SaveableController>();
                        saveable.LoadStates(stateData.TypeToData);
                    }
                    else
                    {
                        SoraCore.LogWarning($"Invalid assetGUID ({stateData.AssetGUID}) while trying to load", nameof(DataManager));
                    }
                }
            }

            // Build-time
            foreach (var saveable in FindObjectsOfType<SaveableController>())
            {
                if (saveData.GUIDToState.TryGetValue(saveable.GUID, out StateData stateData))
                {
                    if (!stateData.Scene.isLoaded)
                    {
                        Debug.LogWarning($"{saveable.gameObject.name} (GUID: {saveable.GUID}) was detected but was in different scene in the save file.");
                        continue;
                    }

                    saveable.LoadStates(stateData.TypeToData);
                }
            }
        }
        #endregion
    }
}