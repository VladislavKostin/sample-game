using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Game.Core;
using Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Framework;

namespace Game.Core
{
    public class MenuInputSystem : ManagedSystem
    {
        [Inject] InputSystem _inputProviderSystem;
        [Inject] GameControllerSystem _gameControllerSystem;

        private void Start()
        {
            _inputProviderSystem.Actions.Menu.performed += OnMenuAction;
            _inputProviderSystem.Actions.Cancel.performed += OnCancelAction;
        }

        public void OnCancelAction(InputAction.CallbackContext context)
        {
            _gameControllerSystem.TogglePauseMenu();
        }

        public void OnMenuAction(InputAction.CallbackContext context)
        {
            _gameControllerSystem.TogglePauseMenu();
        }
    }
}
