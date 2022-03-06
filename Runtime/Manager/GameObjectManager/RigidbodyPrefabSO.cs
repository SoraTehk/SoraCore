namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "RigidbodyPrefabData", menuName = "SoraCore/GameObject Manager/Rigidbody Prefab Data")]
    public class RigidbodyPrefabSO : PrefabSO {
        #region ObjectPool<T> delegate methods
        //public virtual void OnGameObjGet(GameObject gObj) => gObj.SetActive(true);
        public override void OnGameObjRelease(GameObject gObj) {
            base.OnGameObjRelease(gObj); // gObj.SetActive(true);
            var rb = gObj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        //public virtual void OnGameObjDestroy(GameObject gObj) => Destroy(gObj);
        #endregion
    }
}

