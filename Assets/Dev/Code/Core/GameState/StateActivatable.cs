using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    [DefaultExecutionOrder(1)]
    public class StateActivatable : MonoBehaviour
    {
        [Inject] GameStateSystem _gameStateSystem;
        [Inject] GameplayStateSystem _gameplayStateSystem;
        [SerializeField] private List<GameState> _activeGameStates;
        [SerializeField] private List<GameplayState> _activeGameplayStates;

        private void Start()
        {
            if (_activeGameStates.Count > 0)
            {
                _gameStateSystem.OnStateChange += UpdateState;
            }
            if (_activeGameplayStates.Count > 0)
            {
                _gameplayStateSystem.OnStateChange += UpdateState;
            }
            UpdateState();
        }

        public void UpdateState()
        {
            gameObject.SetActive(CheckState());
        }

        private bool CheckState()
        {
            if (CheckState(_gameStateSystem, _activeGameStates) == false)
            {
                return false;
            }
            if (CheckState(_gameplayStateSystem, _activeGameplayStates) == false)
            {
                return false;
            }
            return true;
        }

        private bool CheckState<T>(StateSystemBase<T> stateHandler, List<T> states) where T : Enum
        {
            if (states.Count == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < states.Count; i++)
                {
                    if (stateHandler.CompareState(states[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}