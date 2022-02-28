namespace SoraCore.Manager {
    using UnityEngine;
    public class MenuManager : MonoBehaviour {
        private void Awake() {
            UIManager.ShowScreen(UIType.Menu);
        }
    }
}
