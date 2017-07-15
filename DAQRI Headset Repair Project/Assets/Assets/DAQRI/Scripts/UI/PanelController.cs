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
 *     File Purpose:        Provides easy movement between panels to display different content.                                         *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Provides functionality for navigating between a series of panels.
/// See Panel for more information on individual panels.
/// </summary>
public class PanelController : MonoBehaviour {
    
    public GameObject backButton;
    public Text menuTitle;

    private List<Panel> panels = new List<Panel> ();
	private int currentIndex;
    private string initialMenuTitle = string.Empty;

    void Awake () {
        GetComponentsInChildren<Panel> (true, panels);
        panels.Sort ((p1, p2)=>p1.pageNumber.CompareTo(p2.pageNumber));

        if (menuTitle != null) {
            initialMenuTitle = menuTitle.text;
        }
    }

	void Start() {
        ResetState ();
	}

	void OnEnable () {
        ResetState ();
	}


	#region Public

    /// <summary>
    /// Shows the next panel in the list and hides the current one.
    /// </summary>
    public void GoToNextPanel () {
        GoToNextPanel (null);
    }

	/// <summary>
	/// Shows the next panel in the list and hides the current one.
    /// Has an optional text parameter to override the menu title.
    /// This parameter is useful if the menu title relies on runtime data.
	/// </summary>
    public void GoToNextPanel(Text overriddenMenuTitle = null) {
		bool isIndexValid = (currentIndex <= panels.Count - 2);

		if (isIndexValid) {
			HidePanelAtCurrentIndex ();
			currentIndex++;
            ShowPanelAtCurrentIndex (overriddenMenuTitle);

			Debug.Log("Panel " + currentIndex + " loaded");

		} else {
			Debug.Log("You're at the last panel. No more panles to navigate to!");
		}
	}

	/// <summary>
	/// Shows the previous panel in the list and hides the current one.
	/// </summary>
	public void GoToPreviousPanel() {
		bool isIndexValid = (currentIndex >= 1);

		if (isIndexValid) {
			HidePanelAtCurrentIndex ();
			currentIndex--;
			ShowPanelAtCurrentIndex ();

		} else {
			Debug.Log("You're at the first panel. No more panles to navigate to!");
		}
	}

	/// <summary>
	/// Jumps to the panel at a given index in the list.
	/// </summary>
	/// <param name="panelIndex">Panel index in the list.</param>
	public void JumpToPanelIndex(int panelIndex) {
		bool isIndexValid = (panelIndex >= 0 && panelIndex < panels.Count);

		if (isIndexValid) {
			HidePanelAtCurrentIndex ();
			currentIndex = panelIndex;
			ShowPanelAtCurrentIndex ();
		}
	}

	#endregion


	#region Private Helpers

    /// <summary>
    /// Hides all panels except the first one.
    /// </summary>
    private void ResetState () {
        currentIndex = 0;
        foreach (Panel panel in panels) {
            panel.gameObject.SetActive (false);
        }

        ShowPanelAtCurrentIndex ();
    }

	private void HidePanelAtCurrentIndex () {
		Panel panel = panels [currentIndex];
		panel.gameObject.SetActive(false);
	}

    /// <summary>
    /// Shows the index of the panel at current.
    /// If overridden menu text is provided, that text will be displayed and assigned to the panel.
    /// </summary>
    /// <param name="overriddenMenuText">Overridden menu text.</param>
    private void ShowPanelAtCurrentIndex (Text overriddenMenuText = null) {
		Panel panel = panels [currentIndex];

        if (menuTitle != null) {
            if (overriddenMenuText != null) {
                menuTitle.text = overriddenMenuText.text;
                panel.menuTitle = overriddenMenuText.text; // This ensures proper title when moving back to this panel via back button

            } else {
                bool usePanelTitle = (panel.menuTitle != null && panel.menuTitle != string.Empty);

                if (usePanelTitle) {
                    menuTitle.text = panel.menuTitle;

                } else {
                    menuTitle.text = initialMenuTitle;
                }
            }
        }

		panel.gameObject.SetActive(true);

        backButton.SetActive (currentIndex > 0);
	}

	#endregion
}
