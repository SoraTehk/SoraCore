namespace SoraCore.Manager.Instantiate
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Pool;

    public class InstantiateManager : SoraManager<InstantiateManager>
    {
        #region Static -------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="bd"/>.
        /// </summary>
        public static GameObject Get(BlueprintAsset bd) => GetInstance().InnerGet(bd);
        /// <summary>
        /// Release instance(s) of <paramref name="bd"/> pool.
        /// </summary>
        public static void Release(BlueprintAsset bd) => GetInstance().InnerRelease(bd);
        /// <summary>
        /// Release instance(s) of all pools.
        /// </summary>
        public static void ReleaseAll() => GetInstance().InnerReleaseAll();
        /// <summary>
        /// Release <paramref name="gObj"/> base on it <see cref="BlueprintAsset"/> settings.
        /// </summary>
        public static void ReleaseInstance(GameObject gObj) => GetInstance().InnerReleaseInstance(gObj);
        /// <summary>
        /// Preload instance(s) for <paramref name="bd"/> pool.
        /// </summary>
        public static void Preload(BlueprintAsset bd) => GetInstance().InnerPreload(bd);
        /// <summary>
        /// Destroy instance(s) of <paramref name="bd"/> pool.
        /// </summary>
        public static void Clear(BlueprintAsset bd) => GetInstance().InnerClear(bd);
        /// <summary>
        /// Destroy instance(s) and pool(s).
        /// </summary>
        public static void ClearAll() => GetInstance().InnerClearAll();
        /// <summary>
        /// Return all active blueprints
        /// </summary>
        public static List<BlueprintAsset> GetLoadedBlueprints() => GetInstance().InnerGetLoadedBlueprints();
        #endregion

        [field: SerializeField] public BlueprintCallbackAsset DefaultGetCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackAsset DefaultReleaseCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackAsset DefaultDestroyCallback { get; private set; }

        private readonly Dictionary<GameObject, BlueprintAsset> m_GameObjToBlueprint = new();
        private readonly Dictionary<BlueprintAsset, List<GameObject>> m_BlueprintToActiveGameObjs = new();
        private readonly Dictionary<BlueprintAsset, ObjectPool<GameObject>> m_BlueprintToPool = new();
#if UNITY_EDITOR
        // Only nested game object in hierarchy when in editor mode
        private readonly Dictionary<ObjectPool<GameObject>, Transform> m_PoolToParentTransform = new();
#endif

        private GameObject InnerGet(BlueprintAsset bd)
        {
            GameObject gObj;

            if (CreatePoolIfCan(bd))
            {
                // Return an object from the pool
                ObjectPool<GameObject> pool = m_BlueprintToPool[bd];
                gObj = pool.Get();

#if UNITY_EDITOR
                // Only nested game object in hierarchy when in editor mode
                gObj.transform.parent = m_PoolToParentTransform[pool];
#endif
            }
            else
            {
                // Instantiate new game object as it not marked as poolable
                gObj = Instantiate(bd.Prefab);

                // Save for later look up
                m_GameObjToBlueprint[gObj] = bd;

                // Invoke get callback
                (bd.GetCallback != null ? bd.GetCallback : DefaultGetCallback).Invoke(gObj);
            }

            if (!m_BlueprintToActiveGameObjs.ContainsKey(bd)) m_BlueprintToActiveGameObjs[bd] = new List<GameObject>();
            m_BlueprintToActiveGameObjs[bd].Add(gObj);

            return gObj;
        }

        private void InnerRelease(BlueprintAsset bd)
        {
            foreach (var gObj in m_BlueprintToActiveGameObjs[bd]) InnerReleaseInstance(gObj, false);
            m_BlueprintToActiveGameObjs[bd].Clear();
        }

        private void InnerReleaseAll()
        {
            foreach (var blueprint in InnerGetLoadedBlueprints()) InnerRelease(blueprint);
        }

        private void InnerReleaseInstance(GameObject gObj)
        {
            if (!gObj)
            {
                SoraCore.LogWarning($"You are trying to release null!", nameof(InstantiateManager));
                return;
            }

            InnerReleaseInstance(gObj, true);
        }
        private void InnerReleaseInstance(GameObject gObj, bool removeFromActiveList)
        {
            // Does this game object spawned by the system
            if (!m_GameObjToBlueprint.TryGetValue(gObj, out BlueprintAsset bd))
            {
                SoraCore.LogWarning($"You are trying to release an object which are not created by the system, no callback will be processed!", nameof(InstantiateManager), gObj);
                Destroy(gObj);
                return;
            }

            if (CreatePoolIfCan(bd))
            {
                m_BlueprintToPool[bd].Release(gObj);
            }
            else
            {
                // Callback
                (bd.ReleaseCallback != null ? bd.ReleaseCallback : DefaultReleaseCallback).Invoke(gObj);
                (bd.DestroyCallback != null ? bd.DestroyCallback : DefaultDestroyCallback).Invoke(gObj);
            }

            if (removeFromActiveList) m_BlueprintToActiveGameObjs[bd].Remove(gObj);
        }

        private void InnerPreload(BlueprintAsset bd)
        {
            if (!CreatePoolIfCan(bd)) return;
            if (m_BlueprintToPool[bd].CountAll >= bd.Preload) return;

            Debug.Log($"{bd.name} preloaded");


            // OPTIMIZABLE: InnerPreload()
            var gObjs = new GameObject[bd.Preload];
            for (int i = 0; i < bd.Preload; i++) gObjs[i] = InnerGet(bd);
            for (int i = 0; i < bd.Preload; i++) InnerReleaseInstance(gObjs[i]);
        }

        private void InnerClear(BlueprintAsset bd)
        {
            Debug.Log($"{bd.name} cleared");
            InnerRelease(bd);
            m_BlueprintToPool[bd].Clear();
        }

        private void InnerClearAll()
        {
            foreach (var blueprint in InnerGetLoadedBlueprints()) InnerClear(blueprint);
            m_GameObjToBlueprint.Clear();
        }

        private List<BlueprintAsset> InnerGetLoadedBlueprints() => m_GameObjToBlueprint.Values.Distinct().ToList();

        private bool CreatePoolIfCan(BlueprintAsset bd)
        {
            // Return false if not poolable
            if (!bd.EnablePooling) return false;

            // If key not found then..
            if (!m_BlueprintToPool.ContainsKey(bd))
            {
                // ..create pool
                var pool = new ObjectPool<GameObject>(() =>
                {
                    var gObj = Instantiate(bd.Prefab);

                    m_GameObjToBlueprint[gObj] = bd;

                    return gObj;
                },
                (bd.GetCallback != null ? bd.GetCallback : DefaultGetCallback).Invoke,
                (bd.ReleaseCallback != null ? bd.ReleaseCallback : DefaultReleaseCallback).Invoke,
                (bd.DestroyCallback != null ? bd.DestroyCallback : DefaultDestroyCallback).Invoke,
                true,
                bd.Preload,
                bd.Capacity);

                // Add newly created pool to the dict
                m_BlueprintToPool[bd] = pool;

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                Transform poolTransformParent = new GameObject(bd.name).transform;
                poolTransformParent.parent = transform;
                m_PoolToParentTransform[pool] = poolTransformParent;
#endif
            }

            return true;
        }
    }
}