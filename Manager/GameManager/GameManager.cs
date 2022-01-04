using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using MyBox;

namespace SoraCore.Manager {
    public class GameManager : MonoBehaviour {
        public enum GameState {
            Play,
            Pause,
            Load
        }
        [Range(1, 600)]
        public int TargetFPS = 600;
        [field: SerializeField]
        public GameState CurrentGameState { get; private set; }
        [field: SerializeField]
        public float CurrentTimeScale { get; private set; } = 1f;

        [Separator("Listening to")]
        // Event channels to Listening
        [SerializeField] private ChangeGameStateEventChannelSO _changeGameStateEC;
        [SerializeField] private LoadSceneEventChannelSO _loadSceneEC;

        // Parameters from scene loading requests
        private bool _isLoading;
        private SceneSO _currentlyLoadedScene;
        private SceneSO _sceneToLoad;
        private bool _showLoadingScrene;
        private bool _fadeScreen;

        private void OnEnable() {
            _loadSceneEC.Requested += LoadScene;
            _changeGameStateEC.Requested += ChangeGameState;
        }

        private void OnDisable() {
            _loadSceneEC.Requested -= LoadScene;
            _changeGameStateEC.Requested -= ChangeGameState;

        }



        public void ChangeGameState(GameState newGameState) {
            if (CurrentGameState == newGameState) return;

            switch(newGameState)
            {
                case GameState.Pause: PauseGame(); break;
                case GameState.Play or GameState.Load: ResumeGame(); break;
                default: throw new ArgumentOutOfRangeException(nameof(newGameState), $"Not expected GameState value: {newGameState}");
            }

            CurrentGameState = newGameState;
        }

        private void PauseGame() {
            // Store current timescale for resuming
            CurrentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        private void ResumeGame() {
            //Resuming timescale
            Time.timeScale = CurrentTimeScale;
            AudioListener.pause = false;
        }



        public void LoadScene(SceneSO sd, bool showLoadingScreen = false, bool fadeScreen = false) {
            // Prevent race condition
            if (_isLoading) return;
            _isLoading = true;

            _sceneToLoad = sd;
            _showLoadingScrene = showLoadingScreen;
            _fadeScreen = fadeScreen;

            UnloadPreviousScene();
            LoadNewScene();
        }

        private void UnloadPreviousScene() {
            // TODO: Adjust player input when loading
            // TODO: Handle fadeScreen request

            if(_currentlyLoadedScene != null)
            {
                if (_currentlyLoadedScene.sceneReference.OperationHandle.IsValid())
                {
                    // Unload the scene through its AssetReference (Addressable)
                    _currentlyLoadedScene.sceneReference.UnLoadScene();
                }
#if UNITY_EDITOR
                else
                {
                    // After a "cold start", the AsyncOperationHandle has not been used (the scene was already open in the editor),
                    // the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                    SceneManager.UnloadSceneAsync(_currentlyLoadedScene.sceneReference.editorAsset.name);
                }
#endif
            }
        }

        private void LoadNewScene() {
            // TODO: Show loading progress

            _sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0).Completed += OnNewSceneLoaded;
        }

        private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj) {
            _currentlyLoadedScene = _sceneToLoad;

            _isLoading = false;
            // TODO: Hide loading progress (finished)
            // TODO: Handle fadeScreen request
        }
    }
}