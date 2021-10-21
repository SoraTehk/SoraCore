using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Sora.Constant;

namespace Sora.Manager {
    public abstract class SingletonScriptableObject<T> : ScriptableObject
                                              where T  : ScriptableObject {

        //Currently only ManagerSO inheriting from this so I will put these here
        public bool debugMode;
        #region EDITOR
#if UNITY_EDITOR
        public static readonly string SORA_MANAGER_LOG = Sora.Constant.SORA_LOG + "<b>" + typeName + "</b>";
#endif
        #endregion
        public static readonly string  typeName = typeof(T).Name;
        
        //Scriptable object singleton of Awake() & Destroy()
        public virtual void Initialize() {}
        //TODO: Undestand how OnDestroy(), OnDisable() work on SO
        public virtual void OnDisable() {
            if(this == SharedInstance) Terminate();}
        public virtual void Terminate() {}

        private static readonly object _lock = new object();  
        private static T _sharedInstance;
        public  static T SharedInstance
        {
            get {
                lock(_lock) {
                    if(!_sharedInstance) {
                        //Try to find first
                        T[] objects = Resources.FindObjectsOfTypeAll<T>();
                        if(objects.Length > 0) {
                            _sharedInstance = objects[0];
                            #region EDITOR
#if UNITY_EDITOR
                            if(objects.Length != 1) {
                                var sb = Util.StringBuilderPool.GetFromPool();
                                sb.Append(SORA_WARNING).Append(": ");
                                sb.Append("More than 1 <b>").Append(typeName);
                                sb.Append("</b> was found, this supposes to be a singleton scriptable object!");
                                Debug.LogWarning(sb, _sharedInstance);
                                Util.StringBuilderPool.AddToPool(sb);
                            }
#endif
                            #endregion
                        }

                        //If not available then create one
                        if(!_sharedInstance) {
                            _sharedInstance = ScriptableObject.CreateInstance<T>();
                            #region EDITOR
#if UNITY_EDITOR
                            var sb = Util.StringBuilderPool.GetFromPool();
                            sb.Append(SORA_WARNING).Append(": ");
                            sb.Append("Cant find any <b>").Append(typeName);
                            sb.Append("</b>, proceed to create one (in memory)");
                            Debug.LogWarning(sb);
                            Util.StringBuilderPool.AddToPool(sb);
#endif
                            #endregion
                        }

                        //Initializing cause its the first time loaded
                        (_sharedInstance as SingletonScriptableObject<T>).Initialize();
                    }
                    
                    return _sharedInstance;
                }
            }
            set {_sharedInstance = value;}
        }
    }
}

