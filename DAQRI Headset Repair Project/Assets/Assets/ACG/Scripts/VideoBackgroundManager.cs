using System;
using DAQRI.ACG.Scripts.Utilities;
using UnityEngine;

namespace DAQRI.ACG.Scripts
{
    public class VideoBackgroundManager : MonoSingleton<VideoBackgroundManager>
    {
        [SerializeField]
        private KeyCode colorKeyCode = KeyCode.C;
        [SerializeField]
        private KeyCode thermalKeyCode = KeyCode.T;
        [SerializeField]
        private KeyCode depthKeyCode = KeyCode.D;
        [SerializeField]
        private bool colorActive = false;
        [SerializeField]
        private bool thermalActive = false;
        [SerializeField]
        private bool depthActive = false;

        private void Start()
        {
            KeyboardInputManager.KeyboardInputActivated += OnKeyboardInputActivated;
            KeyboardInputManager.RegisterKeyCodes(colorKeyCode);
            KeyboardInputManager.RegisterKeyCodes(thermalKeyCode);
            KeyboardInputManager.RegisterKeyCodes(depthKeyCode);

            if(colorActive)
                ToggleColorBackground();
            if(thermalActive)
                ToggleThermalBackground();
            if(depthActive)
                ToggleDepthBackground();
        }

        private void OnKeyboardInputActivated(KeyboardInputManager.InputCombinationArgs inputCombinationArgs)
        {
            switch (inputCombinationArgs.MainKeyCode)
            {
                case KeyCode.C:
                    ToggleColorBackground();
                    break;
                case KeyCode.D:
                    ToggleDepthBackground();
                    break;
                case KeyCode.T:
                    ToggleThermalBackground();
                    break;
                default:
                    return;
            }
        }

        public void ToggleColorBackground()
        {
            colorActive = !colorActive;

            if (colorActive)
            {
                thermalActive = false;
                depthActive = false;
                DisplayManager.Instance.TurnVideoBackgroundOn();
            }
            else
            {
                DisplayManager.Instance.TurnVideoBackgroundOff();
            }
        }

        public void ToggleThermalBackground()
        {
            thermalActive = !thermalActive;

            if (thermalActive)
            {
                colorActive = false;
                depthActive = false;
                DisplayManager.Instance.TurnThermalBackgroundOn();
            }
            else
            {
                DisplayManager.Instance.TurnThermalBackgroundOff();
            }
        }

        public void ToggleDepthBackground()
        {
            depthActive = !depthActive;

            if (depthActive)
            {
                thermalActive = false;
                colorActive = false;
                DisplayManager.Instance.TurnDepthBackgroundOn();
            }
            else
            {
                DisplayManager.Instance.TurnDepthBackgroundOff();
            }
        }
    }
}
