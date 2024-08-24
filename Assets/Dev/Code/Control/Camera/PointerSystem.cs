using System.Collections;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    public class PointerSystem : ManagedSystem
    {
        [Inject] private InputSystem _inputProviderSystem;

        [SerializeField] private Transform _pointerTransform;
        [SerializeField] private Camera _mainCamera;
        private Vector3 _pointerWorldPosition;
        // private VectorXZ _pointerGridPosition;

        public void Update()
        {
            UpdatePointerPosition();
        }

        public void UpdatePointerPosition()
        {
            var pointerPosition = _inputProviderSystem.Actions.PointerPosition.ReadValue<Vector2>();
            var ray = _mainCamera.ScreenPointToRay(pointerPosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            plane.Raycast(ray, out var rayEnterDistance);
            _pointerWorldPosition = ray.GetPoint(rayEnterDistance);
            // _pointerGridPosition = new VectorXZ(_pointerWorldPosition);
        }

        public void UpdatePointerInteractionMenu()
        {

        }

        public void UpdatePointerInteraction()
        {
            if (_inputProviderSystem.Actions.Interaction1.IsPressed())
            {

            }
        }
    }
}
