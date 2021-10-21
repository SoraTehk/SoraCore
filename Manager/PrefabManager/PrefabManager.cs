using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

namespace Sora.Manager
{
    [CreateAssetMenu(fileName = "PrefabManager", menuName = "Sora/Manager/PrefabManager")]
    public class PrefabManager : SingletonScriptableObject<PrefabManager> {
        public Dictionary<string, LeanGameObjectPool> PrefabPools = new Dictionary<string, LeanGameObjectPool>();

        public static void Preload(PrefabDataSO prefabData) {
            if (!SharedInstance.PrefabPools.ContainsKey(prefabData.tag)) {
                var gameObj = new GameObject();
                gameObj.transform.parent = ManagerBehaviour.SharedInstance.transform;
                gameObj.name = "[Pool of]" + prefabData.tag;
                var pool = gameObj.AddComponent<LeanGameObjectPool>();
                prefabData.ApplyPoolConfigTo(pool);
                SharedInstance.PrefabPools.Add(prefabData.tag, pool);
            }

            SharedInstance.PrefabPools[prefabData.tag].PreloadAll();
        }
        
        public static void Spawn(PrefabDataSO prefabData)
            => Spawn(prefabData, Vector3.zero, Quaternion.identity);
        public static GameObject Spawn(PrefabDataSO prefabData, Transform parent)
            => Spawn(prefabData, Vector3.zero, Quaternion.identity, parent);
        public static GameObject Spawn(PrefabDataSO prefabData, Vector3 position, Quaternion rotation, Transform parent = null) {
            if(prefabData.enablePooling) {
                //Create pool if key not found
                if (!SharedInstance.PrefabPools.ContainsKey(prefabData.tag)) {
                    var gameObj = new GameObject();
                    gameObj.transform.parent = ManagerBehaviour.SharedInstance.transform;
                    gameObj.name = "[Pool of]" + prefabData.tag;
                    var pool = gameObj.AddComponent<LeanGameObjectPool>();
                    prefabData.ApplyPoolConfigTo(pool);
                    SharedInstance.PrefabPools.Add(prefabData.tag, pool);
                }

                return SharedInstance.PrefabPools[prefabData.tag].Spawn(position, rotation, parent);
            }
            
            return Instantiate(prefabData.prefab, position, rotation, parent);
        }

        public static void Despawn(Component component, float delay = 0f) => LeanPool.Despawn(component, delay);
        public static void DespawnAll() => LeanPool.DespawnAll();
    }
}
