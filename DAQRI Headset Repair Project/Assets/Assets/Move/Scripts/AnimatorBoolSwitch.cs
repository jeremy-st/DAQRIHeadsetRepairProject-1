using UnityEngine;
using System.Collections;

public class AnimatorBoolSwitch : MonoBehaviour {
    
    public Animator animatorCtrl;
    public string animatorBool = "";

    [HideInInspector]
    public bool value;

    void Awake ()
    {
        if(animatorCtrl == null)
            animatorCtrl = gameObject.GetComponent<Animator>();

        if(animatorCtrl == null)
            Debug.Log("No Animator component found on " + gameObject.name);
    }

    public void SetAnimatorBoolToTrue ()
    {
        if(animatorCtrl)
            animatorCtrl.SetBool(animatorBool, value = true);
    }

    public void SetAnimatorBoolToFalse ()
    {
        if(animatorCtrl)
            animatorCtrl.SetBool(animatorBool, value = false);
    }

    public void ToggleBool ()
    {
        value = !value;

        if(animatorCtrl)
            animatorCtrl.SetBool(animatorBool, value);
    }

}
