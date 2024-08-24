using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Game.Framework;
using Game.Core;

namespace Game.UI
{
    [BootstrapAfter(typeof(GameMenuSystem))]
    public class SettingsMenuSystem : ManagedSystem
    {
        [Inject] private GameStateSystem _gameStateSystem;
        [Inject] private SettingsSystem _settingsSystem;
        [Inject] private GameMenuSystem _gameMenuSystem;

        private List<SettingsCategory> _settingsCategories = new();
        private SettingsCategory _currentCategory;
        private VisualElementGroup _group;

        private void Start()
        {
            Setup();
            Hide();
            _gameStateSystem.SubscribeToState(GameState.SettingsMenu, Show, Hide);
        }

        private void Setup()
        {
            _group = new VisualElementGroup(_gameMenuSystem.GameMenu, _gameMenuSystem.GameMenuContentSection);
            GenerateSetingsCategories();
            _group.Add(_gameMenuSystem.CreateButton("Back", OnBackButton));
            SelectCategory(_settingsCategories[0]);
        }

        private void GenerateSetingsCategories()
        {
            foreach (var category in _settingsSystem.Settings.Categories)
            {
                var uiCategory = new SettingsCategory
                {
                    Category = category,
                    VisualElement = new VisualElement()
                };
                _gameMenuSystem.GameMenuContentContainer.Add(uiCategory.VisualElement);
                _settingsCategories.Add(uiCategory);
                uiCategory.VisualElement.Hide();
                _group.Add(_gameMenuSystem.CreateButton(uiCategory.Title, () => SelectCategory(uiCategory)));
                GenerateSettingUIElements(uiCategory);
            }
        }

        private void GenerateSettingUIElements(SettingsCategory category)
        {
            foreach (var setting in category.Category.Settings)
            {
                if (setting is SliderSetting sliderSetting)
                {
                    var uiElement = new SliderSettingUIElement();
                    uiElement.BindSetting(sliderSetting);
                    category.VisualElement.Add(uiElement);
                }
                else if (setting is ToggleSetting toggleSetting)
                {

                }
                else if (setting.GetType().IsGenericType && setting.GetType().GetGenericTypeDefinition() == typeof(DropdownSetting<>))
                {

                }
            }
        }

        private void SelectCategory(SettingsCategory category)
        {
            if (_currentCategory != null)
            {
                _currentCategory.VisualElement.Hide();
            }
            _currentCategory = category;
            _currentCategory.VisualElement.Show();
            _gameMenuSystem.SetContentTitle(category.Title);
            // Logger.LogDebug($"Selected settings category: #{category.Title}");
        }

        public void Hide()
        {
            _group.Hide();
        }

        public void Show()
        {
            // Logger.LogDebug($"Showing settings menu");
            _group.Show();
            _gameMenuSystem.SetMenuTitle("Settings");
        }

        public void OnBackButton()
        {
            _gameStateSystem.GoToPreviousState();
        }

        public class SettingsCategory
        {
            public SettingsSystem.SettingsCategory Category;
            public VisualElement VisualElement;
            public string Title { get => Category.Name; }
        }
    }
}