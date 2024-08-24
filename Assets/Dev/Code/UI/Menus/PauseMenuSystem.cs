using Game.Framework;
using Game.Core;

namespace Game.UI
{
    [BootstrapAfter(typeof(GameMenuSystem))]
    public class PauseMenuSystem : ManagedSystem
    {
        [Inject] private GameMenuSystem _gameMenuSystem;
        [Inject] private GameStateSystem _gameStateSystem;
        [Inject] private GameControllerSystem _gameControllerSystem;

        private VisualElementGroup _group;

        private void Start()
        {
            Setup();
            Hide();
            _gameStateSystem.SubscribeToStateEnter(GameState.PauseMenu, Show);
            _gameStateSystem.SubscribeToStateExit(GameState.PauseMenu, Hide);
        }

        private void Setup()
        {
            _group = new VisualElementGroup(_gameMenuSystem.GameMenu);
            _group.Add(_gameMenuSystem.CreateButton("Continue Game", OnContinueGameButton));
            _group.Add(_gameMenuSystem.CreateButton("Settings", OnSettingsButton));
            _group.Add(_gameMenuSystem.CreateButton("Exit to Desktop", OnExitToDesktopButton));
            _group.Add(_gameMenuSystem.CreateButton("Exit to Main Menu", OnExitToMainMenuButton));
        }

        public void Hide()
        {
            _group.Hide();
        }

        public void Show()
        {
            _group.Show();
        }

        public void OnContinueGameButton()
        {
            _gameControllerSystem.ContinueGame();
        }

        public void OnSettingsButton()
        {
            _gameControllerSystem.OpenSettingsMenu();
        }

        public void OnExitToDesktopButton()
        {
            _gameControllerSystem.ExitToDesktop();
        }

        public void OnExitToMainMenuButton()
        {
            _gameControllerSystem.GoToMainMenu();
        }
    }
}
