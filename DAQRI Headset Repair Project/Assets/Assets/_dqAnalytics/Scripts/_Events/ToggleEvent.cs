using UnityEngine;
using System.Collections;
using Amazon.MobileAnalytics.MobileAnalyticsManager;

public class ToggleEvent : EventWrapper {


    public Animator animatorCtrl;

    [HideInInspector]
    public bool value;
    [HideInInspector]
    public AnimatorBoolSwitch animatorBoolSwitch;
    [HideInInspector]
    public string animatorBool;

    private bool valueNew;

    public override void Init () {

        if (animatorCtrl == null) {
            animatorCtrl = gameObject.GetComponent<Animator> ();
            animatorBoolSwitch = gameObject.GetComponent<AnimatorBoolSwitch> ();
            animatorBool = animatorBoolSwitch.animatorBool;
        } else {
            animatorBoolSwitch = gameObject.GetComponent<AnimatorBoolSwitch> ();
            animatorBool = animatorBoolSwitch.animatorBool;
        }

        if (animatorCtrl == null) {
            Debug.Log ("No Animator component found on " + gameObject.name);
        } else {
            value = animatorCtrl.GetBool (animatorBool);
        }

    }

    private void Update(){
        if(animatorCtrl)
            valueNew = animatorCtrl.GetBool(animatorBool);
        if (value != valueNew) {
            RecordToggle (valueNew);
            value = valueNew;
        }
    }

    void RecordToggle(bool toggleValue)
    {
        string eventLabel = "";

        if (toggleValue == true)
            eventLabel = "toggle_on";

        if (toggleValue == false)
            eventLabel = "toggle_off";

        //record this duration in AWS
        CustomEvent customEvent = new CustomEvent(eventLabel);
        customEvent.AddAttribute("Animation", animatorBool);
        AnalyticsManager.Get.RecordEvent(customEvent);
    }
        

}


