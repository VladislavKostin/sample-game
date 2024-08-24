using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;

namespace Game.Core
{
    public abstract class StateSystemBase<T> : ManagedSystem where T : Enum
    {
        private T _currentState;
        private T _previousState;
        private Dictionary<T, Action> stateEnterSubscribers = new Dictionary<T, Action>();
        private Dictionary<T, Action> stateExitSubscribers = new Dictionary<T, Action>();

        public event Action OnStateChange;
        public T PreviousState => _previousState;
        public T CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (!EqualityComparer<T>.Default.Equals(_currentState, value))
                {
                    _previousState = _currentState;
                    _currentState = value;
                    Logger.LogImportant($"[{typeof(T).Name}] is set to [{_currentState}]");
                    OnStateChange?.Invoke();
                    NotifySubscribers(_previousState, _currentState);
                }
            }
        }

        public void SetState(T state)
        {
            CurrentState = state;
        }

        public bool CompareState(T state)
        {
            return _currentState.Equals(state);
        }

        public void SubscribeToStateEnter(T state, Action onStateEnter)
        {
            if (!stateEnterSubscribers.ContainsKey(state))
            {
                stateEnterSubscribers[state] = onStateEnter;
            }
            else
            {
                stateEnterSubscribers[state] += onStateEnter;
            }
        }

        public void UnsubscribeFromStateEnter(T state, Action onStateEnter)
        {
            if (stateEnterSubscribers.TryGetValue(state, out Action action))
            {
                action -= onStateEnter;
            }
        }

        public void SubscribeToStateExit(T state, Action onStateExit)
        {
            if (!stateExitSubscribers.ContainsKey(state))
            {
                stateExitSubscribers[state] = onStateExit;
            }
            else
            {
                stateExitSubscribers[state] += onStateExit;
            }
        }

        public void UnsubscribeFromStateExit(T state, Action onStateExit)
        {
            if (stateExitSubscribers.TryGetValue(state, out Action action))
            {
                action -= onStateExit;
            }
        }

        public void SubscribeToState(T state, Action onStateEnter, Action onStateExit)
        {
            SubscribeToStateEnter(state, onStateEnter);
            SubscribeToStateExit(state, onStateExit);
        }

        public void GoToPreviousState()
        {
            CurrentState = _previousState;
        }

        private void NotifySubscribers(T oldState, T newState)
        {
            if (stateExitSubscribers.TryGetValue(oldState, out Action exitSubscribers))
            {
                exitSubscribers?.Invoke();
            }

            if (stateEnterSubscribers.TryGetValue(newState, out Action enterSubscribers))
            {
                enterSubscribers?.Invoke();
            }
        }
    }
}
