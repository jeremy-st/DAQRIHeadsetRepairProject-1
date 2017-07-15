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

	public class ThermalBackdrop : Backdrop {

		override public void Setup () {
			SetTexture(ServiceManager.Instance.GetThermalCameraTexture());

			float thermalAspect = ServiceManager.Instance.GetThermalCameraAspectRatio();
			float thermalFieldOfView = ServiceManager.Instance.GetThermalCameraFieldOfView();
			float farClipPlane = ServiceManager.Instance.GetFarClipPlane ();

			var rot = ServiceManager.Instance.GetThermalCameraPose_Rotation ();
			var pos = ServiceManager.Instance.GetThermalCameraPose_Position ();

			transform.localPosition = CalculateLocalPosition (pos, rot, farClipPlane);
			transform.localRotation = rot;
			transform.localScale = CalculateLocalScale (thermalAspect, thermalFieldOfView, farClipPlane);
		}

		void OnEnable () {
			ServiceManager.Instance.RegisterThermalTextureUser (this);
			Setup ();
		}

		void OnDisable () {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterThermalTextureUser (this);
			}
		}
	}
}
