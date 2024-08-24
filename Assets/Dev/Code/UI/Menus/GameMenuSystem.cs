using System;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Framework;
using Game.Core;

namespace Game.UI
{
    public class GameMenuSystem : ManagedSystem
    {
        [Inject] private UIDocumentProvider _uiDocumentProvider;

        [SerializeField] private VisualTreeAsset _buttonTemplate;

        public VisualElement GameMenu { get; private set; }
        public VisualElement GameMenuButtonsContainer { get; private set; }
        public VisualElement GameMenuContentSection { get; private set; }
        public VisualElement GameMenuContentContainer { get; private set; }
        public Label GameMenuTitle { get; private set; }
        public Label GameMenuContentTitle { get; private set; }

        private void Start()
        {
            Setup();
            GameMenuContentSection.Hide();
        }

        private void Setup()
        {
            var root = _uiDocumentProvider.UIDocument.rootVisualElement;
            GameMenu = root.FindRequired<VisualElement>("game-menu");
            GameMenuTitle = GameMenu.FindRequired<Label>("game-menu-title");
            GameMenuContentSection = GameMenu.FindRequired<VisualElement>("game-menu-content-section");
            GameMenuContentTitle = GameMenu.FindRequired<Label>("game-menu-content-title");
            GameMenuContentContainer = GameMenu.FindRequired<VisualElement>("game-menu-content-container");
            GameMenuButtonsContainer = GameMenu.FindRequired<VisualElement>("game-menu-buttons-container");
        }

        public VisualElement CreateButton(string title, Action action)
        {
            var buttonElement = new GameMenuButtonUIElement();
            buttonElement.Bind(title, action);
            GameMenuButtonsContainer.Add(buttonElement);
            return buttonElement;
        }

        public void Hide()
        {
            GameMenu.Hide();
            GameMenuContentSection.Hide();
        }

        public void Show()
        {
            GameMenu.Hide();
        }

        public void SetMenuTitle(string title)
        {
            GameMenuTitle.text = title;
        }

        public void SetContentTitle(string title)
        {
            GameMenuContentTitle.text = title;
        }
    }
}
