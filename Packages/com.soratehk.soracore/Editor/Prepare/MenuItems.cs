using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Prepare {
    public static class MyBoxIPrepareX {
        [MenuItem("Tools/SoraTehk/Prepare/Scene", priority = 1)]
        public static void PrepareScene() {
            var prepareList = new List<(UObject obj, IPrepare prepare)>();
            
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                var gObjs = scene.GetRootGameObjects();
                foreach (var gObj in gObjs) {
                    var prepares = gObj.GetComponentsInChildren<IPrepare>(true);
                    foreach (var prepare in prepares) {
                        prepareList.Add(((prepare as UObject)!, prepare));
                    }
                }
            }
            
            foreach (var item in prepareList) {
                bool changed = item.prepare.Prepare();
                if (!changed) continue;
                
                EditorUtility.SetDirty(item.obj);
                Debug.Log(item.obj.name + "." + item.prepare.GetType().Name + ": Changed on Prepare", item.obj);
            }
        }
        [MenuItem("Tools/SoraTehk/Prepare/Project", priority = 2)]
        public static void PrepareProjectAssets() {
            GUID[] guids = AssetDatabase.FindAssetGUIDs("t:ScriptableObject");
            var prepareList = new List<(UObject asset, IPrepare prepare)>();
            foreach (GUID guid in guids) {
                var asset = AssetDatabase.LoadAssetByGUID<ScriptableObject>(guid);
                if (asset is not IPrepare prepare) continue;
                
                prepareList.Add((asset, prepare));
            }
            
            GUID[] prefabGuids = AssetDatabase.FindAssetGUIDs("t:Prefab");
            foreach (var guid in prefabGuids) {
                var prefab = AssetDatabase.LoadAssetByGUID<GameObject>(guid);
                if (prefab == null) continue;
                
                var prepares = prefab.GetComponentsInChildren<IPrepare>(true);
                foreach (var prepare in prepares) {
                    prepareList.Add((prefab, prepare));
                }
            }
            
            foreach (var prepare in prepareList) {
                bool changed = prepare.prepare.Prepare();
                if (!changed) continue;
                
                EditorUtility.SetDirty(prepare.asset);
                Debug.Log(prepare.asset.name + "." + prepare.asset.GetType().Name + ": Changed on Prepare", prepare.asset);
            }
        }
    }
}