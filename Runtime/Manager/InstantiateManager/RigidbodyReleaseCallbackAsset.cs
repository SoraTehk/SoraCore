namespace SoraCore.Manager.Instantiate
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "RigidbodyReleaseCallback", menuName = "SoraCore/Instantiate Manager/Rigidbody Release Callback")]
    public class RigidbodyReleaseCallbackAsset : BlueprintCallbackAsset
    {
        public override void Invoke(GameObject gObj)
        {
            gObj.SetActive(false);
            var rb = gObj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

