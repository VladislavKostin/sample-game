using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    public class CameraMovement : ManagedSystem
    {
        [Inject] private InputSystem _inputSystem;
        [Inject] private MainCamera _playerCamera;

        [SerializeField] private float _moveSpeed;

        private void Update()
        {
            var direction = _inputSystem.Actions.PrimaryDirectionalInput.ReadValue<Vector2>();
            if (direction != Vector2.zero)
            {
                _playerCamera.transform.Translate(direction * _moveSpeed * Time.deltaTime * (_playerCamera.Camera.orthographicSize * 0.5f));
            }
        }
    }
}