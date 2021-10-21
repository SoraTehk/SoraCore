using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Sora.Constant;

namespace Sora
{
    /// <summary>
    /// Inherit from this base class to create a 'scene' singleton (only have 1 instance per scene).
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
                                        where T  : MonoBehaviour 
    {
            #region EDITOR
#if UNITY_EDITOR
            public static readonly string SORA_SINGLETON_LOG = Sora.Constant.SORA_LOG + "<b>" + typeName + "</b>";
#endif
            #endregion
        public  static readonly string  typeName = typeof(T).Name;
        virtual public void Awake() {
            //Singleton pattern
            if(_instance) {
#if UNITY_EDITOR
                Destroy(this, 10f);
                var sb = Util.StringBuilderPool.GetFromPool();
                sb.Append(SORA_SINGLETON_LOG).Append(": ");
                sb.Append("Destroy <b>").Append(typeName).Append("</b> component of <b>");
                sb.Append(gameObject).Append("<b> (after 10s) to preserve it singleton pattern");
                Debug.LogWarning(sb, gameObject);
                Util.StringBuilderPool.AddToPool(sb);
#else
                Destroy(this);
#endif
            }
        }
        
        // Check to see if we're about to be destroyed.
        private static bool _isShuttingDown = false;
        virtual public void OnApplicationQuit() {
            _isShuttingDown = true;
        }
        virtual public void OnDestroy() {
            _isShuttingDown = true;
        }

        private static object assignLock = new object();
        private static T _instance;
        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T i
        {
            get {
                if (_isShuttingDown)
                {
                    Debug.LogWarning("[Singleton]Instance '" + typeName +
                        "' already destroyed. Returning null.");
                    return null;
                }
                lock (assignLock) {
                    if (_instance == null) {
                        //Search for existing instance.
                        _instance = (T)FindObjectOfType(typeof(T));
    
                        //Create new instance if one doesn't already exist.
                        if (_instance == null) {
                            //Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name =  $"{typeName} ({SceneManager.GetActiveScene().name})";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
    
                    return _instance;
                }
            }
        }

        
    }
}