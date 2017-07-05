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
 *     File Purpose:        Stereo camera hierarchy created at runtime. Manages video background rendering.                             *
 *                                                                                                                                      *
 *     Guide:               Create a new StereoCamera, make its gameobject a child of the Display. Call Show/HideVideoBackground        *
 *                          to toggle video background rendering.                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace DAQRI {

	public class StereoCamera : DisplayCamera {

		public Camera leftCamera;
		public Camera rightCamera;

		public override Vector3 ScreenCenter {
			get {
				Vector3 center = Vector3.zero;
				// Average the positions
				center.x = (leftCamera.transform.localPosition.x + rightCamera.transform.localPosition.x);
				center.y = (leftCamera.transform.localPosition.y + rightCamera.transform.localPosition.y);
				center.z = (leftCamera.transform.localPosition.z + rightCamera.transform.localPosition.z);

				Quaternion rotation = Quaternion.Slerp (leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
				return rotation * (center / 2);
			}
		}

		/// <summary>
		/// Creates a new stereoscopic camera.
		/// </summary>
		public StereoCamera (DisplayManager displayManager, bool preserveAspectRatio) : base(displayManager) {

			this.preserveAspectRatio = false;
			this.opticalSeeThrough = true;

			gameObject = new GameObject ("Stereo Camera");
			gameObject.transform.SetParent (displayManager.transform, false);

			GameObject leftCameraObject = new GameObject ("Left Camera");
			leftCameraObject.transform.SetParent (gameObject.transform, false);
			leftCamera = leftCameraObject.AddComponent<Camera> ();
			InitCamera (leftCamera);

			GameObject rightCameraObject = new GameObject ("Right Camera");
			rightCameraObject.transform.SetParent (gameObject.transform, false);
			rightCamera = rightCameraObject.AddComponent<Camera> ();
			InitCamera (rightCamera);

			leftCamera.rect = new Rect (0.0f, 0.0f, 0.5f, 1.0f);
			rightCamera.rect = new Rect (0.5f, 0.0f, 0.5f, 1.0f);

			float[] leftProjMatrixRaw = new float[16];
			float[] rightProjMatrixRaw = new float[16];
			float[] leftPosRaw = new float[3];
			float[] leftRotRaw = new float[4];
			float[] rightPosRaw = new float[3];
			float[] rightRotRaw = new float[4];

			DSHUnityPlugin.GetDisplayProjectionMatrices (ServiceManager.Instance.GetNearClipPlane(), ServiceManager.Instance.GetFarClipPlane(), leftProjMatrixRaw, rightProjMatrixRaw);
			DSHUnityPlugin.GetDisplayPoses (leftPosRaw, leftRotRaw, rightPosRaw, rightRotRaw);

			Matrix4x4 leftProjMatrix = ServiceManager.MatrixFromFloatArray (leftProjMatrixRaw);
			Matrix4x4 rightProjMatrix = ServiceManager.MatrixFromFloatArray (rightProjMatrixRaw);

			Vector3 leftPos = ServiceManager.Vector3FromFloatArray (leftPosRaw);
			Quaternion leftRot = ServiceManager.QuaternionFromFloatArray (leftRotRaw);
			Vector3 rightPos = ServiceManager.Vector3FromFloatArray (rightPosRaw);
			Quaternion rightRot = ServiceManager.QuaternionFromFloatArray (rightRotRaw);

			leftCamera.transform.localPosition = leftPos;
			leftCamera.transform.localRotation = leftRot;

			leftCamera.transform.localRotation = leftCamera.transform.localRotation;

			rightCamera.transform.localPosition = rightPos;
			rightCamera.transform.localRotation = rightRot;
			rightCamera.transform.localRotation = rightCamera.transform.localRotation;

			/*var eul = rightCamera.transform.localRotation.eulerAngles;

			eul.x = 0;
			eul.y = -0.2f;
			eul.z = 1.6f;
			rightCamera.transform.localRotation = Quaternion.Euler (eul);
			*/

			/*eul = leftCamera.transform.localRotation.eulerAngles;

			eul.x = 0;
			eul.y = 0.2f;
			eul.z = -1.6f;
			leftCamera.transform.localRotation = Quaternion.Euler (eul);
			*/


			leftCamera.projectionMatrix = leftProjMatrix;
			rightCamera.projectionMatrix = rightProjMatrix;

			ServiceManager.Instance.VisionParametersAvailable += Resize;
			DisplayManager.Instance.ScreenSizeChanged += Resize;
		}

		public void Resize () {
			ResizeCamera (leftCamera);
			ResizeCamera (rightCamera);
		}

		public override void MakeMainCamera () {
			if (Camera.main != null) {
				Debug.LogWarning ("StereoCamera is replacing an existing camera as the MainCamera");
				Camera.main.tag = "Untagged";
			}
			leftCamera.tag = "MainCamera";
			leftCamera.gameObject.AddComponent<PhysicsRaycaster> ();
		}

		public override void Render()
		{
			leftCamera.Render();
			rightCamera.Render();
		}
	}
}
