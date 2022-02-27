using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SoraCore.Manager {
    public class GameObjectManager : MonoBehaviour {
        public Dictionary<string, ObjectPool<GameObject>> PrefabPools = new();
        // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
        public Dictionary<string, Transform> PoolsTransformParent = new();
#endif

        [Separator("Listening to")]
        [SerializeField] private GameObjectManagerEventChannelSO _goManagerEC;

        private void OnEnable() {
            _goManagerEC.InstantiateRequested += Get;
            _goManagerEC.DestroyRequested += Release;
        }

        private void OnDisable() {
            _goManagerEC.InstantiateRequested -= Get;
            _goManagerEC.DestroyRequested -= Release;
        }

        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="pd"/>
        /// </summary>
        public GameObject Get(PrefabSO pd) {
            // Return an object from the pool if marked as poolable
            if (TryCreatePool(pd))
            {
                // Return an object from the pool
                GameObject gObj = PrefabPools[pd.AssetGuid].Get();

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                gObj.transform.parent = PoolsTransformParent[pd.AssetGuid];
#endif

                gObj.SetActive(true);
                return gObj;
            }
            return Instantiate(pd.prefab);
        }


        /// <summary>
        /// Destroy <paramref name="gObj"/> or release it to the <paramref name="pd"/> pool
        /// </summary>
        public void Release(GameObject gObj, PrefabSO pd) {
            if (TryCreatePool(pd))
            {
                PrefabPools[pd.AssetGuid].Release(gObj);
                return;
            }

            Destroy(gObj);
        }

        /// <summary>
        /// Return true if marked as poolable
        /// </summary>
        private bool TryCreatePool(PrefabSO pd) {
            if (pd.enablePooling)
            {
                // If key not found then..
                if (!PrefabPools.ContainsKey(pd.AssetGuid))
                {
                    // ..create pool
                    var pool = new ObjectPool<GameObject>(() => Instantiate(pd.prefab),
                                                          pd.OnGameObjGet,
                                                          pd.OnGameObjRelease,
                                                          pd.OnGameObjDestroy,
                                                          true,
                                                          pd.preload,
                                                          pd.capacity);
                    // Add newly created pool to the list
                    PrefabPools.Add(pd.AssetGuid, pool);

                    // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                    Transform poolTransformParent = new GameObject(pd.prefab.name).transform;
                    poolTransformParent.parent = transform;
                    PoolsTransformParent.Add(pd.AssetGuid, poolTransformParent);
#endif
                }

                return true;
            }

            return false;
        }
    }
}