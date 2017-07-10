using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {

    public Button StartAnimation;
    public Button ReverseAnimation;
    public Button Quit;
    public List<Animator> AnimotorList;
    // Use this for initialization
    void Start () {    
        
        StartAnimation.onClick.AddListener(StartAnimationClick);
        ReverseAnimation.onClick.AddListener(ReverseAnimationClick);
        Quit.onClick.AddListener(QuitApp);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Hi2");
    }
    void StartAnimationClick()
    {    foreach(Animator ani in AnimotorList)
        {
            ani.SetBool("Start", true);
        }       
    }
    void ReverseAnimationClick()
    {
        foreach (Animator ani in AnimotorList)
        {
            ani.SetBool("Start", false);
        }
    }
    void QuitApp()
    {
        Application.Quit();
    }
}
