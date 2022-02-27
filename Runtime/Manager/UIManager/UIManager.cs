namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.UIElements;
    using System;
    using MyBox;

    public enum UIType {
        Menu,
        Load,
        Gameplay,
        Inventory
    }

    public class UIManager : MonoBehaviour
    {
        #region Static -------------------------------------------------------------------------------------------------------
        private static event Action<UIType, bool> _showScreenRequested;
        public static void ShowScreen(UIType type, bool value = true) {

            if (_showScreenRequested != null) {
                _showScreenRequested.Invoke(type, value);
            }
            else {
                Debug.LogWarning("ShowScreen(...) was requested but no UIManager picked it up");
            }
        }
        private static event Action<float, float> _updateLoadScreenRequested;
        public static void UpdateLoadScreen(float main, float sub) {
            if (_updateLoadScreenRequested != null) {
                _updateLoadScreenRequested.Invoke(main, sub);
            }
            else {
                Debug.LogWarning("ShowLoadScreen(...) was requested but no UIManager picked it up");
            }
        }
        #endregion

        [SerializeField, AutoProperty] private MenuUIController _menuUIController;
        [SerializeField, AutoProperty] private LoadingUIController _loadingUIController;
        [SerializeField, AutoProperty] private GameplayUIController _gameplayUIController;


        private void Awake() {
            _menuUIController.ShowUI(false);
            _loadingUIController.ShowUI(false);
            _gameplayUIController.ShowUI(false);
        }

        private void OnEnable() {
            _showScreenRequested += InnerShowScreen;
            _updateLoadScreenRequested += InnerUpdateLoadScreen;
        }

        private void OnDisable() {
            _showScreenRequested -= InnerShowScreen;
            _updateLoadScreenRequested -= InnerUpdateLoadScreen;
        }

        private void InnerShowScreen(UIType type, bool value) {
            switch(type) {
                case UIType.Menu:
                    _menuUIController.ShowUI(value);
                    break;
                case UIType.Load:
                    _loadingUIController.ShowUI(value);
                    break;
                case UIType.Gameplay:
                    _gameplayUIController.ShowUI(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }


    private void InnerUpdateLoadScreen(float main, float sub) {
            _loadingUIController.MainProgress = main;
            _loadingUIController.SubProgress = sub;
        }

    }
}
