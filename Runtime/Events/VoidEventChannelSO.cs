using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EC_Void", menuName = "SoraCore/Events/Void")]
public class VoidEventChannelSO : ScriptableObject {
    public event UnityAction EventRequested;
    public void Raise() {
        if (EventRequested != null) {
            EventRequested.Invoke();
        }
        else {
            Debug.LogWarning("An <b>void event</b> was requested, but nobody picked it up");
        }
    }
}
