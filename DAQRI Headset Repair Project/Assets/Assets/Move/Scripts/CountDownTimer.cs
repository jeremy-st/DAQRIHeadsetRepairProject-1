using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DAQRI.Turbine.Scripts
{
    public class CountDownTimer : MonoBehaviour
    {
        [SerializeField]
        private Image foregroundImage;
        [SerializeField]
        private Text textComponent;
        [SerializeField]
        private float waitTime = 5f;
        [SerializeField]
        private List<AudioClip> countDownAudioList = new List<AudioClip>();
        [SerializeField]
        private AudioSource currentAudioSource;
        public UnityEvent CountDownFinished;
        public UnityEvent BodySpaceCountDownFinished;

        private void OnEnable()
        {
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            if (textComponent == null || foregroundImage == null)
            {
                Debug.LogError("Missing Components.");
                yield break;
            }

            if (currentAudioSource == null)
            {
                currentAudioSource = gameObject.GetComponent<AudioSource>();
                if (currentAudioSource == null)
                {
                    currentAudioSource = gameObject.AddComponent<AudioSource>();
                    currentAudioSource.playOnAwake = false;
                }

            }

            float timeLeft = waitTime;

            currentAudioSource.clip = countDownAudioList[Mathf.RoundToInt(timeLeft)];
            currentAudioSource.Play();
            while (true)
            {
                textComponent.text = Mathf.RoundToInt(timeLeft).ToString();
                if (foregroundImage.fillAmount <= 0)
                {
                    foregroundImage.fillAmount = 1f;
                    timeLeft -= 1f;
                    currentAudioSource.clip = countDownAudioList[Mathf.RoundToInt(timeLeft)];
                    if (currentAudioSource.clip != null)
                    {
                        currentAudioSource.Play();
                    }
                }
                foregroundImage.fillAmount -= Time.deltaTime;
                yield return new WaitForEndOfFrame();

                if (timeLeft <= 0)
                {
                    if (!InstructionsController.worldspace)
                    {
                        if (CountDownFinished.GetPersistentEventCount() > 0)
                        {
                            CountDownFinished.Invoke();
                        }
                    }
                    else
                    {
                        if (BodySpaceCountDownFinished.GetPersistentEventCount() > 0)
                        {
                            BodySpaceCountDownFinished.Invoke();
                        }
                    }
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
