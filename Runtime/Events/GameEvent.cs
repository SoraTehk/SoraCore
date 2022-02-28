using System.Collections.Generic;
using UnityEngine;


namespace SoraCore {
    [CreateAssetMenu(fileName = "GameEvent", menuName = "SoraCore/Events/GameEvent")]
    public class GameEvent : ScriptableObject {
        /// <summary>
        /// The list of listeners that this event will notify if it is 'Invoked'.
        /// </summary>
        private readonly HashSet<GameEventListener> _listeners = new();
        public void Register(GameEventListener listener) => _listeners.Add(listener);
        public void Unregister(GameEventListener listener) => _listeners.Remove(listener);

        public void Invoke() {
            foreach (var listener in _listeners) {
                listener.RaiseEvent();
            }
        }
    }
}
