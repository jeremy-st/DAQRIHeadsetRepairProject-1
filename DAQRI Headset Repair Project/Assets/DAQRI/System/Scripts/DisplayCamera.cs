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
 *     File Purpose:        Abstract class for a camera hierarchy created at runtime.                                                   *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DAQRI {

	abstract public class DisplayCamera {
		
		protected GameObject gameObject;
		protected DisplayManager displayManager;

		public bool preserveAspectRatio = false;
		public bool opticalSeeThrough = false;

		abstract public void MakeMainCamera ();
		abstract public void Render ();

		abstract public Vector3 ScreenCenter {
			get;
		}

		protected DisplayCamera(DisplayManager displayManager) {
			this.displayManager = displayManager;

		}

		protected void InitCamera (Camera camera) {

			if (!Application.isEditor) {
				camera.clearFlags = CameraClearFlags.Color;
				camera.backgroundColor = Color.black;
			}
			ResizeCamera (camera);
			camera.enabled = true; // disable to be used in our own render loop in ServiceManager
		}

		protected void ResizeCamera (Camera camera) {

			if (opticalSeeThrough) {

				// we're setting this in StereoCamera now
				// camera.nearClipPlane = ServiceManager.Instance.GetNearClipPlane ();
				// camera.farClipPlane = ServiceManager.Instance.GetFarClipPlane ();
				// camera.fieldOfView = ServiceManager.Instance.GetOpticalFieldOfView ();
				// camera.aspect = ServiceManager.Instance.GetOpticalAspectRatio ();

			} else {

				Matrix4x4 projectionMat = ServiceManager.Instance.GetVisionProjectionMatrix ();

				if (preserveAspectRatio) {
					float cameraAspect = ServiceManager.Instance.GetVisionAspectRatio ();
					Vector2 imageSize = AspectFillScreenSize (cameraAspect);
					float xScale = imageSize.x / Screen.width;
					float yScale = imageSize.y / Screen.height;

					Vector3 scale = new Vector3 (xScale, yScale, 1.0f);
					Matrix4x4 scaleMat = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);

					// this will scale both the video background and the augmentations
					projectionMat = projectionMat * scaleMat;
				}

				camera.nearClipPlane = ServiceManager.Instance.GetNearClipPlane ();
				camera.farClipPlane = ServiceManager.Instance.GetFarClipPlane ();
				camera.projectionMatrix = projectionMat;
				camera.fieldOfView = ServiceManager.Instance.GetVisionFieldOfView ();
				camera.aspect = ServiceManager.Instance.GetVisionAspectRatio ();
			}
		}

		#region Utility Methods

		private Vector2 AspectFillScreenSize (float imageAspect) {

			float screenAspect = Screen.width / (float) Screen.height;
			float imageWidth, imageHeight;

			if (imageAspect > screenAspect) {
				// image is wider than screen
				imageHeight = Screen.height;
				imageWidth = imageHeight * imageAspect;
			} else {
				// screen is wider than image
				imageWidth = Screen.width;
				imageHeight = imageWidth / imageAspect;
			}

			return new Vector2 (imageWidth, imageHeight);
		}

		private Vector2 AspectFitScreenSize (float imageAspect) {

			float screenAspect = Screen.width / (float) Screen.height;
			float imageWidth, imageHeight;

			if (imageAspect > screenAspect) {
				// image is wider than screen
				imageWidth = Screen.width;
				imageHeight = imageWidth / imageAspect;
			} else {
				// screen is wider than image
				imageHeight = Screen.height;
				imageWidth = imageHeight * imageAspect;
			}

			return new Vector2 (imageWidth, imageHeight);
		}

		private Vector2 SizeToFillFrustum (float fieldOfView, float aspectRatio, float distance) {
			float halfFov = Mathf.Deg2Rad * (fieldOfView / 2.0f);
			float height = Mathf.Tan (halfFov) * distance * 2.0f;
			float width = aspectRatio * height;
			return new Vector2 (width, height);
		}

		#endregion
	}
}
