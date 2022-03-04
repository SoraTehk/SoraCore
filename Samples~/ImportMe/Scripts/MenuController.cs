namespace SoraCore.Manager {
    using UnityEngine;
    public class MenuController : MonoBehaviour {

        private void Awake() {
            UIManager.ShowScreen(UIType.Menu);
        }
    }
}
