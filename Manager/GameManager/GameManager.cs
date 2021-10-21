using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using static UnityEngine.SceneManagement.SceneManager;

namespace Sora.Manager
{
    public enum GameStates {
        Playing,
        Pausing,
        Loading
    }
    [CreateAssetMenu(fileName = "GameManager", menuName = "Sora/Manager/GameManager")]
    public class GameManager : SingletonScriptableObject<GameManager> {
        #region GameState
        public int targetFPS = 920;
        public GameStates gameState;
        private float _resumingTimeScale = 1f;

        //public void PauseGame(bool pause) => (pause ? new Action(() => PauseGame()) : () => ResumeGame())();
        
        public static void PauseGame() {
            if(SharedInstance.gameState != GameStates.Playing) return;

            //Saving timescale for resuming
            SharedInstance._resumingTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            //Audio
            AudioListener.pause = true;

            SharedInstance.gameState = GameStates.Pausing;
        }
        public static void ResumeGame() {
            if(SharedInstance.gameState != GameStates.Pausing) return;

            //Resuming timescale
            Time.timeScale = SharedInstance._resumingTimeScale;
            //Audio
            AudioListener.pause = false;

            SharedInstance.gameState = GameStates.Playing;
        }
        #endregion

        #region LoadScene

        public bool isLoading;
        //public PrefabDataSO loadingCanvasPrefab;
        public SceneDataSO PersistentScene; //contain scene asset, path, name, buildindex reference
        public static bool PersistentSceneLoaded;
        public static bool LoadPersistentScene(LoadSceneMode loadSceneMode = LoadSceneMode.Additive) {
            //TODO: Race condition over here
            
            if(!GetSceneByBuildIndex(SharedInstance.PersistentScene.BuildIndex).isLoaded && !PersistentSceneLoaded) {
                PersistentSceneLoaded = true;
                SceneManager.LoadScene(SharedInstance.PersistentScene.BuildIndex, loadSceneMode);
                #region EDITOR
#if UNITY_EDITOR
                if(SharedInstance.debugMode) Debug.Log(SORA_MANAGER_LOG + ": PersistentScene loaded");
#endif
                #endregion
                return true;
            }

            //This mean the persistent scene already loaded
            return false;
        }
        public List<AsyncOperation> sceneOperations = new List<AsyncOperation>();
        //TODO: Rework this
        /// <summary>
        /// Load the target scene in single mode
        /// </summary>
        public static async void LoadScene(SceneDataSO sceneData) {
            List<AsyncOperation> s_SceneOperations = SharedInstance.sceneOperations;
            #region EDITOR
#if UNITY_EDITOR
            bool thisDebugMode = SharedInstance.debugMode;
#endif
            #endregion
            //Despawn all pooled objects in loaded scenes first
            PrefabManager.DespawnAll();

            //This is the first time persistent scene so no need to unload because of LoadSceneMode.Single
            if(!LoadPersistentScene(LoadSceneMode.Single)) {
                //Unload active scene
                AsyncOperation currentScene = UnloadSceneAsync(GetActiveScene());
                s_SceneOperations.Add(currentScene);
                #region EDITOR
#if UNITY_EDITOR
                if(thisDebugMode) Debug.Log(SORA_MANAGER_LOG + ": Unloading (async) active scene");
#endif
                #endregion
            }
            //Load target scene
            AsyncOperation targetScene = LoadSceneAsync(sceneData.BuildIndex, LoadSceneMode.Additive);
            targetScene.allowSceneActivation = false;
            s_SceneOperations.Add(targetScene);
            #region EDITOR
#if UNITY_EDITOR
            if(thisDebugMode) Debug.Log(SORA_MANAGER_LOG + ": Loading (async) target scene");
#endif
            #endregion
            //Show loading interface
            UIManager.ShowLoadingUI();
            UIManager.TransitionClear();

            //Wait for all operation to finish
            while(s_SceneOperations.Count > 0) {
                float totalProgress = 0;
                int operationCount = s_SceneOperations.Count;

                //Reverse for loop
                for(int i = operationCount - 1; i >= 0; i--) {
                    //If the scene operation is not done loading
                    if(s_SceneOperations[i].progress < .9f) {
                        totalProgress += s_SceneOperations[i].progress / operationCount;
                        UIManager.UpdateProgressBar(totalProgress);
                    } else {
                        s_SceneOperations.Remove(s_SceneOperations[i]);
                    }

                    await Task.Yield();
                }    
            }

            //Activating the target scene
            targetScene.allowSceneActivation = true;
            while(true) {
                if(targetScene.isDone) {
                    break;
                }
                await Task.Yield();
            }
            
            //Set active scene to the newest one
            SetActiveScene(GetSceneByBuildIndex(sceneData.BuildIndex));
            #region EDITOR
#if UNITY_EDITOR
            if(thisDebugMode) Debug.Log(SORA_MANAGER_LOG + "<b>" + typeName + "</b>: Finishing loading target scene");
#endif
            #endregion
            
            //Preload scene pools
            foreach(var prefabData in sceneData.PreloadPrefabList) {
                PrefabManager.Preload(prefabData);
            }

            //Hide loading interface
            UIManager.HideLoadingUI();
        }

        //public static void LoadScene(SceneDataSO sceneName) => LoadScene(sceneName.BuildIndex);
        public static void LoadScene(int buildIndex) {
            SceneManager.LoadScene(buildIndex);
            ResumeGame();
        }
        public static void LoadNextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        #endregion
    }
}
