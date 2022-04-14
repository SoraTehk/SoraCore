using MyBox;
using SoraCore.Manager.Level;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIController : MonoBehaviour
{
    [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

    [SerializeField] private LevelAsset _ingameLevel;
    [SerializeField] private Button _startButton;

    private void OnEnable()
    {
        _startButton = Document.rootVisualElement.Q<Button>("start-button");
        _startButton.clicked += OnStartButtonClicked;
    }

    private void OnDisable()
    {
        _startButton.clicked -= OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        LevelManager.LoadLevel(_ingameLevel, true, true, true);
    }
}

