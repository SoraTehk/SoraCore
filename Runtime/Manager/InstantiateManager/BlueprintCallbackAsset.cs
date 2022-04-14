namespace SoraCore.Manager.Instantiate
{
    using UnityEngine;

    public abstract class BlueprintCallbackAsset : ScriptableObject
    {
        public abstract void Invoke(GameObject gObj);
    }
}

