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
	
	public class BodySpace : MonoBehaviour {

		public bool LooseFollow = false;
		public float LooseFollowThreshold = 2.0f; // 0.3f for EVT2

		private bool initialized = false;

		private float maxSecondsOffScreen = 2.0f;
		private float animationDuration = 1.0f;
		private bool screenEdgeContent = true;

		private float timeOffScreen = 0.0f;
		private bool isAnimating = false;

		public void Recenter () {
			if (ServiceManager.Instance.HasIMUData ()) {
				transform.position = ServiceManager.Instance.GetPosition ();
				transform.rotation = ServiceManager.Instance.GetOrientation ();
			}
		}

		void OnEnable () {
			ServiceManager.Instance.RegisterVIOUser (this);
		}

		void OnDisable () {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVIOUser (this);
			}
		}

		void Update () {

			if (!initialized) {
				LateInitialization ();
			}

			transform.position = ServiceManager.Instance.GetPosition ();

			if (LooseFollow) {
				PerformLooseFollow ();
			}
		}

		private void LateInitialization () {
			if (ServiceManager.Instance.HasIMUData ()) {
				transform.position = ServiceManager.Instance.GetPosition ();
				transform.rotation = ServiceManager.Instance.GetOrientation ();
				initialized = true;
			}
		}

		private void PerformLooseFollow () {

			if (!isAnimating)
			{
				if (isOnScreen (transform.position + transform.forward * 2.0f))
				{
					timeOffScreen = 0.0f;
				}
				else
				{
					timeOffScreen += Time.deltaTime;

					Vector3 gyro = ServiceManager.Instance.GetIMUGyro ();
					if (Mathf.Abs (gyro.x) > LooseFollowThreshold || Mathf.Abs (gyro.y) > LooseFollowThreshold || Mathf.Abs (gyro.z) > LooseFollowThreshold) {
						// high activity, reset timer
						timeOffScreen = 0.0f;
					}

					if (timeOffScreen >= maxSecondsOffScreen)
					{
						StartCoroutine (AnimateBackOnScreen ());
					}
				}
			}
		}

		private IEnumerator AnimateBackOnScreen() {
			
			isAnimating = true;

			Quaternion deviceOrientation = ServiceManager.Instance.GetOrientation ();
			Quaternion startRotation = transform.rotation;

			float devicePitch = deviceOrientation.eulerAngles.x;
			float deviceYaw = deviceOrientation.eulerAngles.y;
			Quaternion endRotation = Quaternion.Euler (devicePitch, deviceYaw, 0.0f);

			float t = 0.0f;

			while (t < animationDuration) {
				t += Time.deltaTime;
				transform.rotation = Quaternion.Slerp(startRotation, endRotation, t / animationDuration);
				yield return 0;
			}

			isAnimating = false;
		}

		bool isOnScreen(Vector3 pos) {
			
			Vector3 screenpos = Camera.main.WorldToViewportPoint (pos);
			if (screenEdgeContent) {
				if (screenpos.x >= -0.25 && screenpos.x <= 1.25 && screenpos.y >= -0.25 && screenpos.y <= 1.25) {
					return true;
				} else {
					return false;
				}
			} else {
				if (screenpos.x >= 0 && screenpos.x <= 1 && screenpos.y >= 0 && screenpos.y <= 1) {
					return true;
				} else {
					return false;
				}
			}
		}
	}
}
