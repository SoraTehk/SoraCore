namespace SoraCore.Manager {
    using UnityEngine;
    public interface ISaveable {
        GameObject gameObject { get; }
        object SaveState();
        void LoadState(object state);
    }
}