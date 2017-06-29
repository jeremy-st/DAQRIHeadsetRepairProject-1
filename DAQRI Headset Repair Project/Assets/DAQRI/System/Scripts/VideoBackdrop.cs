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
	
	public class VideoBackdrop : MonoBehaviour {
		bool isInitialized  = false;
		void Start () {
			GetComponent<MeshRenderer> ().material = (Material) Resources.Load("Materials/CameraBackdrop", typeof (Material));
			GetComponent<MeshRenderer> ().material.SetTexture ("_MainTex", ServiceManager.Instance.GetColorCameraTexture ());
			ServiceManager.Instance.VisionParametersAvailable += Setup;
		}

		void Setup () {
            float[] posRaw = new float[3]; float[] rotRaw = new float[4];
            DSHUnityPlugin.CameraGetPose(posRaw, rotRaw);

            float visionAspect = ServiceManager.Instance.GetVisionAspectRatio ();

			float dist = ServiceManager.Instance.GetFarClipPlane () * 0.95f;
			float halfFov = Mathf.Deg2Rad * (ServiceManager.Instance.GetVisionFieldOfView () / 2.0f);
			float height = Mathf.Tan (halfFov) * dist * 2.0f;
			float width = visionAspect * height;

			//var rot = ServiceManager.QuaternionFromFloatArray (rotRaw);
			transform.localPosition = /*ServiceManager.Vector3FromFloatArray(posRaw) + rot * */(new Vector3 (0.0f, 0.0f, dist));
            #if !UNITY_EDITOR
			//transform.localRotation = rot;
            #endif
			transform.localScale = new Vector3 (width, -height, 1.0f);
		}

		void OnEnable () {
			if (isInitialized) {
				ServiceManager.Instance.RegisterVideoTextureUser (this);
				Setup ();
			}
			isInitialized = true;
		}

		void OnDisable () {
			if (isInitialized) {
				if (ServiceManager.InstanceExists) {
					ServiceManager.Instance.UnregisterVideoTextureUser (this);
				}
			}
		}
	}
}
