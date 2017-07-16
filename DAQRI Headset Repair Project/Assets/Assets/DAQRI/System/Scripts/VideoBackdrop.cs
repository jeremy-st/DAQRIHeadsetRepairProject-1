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
using System.Collections;

namespace DAQRI {

	public class VideoBackdrop : Backdrop {

		override public void Setup () {
			SetTexture( ServiceManager.Instance.GetColorCameraTexture());
			//DSHUnityPlugin.Instance.CameraGetPose(posRaw, rotRaw);

			float visionAspect = ServiceManager.Instance.GetColorCameraAspectRatio();
			float farClipPlane = ServiceManager.Instance.GetFarClipPlane ();
			float visionFieldOfView = ServiceManager.Instance.GetColorCameraFieldOfView ();

			var rot = ServiceManager.Instance.GetColorCameraPose_Rotation();
			var pos = ServiceManager.Instance.GetColorCameraPose_Position();


			transform.localPosition = CalculateLocalPosition (pos, rot, farClipPlane);
			transform.localRotation = rot;
			transform.localScale = CalculateLocalScale (visionAspect, visionFieldOfView, farClipPlane);
		}

		void OnEnable () {
			bool isColorCameraHDOn = ServiceManager.Instance.GetColorCameraHDOnOff();
			if (isColorCameraHDOn) {
				ServiceManager.Instance.RegisterVideoTextureUser (this, true);
			} else {
				ServiceManager.Instance.RegisterVideoTextureUser (this);
			}
			Setup ();
		}

		void OnDisable () {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVideoTextureUser (this);
			}
		}
	}
}
