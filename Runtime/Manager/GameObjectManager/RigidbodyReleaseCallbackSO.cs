namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "RigidbodyReleaseCallbackData", menuName = "SoraCore/GameObject Manager/Rigidbody Release Callback Data")]
    public class RigidbodyReleaseCallbackSO : BlueprintCallbackSO {
        public override void Invoke(GameObject gObj)
        {
            gObj.SetActive(false);
            var rb = gObj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

