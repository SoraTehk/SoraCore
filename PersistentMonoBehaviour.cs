using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Sora.Constant;

namespace Sora.Manager {
    /// <summary>
    /// Inherit from this base class to create a 'persistent' singleton (only have 1 instance in define Persistent scene).
    /// e.g. public class MyClassName : PersistentMonoBehaviour<MyClassName> {}
    /// </summary>
    public abstract class PersistentMonoBehaviour<T> : MonoBehaviour
                                            where T : MonoBehaviour
    {
        #region EDITOR
#if UNITY_EDITOR
        public static readonly string SORA_PERSISTER_LOG = Sora.Constant.SORA_LOG + "<b>" + typeName + "</b>";
        public static readonly string SORA_PERSISTER_WARNING = Sora.Constant.SORA_WARNING + "<b>" + typeName + "</b>";
#endif
        #endregion
        public  static readonly string typeName = typeof(T).Name;
        //public abstract Scene rootScene;
        // Check to see if we're about to be destroyed.
        private static bool _isShuttingDown = false;
        private static object _getterLock = new object();
        private static T _sharedInstance;
        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T SharedInstance
        {
            get {
                if (_isShuttingDown) {
                    Debug.LogWarning("[Singleton]Instance '" + typeName +
                        "' already destroyed. Returning null.");
                    return null;
                }
                lock (_getterLock) {
                    if (_sharedInstance == null) {
                        GameManager.LoadPersistentScene();
                        //Search for existing instance.
                        _sharedInstance = (T)FindObjectOfType(typeof(T));

                        //Create new instance if one doesn't already exist.
                        if (_sharedInstance == null) {                 
                            #region EDITOR
#if UNITY_EDITOR
                            Debug.Log(SORA_PERSISTER_WARNING + ": Cant find any available instance in the scene, proceed to create one");
#endif
                            #endregion
                            //Need to create a new GameObject to attach the singleton to.
                            var persistentObject = new GameObject();
                            _sharedInstance = persistentObject.AddComponent<T>();
                            persistentObject.name =  $"{typeName} ({SceneManager.GetActiveScene().name})";
                            SceneManager.MoveGameObjectToScene(persistentObject, SceneManager.GetSceneByBuildIndex(GameManager.SharedInstance.PersistentScene.BuildIndex));
                        }
                    }
    
                    return _sharedInstance;
                }
            }
        }

        virtual public void Awake() {
            //Singleton pattern
            if(_sharedInstance) {
#if UNITY_EDITOR
                Destroy(this, 10f);
                var sb = Util.StringBuilderPool.GetFromPool();
                sb.Append(SORA_PERSISTER_LOG).Append(": ");
                sb.Append("Destroy <b>").Append(typeName).Append("</b> component of <b>");
                sb.Append(gameObject).Append("<b> (after 10s) to preserve it singleton pattern");
                Debug.LogWarning(sb, gameObject);
                Util.StringBuilderPool.AddToPool(sb);
#else
                Destroy(this);
#endif
            }
        }

        virtual public void OnApplicationQuit() {
            _isShuttingDown = true;
        }
    
        virtual public void OnDestroy() {
            _isShuttingDown = true;
        }
    }
}
