namespace SoraCore.Manager {
    using UnityEngine;

    public abstract class BlueprintCallbackSO : ScriptableObject {
        public abstract void Invoke(GameObject gObj);
    }
}

