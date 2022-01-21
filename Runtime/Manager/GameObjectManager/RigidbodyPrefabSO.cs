using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "RigidbodyPrefabData", menuName = "SoraCore/GameObject Manager/RigidbodyPrefabData")]
    public class RigidbodyPrefabSO : PrefabSO {
        #region ObjectPool<T> delegate methods
        public override void OnGameObjRelease(GameObject gObj) {
            base.OnGameObjRelease(gObj);
            var rb = gObj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        #endregion
    }
}

