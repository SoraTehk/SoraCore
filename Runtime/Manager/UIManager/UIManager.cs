namespace SoraCore.Manager {
    using MyBox;
    using System;
    using UnityEngine;
    using System.Collections.Generic;

    public enum UIType {
        Menu,
        Load,
        Gameplay,
        Inventory
    }

    public class UIManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------
        private static Action<UIType, bool> _showScreenRequested;
        public static void ShowScreen(UIType type, bool value = true) {

            if (_showScreenRequested != null) {
                _showScreenRequested.Invoke(type, value);
                return;
            }

            LogWarningForEvent(nameof(UIManager));
        }
        private static Action<float, float> _updateLoadScreenRequested;
        public static void UpdateLoadScreen(float main, float sub) {
            if (_updateLoadScreenRequested != null) {
                _updateLoadScreenRequested.Invoke(main, sub);
                return;
            }

            LogWarningForEvent(nameof(UIManager));
        }
        #endregion

        [SerializeField] private List<GameObject> _uis = new();

        private void Awake() {
            foreach (GameObject ui in _uis) {
                ui.SetActive(false);
            }
        }

        private void OnEnable() {
            _showScreenRequested += InnerShowScreen;
            _updateLoadScreenRequested += InnerUpdateLoadScreen;
        }

        private void OnDisable() {
            _showScreenRequested -= InnerShowScreen;
            _updateLoadScreenRequested -= InnerUpdateLoadScreen;
        }

        private void InnerShowScreen(UIType type, bool show) {
            int targetIndex = (int)type;

            if (targetIndex < 0 || targetIndex >= _uis.Count) {
                SoraCore.LogWarning($"Undefined value of {type}", nameof(UIManager));
                return;
            }

            for (int i = 0; i < _uis.Count; i++) {
                if (i == targetIndex) {
                    _uis[i].SetActive(show);
                }
                else {
                    _uis[i].SetActive(false);
                }
            }
        }

        private void InnerUpdateLoadScreen(float main, float sub) {
            //_loadingUIController.MainProgress = main;
            //_loadingUIController.SubProgress = sub;
        }
    }
}
