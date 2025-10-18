using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class MonoBehaviourX {
        public static T AddComponent<T>(this MonoBehaviour mono) where T : Component => mono.gameObject.AddComponent<T>();
    }
}