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

using System;
using UnityEngine;

namespace DAQRI {

	/// <summary>
	/// Can be placed on an object to allow it to maintain the
	/// <see cref="DisplayManager"/>'s position and follow the screen.
	/// </summary>
	public class BodySpace : MonoBehaviour {
		private static readonly Vector3 ViewPortMiddle = new Vector3(0.5f, 0.5f, 0f);

		/// <summary>
		/// If true, body space will animate back in front of the camera if it goes out of view.
		/// </summary>
		public bool LooseFollow;

		/// <summary>
		/// <para>The rotation on the Y axis that the loose follow will initialize at.</para>
		/// <para>
		/// If <see cref="LooseFollow"/> is <c>true</c>, then this object will rotate from this
		/// value to be on screen which will prevent snapping.
		/// </para>
		/// <para>
		/// If <see cref="LooseFollow"/> is <c>false</c>, then this object will snap to a Y
		/// axis rotation of 0 once the device orientation has been discovered.
		/// </para>
		/// </summary>
		public float StartingYawRotation;

		/// <summary>Number of seconds it takes to perform the rotation to be viewed back on screen.</summary>
		public float LooseFollowDuration = 1f;

		/// <summary>Number of seconds allowed before animating back on screen.</summary>
		public float SecondsAllowedOffScreen = 2f;

		/// <summary>
		/// <para>The view port space distance allowed to consider the object is on screen.</para>
		/// <para>The middle of the screen is considered (0,0), and each edge is one unit away.</para>
		/// </summary>
		public Vector2 OffScreenBounds = Vector2.one;

		/// <summary>Calculated before each loose follow to prevent expensive divides each frame.</summary>
		private float animationSpeed;

		/// <summary>How long the loose follow rotation has taken so far.</summary>
		private float animationTime;

		/// <summary>Our lerp from rotation.</summary>
		private Quaternion startRotation;

		/// <summary>How long this object has been off screen.</summary>
		private float timeOffScreen;

		/// <summary>The method to run during <see cref="LateUpdate"/>.</summary>
		private Action updateState;

		private bool IsOnScreen {
			get {
				Vector3 targetPosition = transform.position + transform.forward * 2.0f;
				if (Vector3.Dot(Camera.main.transform.forward,
					(targetPosition - Camera.main.transform.position).normalized) <= 0f) {
					return false;
				}

				Vector3 screenPosition = (Camera.main.WorldToViewportPoint(targetPosition) - ViewPortMiddle) * 2f;
				return (screenPosition.x >= -OffScreenBounds.x) &&
					(screenPosition.x <= OffScreenBounds.x) &&
					(screenPosition.y >= -OffScreenBounds.y) &&
					(screenPosition.y <= OffScreenBounds.y);
			}
		}

		/// <summary>
		/// <para>
		/// The device's rotational data from the <see cref="ServiceManager"/> without the
		/// roll.
		/// </para>
		/// <para>
		/// Regardless if the <see cref="ServiceManager"/> has, or has not received the devices
		/// orientation yet, it will use the <see cref="DisplayManager"/>'s transform instead for the
		/// end result is that it should be based on exactly where the cameras are at.
		/// </para>
		/// </summary>
		private Quaternion TargetRotation {
			get {
				Quaternion targetRotation = DisplayManager.Instance.transform.rotation;

				Vector3 targetEulerAngles = targetRotation.eulerAngles;
				targetEulerAngles.z = 0f;
				return Quaternion.Euler(targetEulerAngles);
			}
		}

		/// <summary>
		/// <para>The device's positional data from the <see cref="ServiceManager"/>.</para>
		/// <para>
		/// Regardless if the <see cref="ServiceManager"/> has, or has not received the devices
		/// position yet, it will use the <see cref="DisplayManager"/>'s transform instead for the
		/// end result is that it should be based on exactly where the cameras are at.
		/// </para>
		/// </summary>
		private Vector3 TargetPosition {
			get {
				return DisplayManager.Instance.transform.position;
			}
		}

		/// <summary>This will instantly set the object to the device transform.</summary>
		public void Recenter() {
			transform.position = TargetPosition;
			transform.rotation = TargetRotation;
		}

		private void OnEnable() {
			updateState = UpdateInitialization;
			ServiceManager.Instance.RegisterVIOUser(this);
		}

		private void OnDisable() {
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVIOUser(this);
			}
		}

		/// <summary>The movement of this object should be done after the
		/// <see cref="ServiceManager"/> updates it's values, but before the render loop.</summary>
		private void LateUpdate() {
			transform.position = TargetPosition;
			updateState();
		}

		/// <summary>
		/// Keeps the object off screen until the <see cref="ServiceManager"/> has the
		/// device's orientation.
		/// </summary>
		private void UpdateInitialization() {
			transform.rotation = TargetRotation * Quaternion.AngleAxis(StartingYawRotation, Vector3.up);

			if (!ServiceManager.Instance.HasPoseData()) {
				// In the editor, we should wait until the simulated %IMU data is received is true to continue.
				if (!Application.isEditor || !ServiceManager.Instance.HasIMUData()) {
					return;
				}
			}

			timeOffScreen = 0f;
			if (!LooseFollow)
			{
				transform.rotation = TargetRotation;
			}

			updateState = UpdateCheckScreen;
			updateState();
		}

		/// <summary>
		/// Checks to see if and how long the object is off screen and triggers an
		/// animation of it's rotation if needed.
		/// </summary>
		private void UpdateCheckScreen() {
			if (!LooseFollow) {
				return;
			}

			if (IsOnScreen) {
				timeOffScreen = 0f;
				return;
			}

			if ((timeOffScreen += Time.deltaTime) < SecondsAllowedOffScreen) {
				return;
			}

			startRotation = transform.rotation;

			animationTime = 0f;
			animationSpeed = Mathf.Abs(LooseFollowDuration) > float.Epsilon ? 1f / LooseFollowDuration : 1f;
			updateState = UpdateAnimation;
			updateState();
		}

		/// <summary>Rotates the object back in front of the device.</summary>
		private void UpdateAnimation() {
			Quaternion targetRotation = TargetRotation;

			animationTime += Time.deltaTime;
			if (!(animationTime > LooseFollowDuration)) {
				transform.rotation = Quaternion.Slerp(startRotation, targetRotation,
					Mathf.SmoothStep(0f, 1f, animationTime * animationSpeed));
				return;
			}

			transform.rotation = targetRotation;
			timeOffScreen = 0f;
			updateState = UpdateCheckScreen;
		}
	}
}
