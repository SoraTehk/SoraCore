using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Extensions {
    public static partial class SceneX {
        public static bool TryFindFirstComponent<T>(this Scene scene, out T? result, string gameObjName = "") where T : Component {
            result = null;
            bool checkName = !string.IsNullOrEmpty(gameObjName);
            
            var rootGObjs = scene.GetRootGameObjects();
            
            if (checkName) {
                foreach (var gObj in rootGObjs) {
                    // Check root
                    if (gObj.name == gameObjName) {
                        result = gObj.GetComponent<T>();
                        if (result != null) return true;
                    }
                    
                    // Check direct child by name
                    var childTransform = gObj.transform.Find(gameObjName);
                    if (childTransform == null) continue;
                    
                    result = childTransform.GetComponent<T>();
                    if (result != null) return true;
                }
            }
            else {
                foreach (var root in rootGObjs) {
                    result = root.GetComponentInChildren<T>(true);
                    if (result != null) return true;
                }
            }
            
            return false;
        }
    }
}