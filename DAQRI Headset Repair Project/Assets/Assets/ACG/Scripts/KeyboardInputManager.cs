using System;
using System.Collections.Generic;
using DAQRI.ACG.Scripts.Utilities;
using UnityEngine;

namespace DAQRI.ACG.Scripts
{
    public class KeyboardInputManager : MonoSingleton<KeyboardInputManager>
    {
        public struct InputCombinationArgs
        {
            public bool ControlPressed;
            public bool AltPressed;
            public bool ShiftPressed;
            public KeyCode MainKeyCode;
        }

        [SerializeField]
        private List<KeyCode> activeKeyCodes = new List<KeyCode>();
        [SerializeField]
        private bool controlPressed;
        [SerializeField]
        private bool altPressed;
        [SerializeField]
        private bool shiftPressed;

        public static event Action<InputCombinationArgs> KeyboardInputActivated = input => {};

        public static void RegisterKeyCodes(KeyCode key)
        {
            if (instance.activeKeyCodes.Contains(key))
            {
                Debug.LogError(string.Format("{0} Key already registered. Choose a different one.",key));
                return;
            }

            instance.activeKeyCodes.Add(key);
        }

        public static void RegisterKeyCodes(List<KeyCode> keys)
        {

            for (int i = 0; i < keys.Count; i++)
            {
                if (instance.activeKeyCodes.Contains(keys[i]))
                {
                    Debug.LogError(string.Format("{0} Key already registered. Choose a different one.",keys[i]));
                    continue;
                }
                instance.activeKeyCodes.Add(keys[i]);
            }
        }



        void Update ()
        {
            controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) ||
                        Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand);

            altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            for (int i = 0; i < activeKeyCodes.Count; i++)
            {
                if (Input.GetKeyDown(activeKeyCodes[i]))
                {
                    KeyboardInputActivated(new InputCombinationArgs() { ControlPressed = controlPressed, AltPressed = altPressed, ShiftPressed = shiftPressed, MainKeyCode = activeKeyCodes[i] });
                }
            }
        }
    }
}
