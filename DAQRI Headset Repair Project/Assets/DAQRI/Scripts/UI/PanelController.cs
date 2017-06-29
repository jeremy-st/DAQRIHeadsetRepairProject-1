/****************************************************************************************************************************************
 * © 2016 Daqri International. All Rights Reserved.                                                                                     *
 *                                                                                                                                      *
 *     NOTICE:  All software code and related information contained herein is, and remains the property of DAQRI INTERNATIONAL and its  *
 * suppliers, if any.  The intellectual and technical concepts contained herein are proprietary to DAQRI INTERNATIONAL and its          *
 * suppliers and may be covered by U.S. and Foreign Patents, patents in process, and/or trade secret law, and the expression of         *
 * those concepts is protected by copyright law. Dissemination, reproduction, modification, public display, reverse engineering, or     *
 * decompiling of this material is strictly forbidden unless prior written permission is obtained from DAQRI INTERNATIONAL.             *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *     File Purpose:        Used by NavigationPanel UI Prefab                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using System.Collections.Generic;

public class PanelController : MonoBehaviour {

	public List<Panel> panels;

	private int currentIndex;

	void Start()
	{
		currentIndex = 0;
		foreach (Panel panel in panels)
		{
			panel.gameObject.SetActive(false);
		}
		panels[currentIndex].gameObject.SetActive(true);
	}

	public void GoToNextPanel()
	{
		if (currentIndex <= panels.Count - 2)
		{
			panels[currentIndex].gameObject.SetActive(false);
			currentIndex++;
			panels[currentIndex].gameObject.SetActive(true);
			Debug.Log("Panel " + currentIndex + " loaded");
		}
		else {
			Debug.Log("You're at the last panel. No more panles to navigate to!");
		}
	}

	public void GoToPreviousPanel()
	{
		if (currentIndex >= 1)
		{
			panels[currentIndex].gameObject.SetActive(false);
			currentIndex--;
			panels[currentIndex].gameObject.SetActive(true);
		}
	}

	public void JumpToPanelIndex(int panelIndex)
	{
		if (panelIndex >= 0 && panelIndex < panels.Count)
		{
			panels[currentIndex].gameObject.SetActive(false);
			currentIndex = panelIndex;
			panels[currentIndex].gameObject.SetActive(true);
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}
