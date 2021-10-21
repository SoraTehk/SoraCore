using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sora {
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public GameEvent gameEvent;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent responseEvent;

        private void OnEnable() => gameEvent.Register(this);
        private void OnDisable() => gameEvent.Unregister(this);

        public void RaiseEvent() => responseEvent.Invoke();
    }
}
