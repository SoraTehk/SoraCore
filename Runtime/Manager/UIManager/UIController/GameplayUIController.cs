namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.UIElements;
    using MyBox;

    public class GameplayUIController : MonoBehaviour, IUIController {
        [SerializeField] LevelSO _menuLevel;
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

        private Button _menuButton;

        private void Awake() {
            _menuButton = Document.rootVisualElement.Q<Button>("menu-button");
            _menuButton.clicked += () =>
            {
                UIManager.ShowScreen(UIType.Gameplay, false);
                GameManager.LoadLevel(_menuLevel, true, true, true);
            };
        }

        public void ShowUI(bool value) {
            Document.rootVisualElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            Document.rootVisualElement.pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}
