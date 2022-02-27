namespace SoraCore.Manager {
    using UnityEngine;
    public class IngameManager : MonoBehaviour
    {
        private void Awake() {
            UIManager.ShowScreen(UIType.Gameplay);
        }
    }
}
