using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Amazon.MobileAnalytics.MobileAnalyticsManager;

public class GazeEvent : EventWrapper {

    public DateTime gaze_start_time;
    public DateTime gaze_end_time;

    public override void Init () {

        AddEventTriggerListener(
            trigger,
            EventTriggerType.PointerEnter,
            StartGaze);

        AddEventTriggerListener(
            trigger,
            EventTriggerType.PointerExit,
            RecordGaze);
        
    }

    void StartGaze( BaseEventData data )
    {
        gaze_start_time = DateTime.Now;
    }
        
    void OnApplicationFocus(bool focus) {
        if (!focus && gaze_start_time != default(DateTime)) {
            RecordGaze (null);
        }
    }

    void OnApplicationQuit() {
        if (gaze_start_time != default(DateTime)) {
            RecordGaze (null);
        }
    }

    void RecordGaze(BaseEventData data){
        PointerEventData pointerEventData = (PointerEventData)data;

        //determine how long the focus was on that object
        gaze_end_time = DateTime.Now;
        long elapsedTicks = gaze_end_time.Ticks - gaze_start_time.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        gaze_start_time = default(DateTime);
        gaze_end_time = default(DateTime);

        //send to analytics
        CustomEvent customEvent = new CustomEvent("gaze");
        customEvent.AddAttribute("Object", objectName);
        customEvent.AddMetric("TimeGazed", elapsedSpan.TotalSeconds);
        RecordEvent(customEvent);
    }
}
