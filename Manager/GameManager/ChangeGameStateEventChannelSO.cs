using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SoraCore.Manager {
    using static GameManager;

    [CreateAssetMenu(fileName = "EC_ChangeGameState", menuName = "SoraCore/Game Manager/Change Game State EC")]
    public class ChangeGameStateEventChannelSO : ScriptableObject {

        public event UnityAction<GameState> Requested;

        public void Raise(GameState gameState) {
            if (Requested != null)
            {
                Requested.Invoke(gameState);
            }
            else
            {
                Debug.LogWarning("A game state changing event was requested, but no GameManager picked it up.");
            }
        }
    }
}
