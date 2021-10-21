using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Manager
{
    [CreateAssetMenu(fileName = "UIManager", menuName = "Sora/Manager/UIManager")]
    public class UIManager: SingletonScriptableObject<UIManager> {
        private static object assignLock = new object();

        #region Transition UI
        public PrefabDataSO transitionCanvasPrefab;
        private TransitionCanvas _transitionCanvas;
        public TransitionCanvas TransitionCanvas {
            get {
                lock(assignLock) {
                    if (!_transitionCanvas) {
                        //Loading in the persistent scene first
                        GameManager.LoadPersistentScene();

                        // Search for existing instance.
                        _transitionCanvas = FindObjectOfType<TransitionCanvas>();

                        // Create new instance if one doesn't already exist.
                        if (_transitionCanvas == null) {
                            _transitionCanvas = PrefabManager.Spawn(transitionCanvasPrefab, ManagerBehaviour.SharedInstance.transform)
                                                .GetComponent<TransitionCanvas>();
                        }           
                    }

                    return _transitionCanvas;
                }
            }
        }
        
        /// <summary>
        /// Fade in transition (this will make screen black forever, remember to release this)
        /// </summary>
        public static void TransitionFadeIn(float t) {
            SharedInstance.TransitionCanvas.TransitionFadeIn(t);
#region EDITOR
#if UNITY_EDITOR
            if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": TransitionStart()");
#endif
#endregion
        }
        /// <summary>
        /// Fade out transition
        /// </summary>
        public static void TransitionFadeOut(float t) {
            SharedInstance.TransitionCanvas.TransitionFadeOut(t);
#region EDITOR
#if UNITY_EDITOR
            if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": TransitionEnd()");
#endif
#endregion
        }
        public static void TransitionClear() {
            SharedInstance.TransitionCanvas.ShowTransitionCanvas(0);
#region EDITOR
#if UNITY_EDITOR
            if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": TransitionClear()");
#endif
#endregion
        }
        #endregion
        

        #region Loading UI
        public PrefabDataSO loadingCanvasPrefab;
        public int progressBarChildIndex = -1;
        private LoadingCanvas _loadingCanvas;
        public LoadingCanvas LoadingCanvas {
            get {
                lock(assignLock) {
                    if (!_loadingCanvas) {
                        //Loading in the persistent scene first
                        GameManager.LoadPersistentScene();

                        // Search for existing instance.
                        _loadingCanvas = FindObjectOfType<LoadingCanvas>();

                        // Create new instance if one doesn't already exist.
                        if (_loadingCanvas == null) {
                            _loadingCanvas = PrefabManager.Spawn(loadingCanvasPrefab, ManagerBehaviour.SharedInstance.transform)
                                            .GetComponent<LoadingCanvas>();
                        }           
                    }

                    return _loadingCanvas;
                }
            }
        }
        public static void ShowLoadingUI() => SharedInstance.LoadingCanvas.gameObject.SetActive(true);
        public static void HideLoadingUI() => SharedInstance.LoadingCanvas.gameObject.SetActive(false);
        public static void UpdateProgressBar(float value) => SharedInstance.LoadingCanvas.ProgressBarImage.fillAmount = value;
        #endregion

        #region Logging UI
        public static void Log() {
            
        }

        #endregion
    }
}
