using System.Collections;
using UnityEngine;

namespace DAQRI.Turbine.Scripts
{
    public class AutoExit : MonoBehaviour
    {
        [SerializeField]
        private float timeOut = 20;

        // Use this for initialization
        IEnumerator Start ()
        {
            yield return new WaitForSeconds(timeOut);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
