using DAQRI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OBClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject gameobj;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }    

    public void OnPointerClick(PointerEventData eventData)
    {      
        //foreach (Transform child in gameobj.transform)
        //{
        //    Renderer rend = child.GetComponent<Renderer>();
        //    rend.material.shader = Shader.Find("Outlined/SilhouettedDiffuse");
        //}              
        EventSystem eventSystem = EventSystem.current;
        var RIM = (ReticleInputModule)eventSystem.gameObject.GetComponent(typeof(ReticleInputModule));
        GameObject tmp = GameObject.Find("Reticle Text");
        var ob = (TextDisplay)gameobj.GetComponent(typeof(TextDisplay));
       
        if (tmp == null)
        {            
            GameObject TextGO = new GameObject("Reticle Text");
            Text txt = TextGO.gameObject.AddComponent<Text>();
            txt.text = ob.Message;
            txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txt.fontSize = 20;
            txt.color = Color.black;
            txt.alignment = TextAnchor.MiddleCenter;
            TextGO.transform.SetParent(RIM.reticle.transform);
            TextGO.transform.localPosition = new Vector3(0f, -200f, -1.2f);
            TextGO.transform.localScale = new Vector3(1f, 1f, 1f);
            TextGO.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            RectTransform rt = TextGO.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(500, 30);
        }
        else
        {
            Text txt = tmp.GetComponent<Text>();
            txt.text = ob.Message;
        }
    }
}
