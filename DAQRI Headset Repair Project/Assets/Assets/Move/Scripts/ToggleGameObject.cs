using UnityEngine;
using System.Collections;

public class ToggleGameObject : MonoBehaviour {

    public GameObject [] objectToToggle;

    // This toggle will be called with an animator event during the jet engine xray transition to enable/disable the regular engine
    public void Toggle ()
    {
        foreach(GameObject objectsToToggle in objectToToggle)
        {
            if(objectsToToggle.activeSelf)
                objectsToToggle.SetActive(false);
            else
                objectsToToggle.SetActive(true);
        }
    }
}
