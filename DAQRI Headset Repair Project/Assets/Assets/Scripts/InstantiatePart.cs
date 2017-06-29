using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePart : MonoBehaviour {

    public GameObject Enable_Disable;
    public void EnableorDisable()
    {
        if (Enable_Disable.active)
        {
            Enable_Disable.SetActive(false);
        }
        else
        {
            Enable_Disable.SetActive(true);
        }
    }
}
