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
 *     File Purpose:        Enable video see-through.                                                                                   *
 *                                                                                                                                      *
 *     Guide:               Attach this to any object to enable video see-through on a supported DAQRI smart device.                    *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using DAQRI;

public class VideoSeeThrough : MonoBehaviour {
	
	void Start () {
		StartCoroutine (StartVideoSeeThrough());
	}

	private IEnumerator StartVideoSeeThrough()
	{
		yield return new WaitForSeconds(1.0f);
		DisplayManager.Instance.TurnVideoBackgroundOn ();
	}

	void Update()
	{
		/*if (Input.GetKeyDown (KeyCode.N)){

			StartCoroutine (StartVideoSeeThrough());

		}

		if (Input.GetKeyDown (KeyCode.M)){
			DisplayManager.Instance.TurnVideoBackgroundOff ();


		}*/

	}
}
