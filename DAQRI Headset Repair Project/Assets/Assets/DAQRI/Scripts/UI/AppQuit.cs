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
 *     File Purpose:        Used by Launcher Prefab.                                                                                    *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class AppQuit : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// Quits the application when a pointer click is received.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("Quitting application");
		Application.Quit();
	}

	/// <summary>
	/// At any instance, hitting 'esc' on the keyboard takes you out of the application.
	/// </summary>
	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Debug.Log("Quitting application");
			Application.Quit();
		}
	}
}
