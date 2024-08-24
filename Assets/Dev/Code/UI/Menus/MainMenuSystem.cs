using Game.Core;
using Game.Framework;

namespace Game.UI
{
    [BootstrapAfter(typeof(GameMenuSystem))]
    public class MainMenuSystem : ManagedSystem
    {
        [Inject] private GameMenuSystem _gameMenuSystem;
        [Inject] private GameStateSystem _gameStateSystem;
        [Inject] private UIDocumentProvider _uiDocumentProvider;
        [Inject] private GameControllerSystem _gameControllerSystem;

        private VisualElementGroup _group;

        private void Start()
        {
            Setup();
            Hide();
            _gameStateSystem.SubscribeToState(GameState.MainMenu, Show, Hide);
        }

        private void Setup()
        {
            _group = new VisualElementGroup(_gameMenuSystem.GameMenu);
            _group.Add(_gameMenuSystem.CreateButton("New Game", OnNewGameButton));
            _group.Add(_gameMenuSystem.CreateButton("Settings", OnSettingsButton));
            _group.Add(_gameMenuSystem.CreateButton("Exit to Desktop", OnExitToDesktopButton));
        }

        public void Hide()
        {
            _group.Hide();
        }

        public void Show()
        {
            _group.Show();
            _gameMenuSystem.SetMenuTitle("Main Menu");
        }

        public void OnNewGameButton()
        {
            _gameControllerSystem.StartNewGame();
        }

        public void OnSettingsButton()
        {
            _gameControllerSystem.OpenSettingsMenu();
        }

        public void OnExitToDesktopButton()
        {
            _gameControllerSystem.ExitToDesktop();
        }
    }
}
