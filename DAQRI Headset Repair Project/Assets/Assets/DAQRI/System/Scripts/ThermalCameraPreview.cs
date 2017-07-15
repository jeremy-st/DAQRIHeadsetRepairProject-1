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
 *     File Purpose:        <todo>                                                                                                      *
 *                                                                                                                                      *
 *     Guide:               <todo>                                                                                                      *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DAQRI {

	/// <summary>
	/// This script handles the rendering of a live video feed from the thermal camera.
	/// To use, drag the thermal camera preview prefab into your scene.
	/// </summary>
	[RequireComponent (typeof (RawImage))]
	public class ThermalCameraPreview : AbstractCameraPreview {
		
		RawImage rawImage;

		void Awake () {
			rawImage = GetComponent<RawImage> ();
			if (gameObject.GetComponentInParent<Canvas> () == null) {
				Debug.LogWarning ("ThermalCameraPreview requires a Canvas");
			}
		}

		void InitRawTexture () {
			rawImage.texture = ServiceManager.Instance.GetThermalCameraTexture();

			if (rawImage.texture != null) {
				Vector2 dimensions = ServiceManager.Instance.GetThermalCameraDimensions();
				rawImage.rectTransform.localScale = CalculateImageLocalScale (dimensions.x, dimensions.y);

			} else {
				Debug.LogWarning("Thermal Camera texture unavailable");
			}
		}

		void OnEnable () {
			ServiceManager.Instance.RegisterThermalTextureUser (this);
			InitRawTexture ();
		}

		void OnDisable () {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterThermalTextureUser (this);
			}
		}
	}
}
