using UnityEngine;
using System.Collections;

public class FireAnimatorEvent: MonoBehaviour {

    // The animator we want to fire for this event
    public Animator animatorEvent;

    // The animator trigger to be set in the inspector
    public string parameter;

    // Bool value to set on the Animator
    public bool value;

    // Reference to the Audio Source and clip we want to play when the animator event fires
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Wait time before the animator trigger can be fired again
    public float waitTime = 5f;

    // Whether or not we can fire the animator event
    private bool canFire = true;

    // Cached coroutines
    IEnumerator fireEvent;

    public void _FireEvent ()
    {
        if(canFire)
        {
            if(fireEvent != null)
                StopCoroutine (fireEvent);
            fireEvent = FireEvent();
                StartCoroutine (fireEvent);
        }
    }

    IEnumerator FireEvent ()
    {
        canFire = false;
        animatorEvent.SetBool(parameter, value);

        if(audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        yield return new WaitForSeconds(waitTime);
        canFire = true;
    }
}
