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
 *     File Purpose:        Mono camera hierarchy created at runtime. Manages video background rendering.                               *
 *                                                                                                                                      *
 *     Guide:               Create a new MonoCamera, make its gameobject a child of the Display. Call Show/HideVideoBackground          *
 *                          to toggle video background rendering.                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace DAQRI {

	public class MonoCamera : DisplayCamera {

		public Camera camera = null;
		public GameObject videoBackground = null;
		public GameObject thermalBackground = null;

        // put the camera in the IMU position; otherwise it will be aligned with the video camera
		private const bool useImuPose = true; 

		public override Vector3 ScreenCenter {
			get {
				return camera.transform.localPosition;
			}
		}

		/// <summary>
		/// Creates a new monoscopic camera.
		/// </summary>
		/// <param name="preserveAspectRatio">Preserves the aspect ratio of the video
		/// background and augmentations as the screen size changes.</param>
		public MonoCamera (DisplayManager displayManager, bool preserveAspectRatio) : base(displayManager) {

			this.preserveAspectRatio = preserveAspectRatio;
			this.opticalSeeThrough = false;

			gameObject = new GameObject ("Mono Camera");

			// set camera pose
			gameObject.transform.SetParent (displayManager.transform, false);
			if (useImuPose) {
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
			} else {
				float[] posRaw = new float[3];
				float[] rotRaw = new float[4];
				DSHUnityPlugin.CameraGetPose (posRaw, rotRaw);
				gameObject.transform.localPosition = ServiceManager.Vector3FromFloatArray (posRaw);
                #if !UNITY_EDITOR
                gameObject.transform.localRotation = ServiceManager.QuaternionFromFloatArray (rotRaw);
                #endif
			}

			camera = gameObject.AddComponent<Camera> ();
			InitCamera (camera);

			ServiceManager.Instance.VisionParametersAvailable += Resize;
			DisplayManager.Instance.ScreenSizeChanged += Resize;
		}

		public void Resize () {
			ResizeCamera (camera);
		}

		public override void MakeMainCamera () {
			if (Camera.main != null) {
				Debug.LogWarning ("MonoCamera is replacing an existing camera as the MainCamera");
				Camera.main.tag = "Untagged";
			}
			camera.tag = "MainCamera";
			camera.gameObject.AddComponent<PhysicsRaycaster> ();
		}

		public override void Render()
		{
			camera.Render();
		}
	}
}
