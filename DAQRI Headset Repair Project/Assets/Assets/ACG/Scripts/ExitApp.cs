using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAQRI.ACG.Scripts
{
    public class ExitApp : MonoBehaviour
    {
        [SerializeField]
        private KeyCode exitKey = KeyCode.Backspace;

        [SerializeField]
        private KeyCode disableAutoExit = KeyCode.E;

        [SerializeField]
        private float exitTimeout = 10f;

        private Coroutine currentAutoShutdown = null;

        private void Start()
        {
            KeyboardInputManager.KeyboardInputActivated += OnKeyboardInputActivated;
            KeyboardInputManager.RegisterKeyCodes(new List<KeyCode>() {exitKey,disableAutoExit});
            currentAutoShutdown = StartCoroutine(AutoShutdown());
        }

        private void OnKeyboardInputActivated(KeyboardInputManager.InputCombinationArgs inputCombinationArgs)
        {
            if (inputCombinationArgs.MainKeyCode == exitKey)
            {
                QuitApp();
            }
            if (inputCombinationArgs.MainKeyCode == disableAutoExit)
            {
                StopAutoShutdown();
            }
        }

        public void StopAutoShutdown()
        {

            if (currentAutoShutdown == null)
            {
                return;
            }
            StopCoroutine(currentAutoShutdown);
            currentAutoShutdown = null;
            Debug.Log("Auto Shutdown Disengaged.");
        }

        private IEnumerator AutoShutdown()
        {
            yield return new WaitForSeconds(exitTimeout);
            QuitApp();
        }

        private void QuitApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            Debug.Log("Quiting Application");
        }

        private void OnDestroy()
        {
            KeyboardInputManager.KeyboardInputActivated -= OnKeyboardInputActivated;
        }
    }
}
