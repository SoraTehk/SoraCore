namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "GetCallbackData", menuName = "SoraCore/GameObject Manager/Get Callback Data")]
    public class GetCallbackSO : BlueprintCallbackSO {
        public override void Invoke(GameObject gObj) => gObj.SetActive(true);
    }
}

