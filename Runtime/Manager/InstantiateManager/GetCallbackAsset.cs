namespace SoraCore.Manager.Instantiate
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GetCallback", menuName = "SoraCore/Instantiate Manager/Get Callback")]
    public class GetCallbackAsset : BlueprintCallbackAsset
    {
        public override void Invoke(GameObject gObj) => gObj.SetActive(true);
    }
}

