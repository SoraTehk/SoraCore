namespace SoraCore.Manager {
    using UnityEngine;
    public class IngameManager : MonoBehaviour {
        private void Start() {
            UIManager.ShowScreen(UIType.Gameplay);
        }
    }
}
