using System;
using UnityEngine;
using UnityEngine.InputSystem;
 namespace MMORPG.Tool
{
    public class DoubleClickDetector
    {
        public InputAction InputAction;
        public float DoubleClickTime;
        public event Action OnDoubleClick;

        private float _lastClickTime;

        public DoubleClickDetector(InputAction inputAction, Action onDoubleClick, float doubleClickTime = 0.5f, bool autoEnable = true)
        {
            InputAction = inputAction;
            DoubleClickTime = doubleClickTime;
            OnDoubleClick += onDoubleClick;
            if (autoEnable)
            {
                Enable();
            }
        }

        public void Enable()
        {
            InputAction.Enable();
            InputAction.started += OnInputStarted;
        }

        public void Disable()
        {
            InputAction.Disable();
            InputAction.started -= OnInputStarted;
        }

        private void OnInputStarted(InputAction.CallbackContext obj)
        {
            float timeSinceLastClick = Time.time - _lastClickTime;
            if (timeSinceLastClick <= DoubleClickTime)
            {
                OnDoubleClick?.Invoke();
            }
            _lastClickTime = Time.time;
        }
    }
}
