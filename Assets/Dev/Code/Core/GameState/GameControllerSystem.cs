using UnityEngine;
using Game.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Core
{
    [BootstrapOrder(1)] // Run after systems subscribe to state changes. 
    public class GameControllerSystem : ManagedSystem
    {
        [Inject] private GameStateSystem _gameStateSystem;
        [Inject] private GameplayStateSystem _gameplayStateSystem;

        private void Start()
        {
            GoToMainMenu();
        }

        public void StartNewGame()
        {
            _gameplayStateSystem.SetState(GameplayState.Production);
            _gameStateSystem.SetState(GameState.Gameplay);
            // Reducted.
        }

        public void ContinueGame()
        {
            _gameStateSystem.SetState(GameState.Gameplay);
        }

        public void SaveGame()
        {
            // Reducted.
        }

        public void LoadGame()
        {
            // Reducted.
        }

        public void GoToMainMenu()
        {
            _gameStateSystem.SetState(GameState.MainMenu);
        }

        public void OpenPauseMenu()
        {
            _gameStateSystem.SetState(GameState.PauseMenu);
        }

        public void TogglePauseMenu()
        {
            if (_gameStateSystem.CompareState(GameState.Gameplay))
            {
                _gameStateSystem.SetState(GameState.PauseMenu);
            }
            else if (_gameStateSystem.CompareState(GameState.PauseMenu))
            {
                _gameStateSystem.SetState(GameState.Gameplay);
            }
        }

        public void ExitToDesktop()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        public void OpenSettingsMenu()
        {
            _gameStateSystem.SetState(GameState.SettingsMenu);
        }
    }
}
