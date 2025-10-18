using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Tools {
    public static class SoraTehk {
        [MenuItem("Tools/SoraTehk/RefreshUPM")]
        public static async UniTask RefreshUnityPackageManager() {
            try {
                string dummyPackageId = "com.unity.nuget.newtonsoft-json";
                
                EditorUtility.DisplayProgressBar("Refreshing UPM", "Checking dummy package installation", 0.1f);
                var listReq = Client.List(true);
                await listReq.WaitForCompletion();
                
                bool dummyPackageInstalled = listReq.Result.Any(p => p.name == dummyPackageId);
                
                if (dummyPackageInstalled) {
                    EditorUtility.DisplayProgressBar("Refreshing UPM", "Removing dummy package", 0.3f);
                    var removeReq = Client.Remove(dummyPackageId);
                    await removeReq.WaitForCompletion();
                    
                    EditorUtility.DisplayProgressBar("Refreshing UPM", "Adding dummy package", 0.6f);
                    var addReq = Client.Add(dummyPackageId);
                    await addReq.WaitForCompletion();
                }
                else {
                    EditorUtility.DisplayProgressBar("Refreshing UPM", "Adding dummy package", 0.3f);
                    var addReq = Client.Add(dummyPackageId);
                    await addReq.WaitForCompletion();
                    
                    EditorUtility.DisplayProgressBar("Refreshing UPM", "Removing dummy package", 0.6f);
                    var removeReq = Client.Remove(dummyPackageId);
                    await removeReq.WaitForCompletion();
                }
                
                AssetDatabase.Refresh();
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
        
        [MenuItem("Tools/SoraTehk/SetDirtyAllAssets")]
        public static void SetDirtyAllAssets() {
            GUID[] allGuids = AssetDatabase.FindAssetGUIDs("t:Object"); // UnityEngine.Object
            int skipCount = 0;
            
            try {
                for (int i = 0; i < allGuids.Length; i++) {
                    var guid = allGuids[i];
                    var asset = AssetDatabase.LoadAssetByGUID<UObject>(guid);
                    if (asset == default) continue;
                    
                    if (i % 100 == 0) {
                        float percent = (float)i / allGuids.Length;
                        EditorUtility.DisplayProgressBar(
                            $"Set All Dirty {percent:P1}",
                            $"Processing {i}/{allGuids.Length}: {asset.name}",
                            percent
                        );
                    }
                    
                    if (!AssetDatabase.IsOpenForEdit(asset)) {
                        skipCount++;
                        continue;
                    }
                    
                    Type mainType = AssetDatabase.GetMainAssetTypeFromGUID(guid);
                    
                    if (mainType == typeof(GameObject)) {
                        if (asset is GameObject gObj && gObj.IsValidOrWarn()) {
                            EditorUtility.SetDirty(gObj);
                            if (PrefabUtility.IsPartOfPrefabAsset(gObj)) {
                                try {
                                    PrefabUtility.SavePrefabAsset(gObj);
                                }
                                catch (Exception ex) {
                                    Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(gObj)}' (Prefab): {ex}");
                                    skipCount++;
                                }
                            }
                        }
                    }
                    else if (mainType == typeof(SceneAsset)) {
                        try {
                            Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(asset), OpenSceneMode.Single);
                            foreach (var root in scene.GetRootGameObjects()) {
                                EditorUtility.SetDirty(root);
                            }
                            
                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                        }
                        catch (Exception ex) {
                            Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}' (SceneAsset): {ex}");
                            skipCount++;
                        }
                    }
                    else {
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Finished: Total={allGuids.Length} , Skipped={skipCount}");
        }
        
        [MenuItem("Tools/SoraTehk/DryBuild")]
        public static void DryBuild() {
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
            
            BuildTarget target = BuildTarget.WebGL;
            
            string platformFolder = target.ToString();
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string buildPath = Path.Combine(projectRoot, "Builds", ".Temp", platformFolder);
            Directory.CreateDirectory(buildPath);
            
            BuildPlayerOptions options = new BuildPlayerOptions {
                scenes = scenes,
                locationPathName = buildPath,
                target = target,
                options = BuildOptions.BuildScriptsOnly |
                          BuildOptions.Development |
                          BuildOptions.DetailedBuildReport |
                          BuildOptions.StrictMode
            };
            
            var originWebGlPlayerSettings = new {
                memorySize = PlayerSettings.WebGL.memorySize,
                exceptionSupport = PlayerSettings.WebGL.exceptionSupport,
                nameFilesAsHashes = PlayerSettings.WebGL.nameFilesAsHashes,
                showDiagnostics = PlayerSettings.WebGL.showDiagnostics,
                dataCaching = PlayerSettings.WebGL.dataCaching,
                debugSymbols = PlayerSettings.WebGL.debugSymbolMode,
                emscriptenArgs = PlayerSettings.WebGL.emscriptenArgs,
                modulesDirectory = PlayerSettings.WebGL.modulesDirectory,
                template = PlayerSettings.WebGL.template,
                analyzeBuildSize = PlayerSettings.WebGL.analyzeBuildSize,
                useEmbeddedResources = PlayerSettings.WebGL.useEmbeddedResources,
                compressionFormat = PlayerSettings.WebGL.compressionFormat,
                wasmArithmeticExceptions = PlayerSettings.WebGL.wasmArithmeticExceptions,
                linkerTarget = PlayerSettings.WebGL.linkerTarget,
                threadsSupport = PlayerSettings.WebGL.threadsSupport,
                decompressionFallback = PlayerSettings.WebGL.decompressionFallback,
                initialMemorySize = PlayerSettings.WebGL.initialMemorySize,
                maximumMemorySize = PlayerSettings.WebGL.maximumMemorySize,
                memoryGrowthMode = PlayerSettings.WebGL.memoryGrowthMode,
                memoryLinearGrowthStep = PlayerSettings.WebGL.linearMemoryGrowthStep,
                memoryGeometricGrowthStep = PlayerSettings.WebGL.geometricMemoryGrowthStep,
                memoryGeometricGrowthCap = PlayerSettings.WebGL.memoryGeometricGrowthCap,
                powerPreference = PlayerSettings.WebGL.powerPreference,
                webAssemblyTable = PlayerSettings.WebGL.webAssemblyTable,
                webAssemblyBigInt = PlayerSettings.WebGL.webAssemblyBigInt
            };
            
            try {
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
                PlayerSettings.WebGL.nameFilesAsHashes = false;
                PlayerSettings.WebGL.showDiagnostics = false;
                PlayerSettings.WebGL.dataCaching = false;
                PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                PlayerSettings.WebGL.threadsSupport = false;
                PlayerSettings.WebGL.decompressionFallback = false;
                
                var report = BuildPipeline.BuildPlayer(options);
                Debug.Log($"Finished: Result={report.summary.result}, Time={report.summary.totalTime}");
            }
            finally {
                PlayerSettings.WebGL.exceptionSupport = originWebGlPlayerSettings.exceptionSupport;
                PlayerSettings.WebGL.debugSymbolMode = originWebGlPlayerSettings.debugSymbols;
                PlayerSettings.WebGL.compressionFormat = originWebGlPlayerSettings.compressionFormat;
                PlayerSettings.WebGL.threadsSupport = originWebGlPlayerSettings.threadsSupport;
                PlayerSettings.WebGL.decompressionFallback = originWebGlPlayerSettings.decompressionFallback;
                PlayerSettings.WebGL.nameFilesAsHashes = originWebGlPlayerSettings.nameFilesAsHashes;
                PlayerSettings.WebGL.dataCaching = originWebGlPlayerSettings.dataCaching;
                PlayerSettings.WebGL.showDiagnostics = originWebGlPlayerSettings.showDiagnostics;
            }
        }
    }
}