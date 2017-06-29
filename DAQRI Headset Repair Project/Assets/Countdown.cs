using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Countdown : MonoBehaviour, IPointerClickHandler
{

    public float CountdownFrom;
    public Text textbox;

    void Update()
    {
        float time = CountdownFrom - Time.timeSinceLevelLoad;
        textbox.text = "Time left: " + time.ToString("0.00") + "s";

        if (time <= 0f)
        {
            TimeUp();
        }
    }

    void TimeUp()
    {
        // this function is called when the timer runs out
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
