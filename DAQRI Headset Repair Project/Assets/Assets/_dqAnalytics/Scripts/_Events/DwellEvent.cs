using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Amazon.MobileAnalytics.MobileAnalyticsManager;

public class DwellEvent : EventWrapper {

    public override void Init () {

        AddEventTriggerListener(
            trigger,
            EventTriggerType.PointerClick,
            RecordDwell);

    }

    void RecordDwell( BaseEventData data )
    {
        PointerEventData pointerEventData = (PointerEventData)data;
        //record this duration in AWS
        CustomEvent customEvent = new CustomEvent("dwell");
        customEvent.AddAttribute("Object", objectName);
        //AnalyticsManager.Get.RecordEvent(customEvent);
    }

}
