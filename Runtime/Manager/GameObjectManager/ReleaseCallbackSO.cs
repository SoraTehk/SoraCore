namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "ReleaseCallbackData", menuName = "SoraCore/GameObject Manager/Release Callback Data")]
    public class ReleaseCallbackSO : BlueprintCallbackSO {
        public override void Invoke(GameObject gObj) => gObj.SetActive(false);
    }
}

