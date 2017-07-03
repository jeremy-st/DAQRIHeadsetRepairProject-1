using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PauseVideoCloseVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private VideoSource videoSource;

    private AudioSource audioSource;
    public GameObject Video;
    public void EnableorDisable()
    {

        if (Video.name == "RawImage")
        {
            //Add VideoPlayer to the GameObject
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            
            Debug.Log("tag: " + Video.tag);
            //Add AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("it is");
            videoPlayer.Pause();
            audioSource.Pause();
            IsPaused.Is_Paused = true;
        }
        if (Video.active)
        {
            Video.SetActive(false);
        }
        else
        {
            Video.SetActive(true);
        }

    }
}
