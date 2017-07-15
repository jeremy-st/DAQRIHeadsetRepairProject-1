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
 *     File Purpose:        Attach this script to any generic panel for it to be part of a set of navigation panels                     *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// A panel is a single page that holds UI content.
/// To add UI elements to a panel, add your UI game objects as children of the panel in a scene.
/// See PanelController for navigating between a series of panels.
/// </summary>
public class Panel : MonoBehaviour {
	
    /// <summary>
    /// The page number of this panel.
    /// The menu uses this to order the panels.
    /// </summary>
    public int pageNumber = 0;

    /// <summary>
    /// The title to be displayed in the menu when this panel appears.
    /// If left blank, the menu title will default to whatever its text was on start.
    /// </summary>
    public string menuTitle = string.Empty;

	/// <summary>
	/// The event to invoke when the panel is enabled.
	/// </summary>
	[SerializeField]
	public UnityEvent OnPanelAppear;

	/// <summary>
	/// The event to invoke when the panel is disabled.
	/// </summary>
	[SerializeField]
	public UnityEvent OnPanelDisappear;

	void OnEnable () {
		if (OnPanelAppear != null) {
			OnPanelAppear.Invoke ();
		}
	}

	void OnDisable () {
		if (OnPanelDisappear != null) {
			OnPanelDisappear.Invoke ();
		}
	}
}
