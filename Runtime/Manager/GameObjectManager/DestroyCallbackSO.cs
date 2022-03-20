namespace SoraCore.Manager {
    using UnityEngine;

    [CreateAssetMenu(fileName = "DestroyCallbackData", menuName = "SoraCore/GameObject Manager/Destroy Callback Data")]
    public class DestroyCallbackSO : BlueprintCallbackSO {
        public override void Invoke(GameObject gObj) => Destroy(gObj);
    }
}

