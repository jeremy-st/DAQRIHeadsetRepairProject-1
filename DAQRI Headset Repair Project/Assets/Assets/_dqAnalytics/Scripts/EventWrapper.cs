using UnityEngine;
using UnityEngine.EventSystems;
using Amazon.MobileAnalytics.MobileAnalyticsManager;

public class EventWrapper : MonoBehaviour {
    
    [HideInInspector]
    public string objectName;
    [HideInInspector]
    public EventTrigger trigger;

    public void Start(){
        objectName = this.gameObject.name;
        trigger = GetOrCreateTrigger ();
        Init ();
    }

    public virtual void Init(){
        //name of the object will be a sortable attribute in analytics
    }

    public void RecordEvent(CustomEvent customEvent){
        //if we move away from AWS Mobile Analytics, we can change that here.
        AnalyticsManager.Get.RecordEvent(customEvent);
    }

    public EventTrigger GetOrCreateTrigger(){
        EventTrigger trigger = GetComponent<EventTrigger> ();
        if (trigger == null) {
            trigger = gameObject.AddComponent<EventTrigger> ();
        }
        return trigger;
    }

    public void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

}
