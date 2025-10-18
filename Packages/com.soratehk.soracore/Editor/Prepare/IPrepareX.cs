using System;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SoraTehk.Prepare {
    public static class IPrepareX {
        [MenuItem("Tools/SoraTehk/Prepare/PrepareProjectAssets")]
        public static void PrepareProjectAssets() {
            GUID[] prepareGuids = AssetDatabase.FindAssetGUIDs("t:SceneAsset t:GameObject t:ScriptableObject"); // All possible IPrepare assets
            int changeCount = 0;
            int skipCount = 0;
            
            try {
                for (int i = 0; i < prepareGuids.Length; i++) {
                    var guid = prepareGuids[i];
                    var asset = AssetDatabase.LoadAssetByGUID<UObject>(guid);
                    if (asset == null) continue;
                    // Progress bar
                    if (i % 100 == 0) {
                        float percent = (float)i / prepareGuids.Length;
                        EditorUtility.DisplayProgressBar(
                            $"Preparing {percent:P1}",
                            $"Processing {i}/{prepareGuids.Length}: {asset.name}",
                            percent
                        );
                    }
                    // Immutable asset
                    if (!AssetDatabase.IsOpenForEdit(asset)) {
                        skipCount++;
                        continue;
                    }
                    // Args
                    Type mainType = AssetDatabase.GetMainAssetTypeFromGUID(guid);
                    // Process
                    try {
                        if (typeof(ScriptableObject).IsAssignableFrom(mainType)) {
                            if (asset is not IPrepare prepare) continue;
                            
                            if (prepare.Prepare()) {
                                EditorUtility.SetDirty(asset);
                                Debug.Log($"Prepared: {asset.name} (ScriptableObject)", asset);
                                changeCount++;
                            }
                        }
                        else if (typeof(GameObject).IsAssignableFrom(mainType)) {
                            var prefab = asset as GameObject;
                            if (prefab == null || !prefab.IsValidOrWarn()) continue;
                            
                            bool changed = false;
                            foreach (var prepare in prefab.GetComponentsInChildren<IPrepare>(true)) {
                                if (prepare.Prepare()) changed = true;
                            }
                            
                            if (changed) {
                                EditorUtility.SetDirty(prefab);
                                if (PrefabUtility.IsPartOfPrefabAsset(prefab)) {
                                    try {
                                        PrefabUtility.SavePrefabAsset(prefab);
                                        Debug.Log($"Prepared: {prefab.name} (Prefab)", asset);
                                        changeCount++;
                                    }
                                    catch (Exception ex) {
                                        Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}': {ex}");
                                        skipCount++;
                                    }
                                }
                            }
                        }
                        else if (typeof(SceneAsset).IsAssignableFrom(mainType)) {
                            try {
                                string path = AssetDatabase.GetAssetPath(asset);
                                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                                bool changed = false;
                                
                                foreach (var root in scene.GetRootGameObjects()) {
                                    foreach (var prepare in root.GetComponentsInChildren<IPrepare>(true)) {
                                        if (prepare.Prepare()) changed = true;
                                    }
                                }
                                
                                if (changed) {
                                    EditorSceneManager.MarkSceneDirty(scene);
                                    EditorSceneManager.SaveScene(scene);
                                    Debug.Log($"Prepared: {scene.name} (Prefab)", asset);
                                    changeCount++;
                                }
                            }
                            catch (Exception ex) {
                                Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}' (SceneAsset): {ex}");
                                skipCount++;
                            }
                        }
                    }
                    catch (Exception ex) {
                        Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}': {ex}");
                        skipCount++;
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Finished Preparing: Total={prepareGuids.Length}, Changed={changeCount}, Skipped={skipCount}");
        }
    }
}