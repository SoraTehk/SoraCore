namespace SoraCore.Manager {
    using MyBox;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MenuUIController : MonoBehaviour, IUIController {
        [SerializeField] LevelSO _ingameLevel;
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

        private Button _startButton;

        private void Awake() {
            _startButton = Document.rootVisualElement.Q<Button>("start-button");
            _startButton.clicked += () =>
            {
                UIManager.ShowScreen(UIType.Menu, false);
                LevelManager.LoadLevel(_ingameLevel, true, true, true);
            };
        }

        public void ShowUI(bool value) {
            Document.rootVisualElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            Document.rootVisualElement.pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}
