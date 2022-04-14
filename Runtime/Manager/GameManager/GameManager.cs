namespace SoraCore.Manager.Game
{
    using System;
    using UnityEngine;

    public enum GameState
    {
        Playing,
        Pausing,
        Loading
    }

    public class GameManager : SoraManager<GameManager>
    {
        #region Static -------------------------------------------------------------------------------------------------------
        public static void ChangeGameState(GameState gameState) => GetInstance().InnerChangeGameState(gameState);
        #endregion

        [field: Range(1, 600)]
        [field: SerializeField] public int TargetFPS { get; private set; } = 600;

        [field: SerializeField] public GameState CurrentGameState { get; private set; }
        [field: SerializeField] public float CurrentTimeScale { get; private set; } = 1f;

        #region GameState ----------------------------------------------------------------------------------------------------
        private void InnerChangeGameState(GameState newGameState)
        {
            if (CurrentGameState == newGameState) return;

            switch (newGameState)
            {
                case GameState.Pausing: PauseGame(); break;
                case GameState.Playing or GameState.Loading: ResumeGame(); break;
                default: throw new ArgumentOutOfRangeException(nameof(newGameState), $"Not expected GameState value: {newGameState}");
            }

            CurrentGameState = newGameState;
        }

        private void PauseGame()
        {
            // Store current timescale for resuming
            CurrentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        private void ResumeGame()
        {
            //Resuming timescale
            Time.timeScale = CurrentTimeScale;
            AudioListener.pause = false;
        }
        #endregion
    }
}