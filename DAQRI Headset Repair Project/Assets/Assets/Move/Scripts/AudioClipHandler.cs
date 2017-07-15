using UnityEngine;
using System.Collections;

public class AudioClipHandler : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip audioClip;

    void Awake ()
    {
        // First failsafe to assign an AudioSource if none has been assigned in the inspector
        if(audioSource == null)
            audioSource = gameObject.GetComponent<AudioSource>();

        // Second safety check to warn us if no AudioSource is found on the gameobject this script is attached to
        if(audioSource == null)
            Debug.Log("No AudioSource component found on " + gameObject.name);
    }

    // Used to play the assigned AudioClip
    public void PlayClip () 
    {
        if(audioSource)
        {
            if(audioSource.isPlaying)
                return;
            else
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }

    // Used to pause the assigned AudioClip
    public void PauseClip ()
    {
        if(audioSource)
        {
            audioSource.clip = audioClip;
            audioSource.Pause();
        }
    }

    // Used to stop the assigned AudioClip
    public void StopClip ()
    {
        if(audioSource)
        {
            audioSource.clip = audioClip;
            audioSource.Stop();
        }
    }
}