using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace Sora.Manager {
    public class ManagerBehaviour : PersistentMonoBehaviour<ManagerBehaviour> {
        [Foldout("References")][ReadOnly]public PrefabManager PrefabManager;
        [Foldout("References")][ReadOnly]public GameManager GameManager;
        [Foldout("References")][ReadOnly]public AudioManager AudioManager;
        [Foldout("References")][ReadOnly]public UIManager UIManager;
        [Foldout("References")][ReadOnly]public InputManager InputManager;

        #region MonoBehavior
        override public void Awake() {
            base.Awake();

            GameManager = GameManager.SharedInstance;
            AudioManager = AudioManager.SharedInstance;
            PrefabManager = PrefabManager.SharedInstance;
            UIManager = UIManager.SharedInstance;
            InputManager = InputManager.SharedInstance;

            InputManagerAwake();
        }
        private void Start() {
        }
        private void Update() {
            GameManagerUpdate();
        }
        #endregion

        #region GameManager
        [Header("Visualise")]
        public GameStates currentGameState;
        public int currentTargetFPS;
        public void GameManagerUpdate() {
            currentGameState = GameManager.gameState;
            Application.targetFrameRate = GameManager.targetFPS;
            currentTargetFPS = GameManager.targetFPS;
        }
        #endregion

        #region AudioManager
        #endregion

        #region UIManager
        #endregion

        #region InputManager
        private void InputManagerAwake() {
        }
        #endregion
    }
}

