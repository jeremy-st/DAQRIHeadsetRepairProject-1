using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkInstructionsController : MonoBehaviour {

    public List<GameObject> steps;

    private int currentIndex;

    void Start()
    {
        currentIndex = 0;
        foreach (GameObject step in steps)
        {
            step.gameObject.SetActive(false);
        }

        steps[currentIndex].gameObject.SetActive(true);
    }

    public void GoToNextStep()
    {
        if (currentIndex <= steps.Count - 2)
        {
            steps[currentIndex].SetActive(false);
            currentIndex++;
            steps[currentIndex].SetActive(true);
            Debug.Log("Step " + currentIndex + " loaded");
        }
        else
        {
            Debug.Log("You're at the last step. No more steps to navigate to!");
        }
    }

    public void GoToPreviousStep()
    {
        if (currentIndex >= 1)
        {
            steps[currentIndex].SetActive(false);
            currentIndex--;
            steps[currentIndex].SetActive(true);
            Debug.Log("Step " + currentIndex + " loaded");
        }
    }

    public void Restart()
    {
        steps[currentIndex].SetActive(false);
        currentIndex = 0;
        steps[currentIndex].SetActive(true);
        Debug.Log("Step " + currentIndex + " loaded");
    }

    public void Quit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}
