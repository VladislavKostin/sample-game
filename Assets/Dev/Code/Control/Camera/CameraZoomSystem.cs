using System.Collections;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    public class CameraZoomSystem : ManagedSystem
    {
        [Inject] InputSystem _inputProviderSystem;
        [Inject] private MainCamera _mainCamera;

        [SerializeField] private int _defaultZoomStep = 2;
        [SerializeField] private float _maxOrthographicSize;
        [SerializeField] private float _minOrthographicSize;
        [SerializeField] private float _zoomPerStep = 0.2f;

        private List<float> _zoomSteps = new List<float>();
        private int _currentZoomStep;

        public void Start()
        {
            if (_zoomPerStep > 0 && _minOrthographicSize > 0)
            {
                var currentZoom = _minOrthographicSize;
                _zoomSteps.Add(currentZoom);
                while (currentZoom < _maxOrthographicSize)
                {
                    currentZoom = currentZoom + currentZoom * _zoomPerStep;
                    _zoomSteps.Add(currentZoom);
                }
            }
            SetZoomLevel(_defaultZoomStep);
        }

        public void Update()
        {
            var scroll = _inputProviderSystem.Actions.Zoom.ReadValue<float>();
            if (scroll > 0.25f)
            {
                ZoomIn();
            }
            else if (scroll < -0.25f)
            {
                ZoomOut();
            }
        }

        public void SetZoomLevel(int value)
        {
            if (value > 0 && value < _zoomSteps.Count)
            {
                _currentZoomStep = value;
                _mainCamera.Camera.orthographicSize = _zoomSteps[_currentZoomStep];
            }
        }

        public void ZoomIn()
        {
            var newZoom = _currentZoomStep - 1;
            if (newZoom >= 0)
            {
                SetZoomLevel(newZoom);
            }
        }

        public void ZoomOut()
        {
            var newZoom = _currentZoomStep + 1;
            if (newZoom < _zoomSteps.Count)
            {
                SetZoomLevel(newZoom);
            }
        }
    }
}