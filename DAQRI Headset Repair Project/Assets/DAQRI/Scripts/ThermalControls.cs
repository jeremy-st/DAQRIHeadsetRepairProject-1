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
 *     File Purpose:        Show how to access the thermal camera on a supported DAQRI smart device.                                    *
 *                                                                                                                                      *
 *     Guide:               Run this sample on a device, use the toggles to experiment with thermal vision and thermal previews.        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using DAQRI;

public class ThermalControls : MonoBehaviour {

	public GameObject thermalCameraPreview;

	void Start () {
		DisplayManager.Instance.TurnVideoBackgroundOff ();
		DisplayManager.Instance.TurnThermalBackgroundOn ();
	}

	// Thermal vision is overlaid on the real world
	public void ThermalVisionToggled (bool isOn) {
		if (isOn) {
			DisplayManager.Instance.TurnThermalBackgroundOn ();
		} else {
			DisplayManager.Instance.TurnThermalBackgroundOff ();
		}
	}

	// Thermal previews are used in user interfaces
	public void ThermalPreviewToggled (bool isOn) {
		if (isOn) {
			thermalCameraPreview.SetActive (true);
		} else {
			thermalCameraPreview.SetActive (false);
		}
	}
}
