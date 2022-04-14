namespace SoraCore.Manager.Instantiate
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ReleaseCallback", menuName = "SoraCore/Instantiate Manager/Release Callback")]
    public class ReleaseCallbackAsset : BlueprintCallbackAsset
    {
        public override void Invoke(GameObject gObj) => gObj.SetActive(false);
    }
}

