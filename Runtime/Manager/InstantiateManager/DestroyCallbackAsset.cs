namespace SoraCore.Manager.Instantiate
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DestroyCallback", menuName = "SoraCore/Instantiate Manager/Destroy Callback")]
    public class DestroyCallbackAsset : BlueprintCallbackAsset
    {
        public override void Invoke(GameObject gObj) => Destroy(gObj);
    }
}

