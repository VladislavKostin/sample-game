using System.Collections;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Core
{
    public class InputSystem : ManagedSystem
    {
        private InputActions _inputActions;
        public InputActions.ActionsActions Actions => _inputActions.Actions;

        protected void Awake()
        {
            _inputActions = new InputActions();
            _inputActions.Enable();
            _inputActions.Actions.Enable();
        }

        public int GetCurrentNumberKeyDown()
        {
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown((KeyCode)(48 + i))) // 48 is the ASCII code for '0'
                {
                    return i;
                }
            }
            return -1; // Return -1 if no number key is pressed
        }
    }
}