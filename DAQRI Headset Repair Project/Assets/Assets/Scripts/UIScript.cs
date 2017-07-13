using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {

    public Button StartAnimation;
    public Button ReverseAnimation;
    public Button Quit;
    public Button Move;
    public List<Animator> AnimotorList;
    public GameObject TimerCanvas;
    public float letterPause = 0.2f;
    private IEnumerator coroutine;
    string message = "";
    // Use this for initialization
    void Start () {    
        
        StartAnimation.onClick.AddListener(StartAnimationClick);
        ReverseAnimation.onClick.AddListener(ReverseAnimationClick);
        Quit.onClick.AddListener(QuitApp);
        Move.onClick.AddListener(MoveOB);
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
    void MoveOB()
    {
        coroutine = TypeText();
        StartCoroutine(coroutine);
        TimerCanvas.SetActive(true);
    }
    private IEnumerator TypeText()
    {
        var ob = (Text)TimerCanvas.transform.Find("Panel").gameObject.transform.Find("TimerText").gameObject.GetComponent(typeof(Text)); ;
        Debug.Log(ob);
        message = "321 ";
        ob.text = "";
        foreach (char letter in message.ToCharArray())
        {
            ob.text = letter.ToString();
            yield return new WaitForSeconds(1);
        }
        TimerCanvas.SetActive(false);
    }
}
