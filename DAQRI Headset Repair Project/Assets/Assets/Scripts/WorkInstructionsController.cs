using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkInstructionsController : MonoBehaviour {
    void Start()
    {
        
    }    

     public void Quit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}
