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
	
	[RequireComponent (typeof (RawImage))]
	public class ColorCameraPreview : MonoBehaviour {

		void Awake () {

			if (gameObject.GetComponentInParent<Canvas> () == null) {
				Debug.LogWarning ("ColorCameraPreview requires a Canvas");
			}
		}

		void InitRawTexture()
		{
			RawImage rawImage = GetComponent<RawImage> ();
			rawImage.texture = ServiceManager.Instance.GetColorCameraTexture ();
			if (rawImage.texture != null) {
				float aspectRatio = rawImage.texture.width / (float)rawImage.texture.height;
				rawImage.rectTransform.localScale = new Vector3 (aspectRatio, -1.0f, 1.0f);
			} else {
				Debug.LogWarning("Color Camera texture unavailable");
			}
		}

		void OnEnable () {
			ServiceManager.Instance.RegisterVideoTextureUser (this);
			InitRawTexture ();
		}

		void OnApplicationQuit()
		{
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVideoTextureUser (this);
			}
		}

		void OnDisable () {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVideoTextureUser (this);
			}
		}
	}
}
