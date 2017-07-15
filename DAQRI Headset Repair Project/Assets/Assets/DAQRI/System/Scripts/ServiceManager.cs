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
 *     File Purpose:        Singleton class for managing the device. Gives access to sensors and tracking.                              *
 *                                                                                                                                      *
 *     Guide:               Call ServiceManager.Instance to access the singleton object.                                                *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Linq;

namespace DAQRI {

	/// <summary>
	/// The service manager contains more detailed functionality around sensors and targets.
	/// A basic application should not need to interact with the service manager directly.
	/// </summary>
	public class ServiceManager : MonoBehaviour {

		/// <summary>
		/// Occurs when target identifiers are available.
		/// </summary>
		public event System.Action TargetIdsAvailable;

		/// <summary>
		/// Occurs when vision parameters are available.
		/// </summary>
		public event System.Action VisionParametersAvailable;

		private bool _initialized = false;
		private Vector3 finalGyro = Vector3.zero;
		private IServiceManagerController controller;


		#region Singleton

		private static ServiceManager instance;

		/// <summary>
		/// The accessor for the <see cref="DAQRI.ServiceManager"/> singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		public static ServiceManager Instance {
			get {
				if (instance == null) {

					DisplayManager[] displayInstances = GameObject.FindObjectsOfType<DisplayManager> ();
					GameObject display = null;

					foreach (DisplayManager manager in displayInstances) {
						if (!manager.isInvalidDuplicate) {
							display = manager.gameObject;
							break;
						}
					}

					if (display != null) {
						instance = display.AddComponent<ServiceManager> ();

					} else {
						GameObject go = new GameObject ("ServiceManager");
						instance = go.AddComponent<ServiceManager> ();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="DAQRI.ServiceManager"/> instance exists.
		/// </summary>
		/// <value><c>true</c> if instance exists; otherwise, <c>false</c>.</value>
		public static bool InstanceExists {
			get { return instance != null; }
		}

		#endregion

		/// <summary>
		/// Sets a custom controller.
		/// This is helpful when mocking behavior in tests.
		/// </summary>
		/// <param name="controller">A custom controller.</param>
		public void SetController (IServiceManagerController controller) {
			this.controller = controller;
		}

		/// <summary>
		/// Checks if the standard %IMU is enabled.
		/// </summary>
		/// <returns><c>true</c>, if %IMU is enabled, <c>false</c> otherwise.</returns>
		public bool GetIMUEnabled () {
			return controller.IsIMUEnabled ();
		}

		/// <summary>
		/// Checks if the VIO %IMU is enabled.
		/// </summary>
		/// <returns><c>true</c>, if the VIO %IMU is enabled, <c>false</c> otherwise.</returns>
		public bool GetVIOIMUEnabled () {
			return controller.IsVIOIMUEnabled ();
		}


		#region MonoBehavior Events

		void Awake () {
			// Initialize the camera early so the textures are ready to be accessed
			// by other classes on Start
			DSHUnityPlugin.Instance.Initialize ();

			controller = new ServiceManagerController (DSHUnityPlugin.Instance, VisionUnityPlugin.Instance);
			controller.OnTargetIdsAvailable += HandleControllerTargetsAvailable;
			controller.OnVisionParametersAvailable += HandleControllerVisionParamsAvailable;
		}

		/*public IColorCamera GetColorCamera(){
			return controller.GetColorCamera ();
		}
		public IThermalSensor GetThermalSensor(){
			return controller.GetThermalSensor ();
		}
		public IDepthCamera GetDepthCamera(){
			return controller.GetDepthCamera ();
		}*/

		void Update () {
			if (!DSHUnityPlugin.Instance.bCorrectDLLloaded || VIOEmulation) {
				return;
			}

			if (!_initialized) {
				LateInitialization();
			}

			controller.UpdateVideoTexture();
			controller.UpdateThermalTexture();
			controller.UpdateDepthTexture();

			if (controller.IsIMUEnabled () && !controller.IsPoseEnabled ()) {
				controller.UpdateIMU();
			}
		}

		void LateUpdate () {
			if (!DSHUnityPlugin.Instance.bCorrectDLLloaded || VIOEmulation) {
				DisplayManager.Instance.transform.position = ServiceManager.Instance.GetPosition ();
				DisplayManager.Instance.transform.rotation = ServiceManager.Instance.GetOrientation  ();
				return;
			}

			if (!_initialized) {
				LateInitialization ();
			}

			if (controller.IsPoseEnabled ()) {
				controller.UpdatePose ();
			}

			if (controller.IsTrackingEnabled ()) {
				controller.UpdateTracking ();
			}

			finalGyro += ServiceManager.Instance.GetIMUGyro() * Time.deltaTime;

			DisplayManager.Instance.transform.position = ServiceManager.Instance.GetPosition();
			DisplayManager.Instance.transform.rotation = ServiceManager.Instance.GetOrientation ();
		}

		private void LateInitialization () {
			// Initialize the tracker late to give other classes a chance to register
			// target paths
			if (controller.HasTargetInfo () && !VIOEmulation) {
				bool status = controller.StartTracking ();
				_initialized = status;
			}
		}

		void OnApplicationQuit () {
			controller.Dispose ();
			controller.StopIMU ();
			controller.StopPositionMonitor ();

			if (controller.IsTrackingEnabled ()) {
				VisionUnityPlugin.Instance.StopAR ();
			}

			//dispose sensors
			DSHUnityPlugin.Instance.Dispose ();
	    }

		#endregion


		private void HandleControllerTargetsAvailable (object sender, EventArgs e) {
			if (TargetIdsAvailable != null) {
				TargetIdsAvailable.Invoke ();
			}
		}

		private void HandleControllerVisionParamsAvailable (object sender, EventArgs e) {
			if (VisionParametersAvailable != null) {
				VisionParametersAvailable.Invoke ();
			}
		}


		#region Service Registration

		/// <summary>
		/// Registers an object as using the video texture, and starts the video camera if required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		/// <param name="bHD">If set to <c>true</c> the camera is set to HD.</param>
		public void RegisterVideoTextureUser (System.Object user, bool bHD = false) {
			controller.RegisterVideoTextureUser (user, bHD);
		}

		/// <summary>
		/// Unregisters the video texture user and stops the video camera if no longer required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void UnregisterVideoTextureUser (System.Object user) {
			controller.UnregisterVideoTextureUser (user);
		}

		/// <summary>
		/// Registers an object as using the depth texture, and starts the depth camera if required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void RegisterDepthTextureUser (System.Object user) {
			controller.RegisterDepthTextureUser (user);
		}

		/// <summary>
		/// Unregisters the depth texture user and stops the depth camera if no longer required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void UnregisterDepthTextureUser (System.Object user) {
			controller.UnregisterDepthTextureUser (user);
		}

		/// <summary>
		/// Registers an object as using the thermal texture, and starts the thermal camera if required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void RegisterThermalTextureUser (System.Object user) {
			controller.RegisterThermalTextureUser (user);
		}

		/// <summary>
		/// Unregisters the thermal texture user and stops the thermal camera if no longer required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void UnregisterThermalTextureUser (System.Object user) {
			controller.UnregisterThermalTextureUser (user);
		}

		/// <summary>
		/// Registers an object as using the %IMU, and starts the %IMU if required.
		/// The user will not be registered if the %IMU is not available.
		/// </summary>
		/// <returns><c>true</c>, if the %IMU user was registered, <c>false</c> otherwise.</returns>
		/// <param name="user">The object using this service.</param>
		private bool RegisterIMUUser (System.Object user) {
			return controller.RegisterIMUUser (user);
		}

		/// <summary>
		/// Unregisters the %IMU user and stops the %IMU if no longer required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		private void UnregisterIMUUser (System.Object user) {
			controller.UnregisterIMUUser (user);
		}

		/// <summary>
		/// Registers an object as using the VIO, and starts VIO if needed.
		/// The user will not be registered if VIO is not available.
		/// </summary>
		/// <returns><c>true</c>, if VIO user was registered, <c>false</c> otherwise.</returns>
		/// <param name="user">The object using this service.</param>
		public bool RegisterVIOUser (System.Object user) {
			return controller.RegisterVIOUser (user);
		}

		/// <summary>
		/// Unregisters the VIO user and stops VIO if no longer required.
		/// </summary>
		/// <param name="user">The object using this service.</param>
		public void UnregisterVIOUser (System.Object user) {
			controller.UnregisterVIOUser (user);
		}

		#endregion


		#region Getters

		/// <summary>
		/// Gets the distance to the near clip plane.
		/// Anything closer to the camera than the near clip plane will not be rendered.
		/// </summary>
		/// <returns>The distance to the near clip plane.</returns>
		public float GetNearClipPlane () {
			return controller.GetNearClipPlane ();
		}

		/// <summary>
		/// Gets the distance to the far clip plane.
		/// Anything further away from the camera than the far clip plane will not be rendered.
		/// </summary>
		/// <returns>The distance to the far clip plane.</returns>
		public float GetFarClipPlane () {
			return controller.GetFarClipPlane ();
		}

		/// <summary>
		/// Checks if the vision system has initialized.
		/// </summary>
		/// <returns><c>true</c> if the vision system has initialized; otherwise, <c>false</c>.</returns>
		public bool HasVisionParameters () {
			return controller.HasVisionParameters ();
		}

		/// <summary>
		/// Gets the vision projection matrix.
		/// </summary>
		/// <returns>The vision projection matrix.</returns>
		public Matrix4x4 GetVisionProjectionMatrix () {
			return controller.GetVisionProjectionMatrix ();
		}

		/// <summary>
		/// Gets the vision field of view.
		/// </summary>
		/// <returns>The vision field of view.</returns>
		public float GetVisionFieldOfView () {
			return controller.GetVisionFieldOfView ();
		}

		/// <summary>
		/// Gets the vision aspect ratio.
		/// </summary>
		/// <returns>The vision aspect ratio.</returns>
		public float GetVisionAspectRatio () {
			return controller.GetVisionAspectRatio ();
		}

		/// <summary>
		/// Gets the color camera HD On Off status.
		/// </summary>
		/// <returns>The color camera HD On Off status.</returns>
		//[Obsolete("GetColorCameraHDOnOff is deprecated, please use ServiceManager.Instance.GetColorCamera ().GetColorCameraHDOnOffStatus ();",false)]
		public bool GetColorCameraHDOnOff () {
			return controller.GetColorCamera ().GetColorCameraHDOnOffStatus ();
		}

		/// <summary>
		/// Gets the color camera aspect ratio.
		/// </summary>
		/// <returns>The color camera aspect ratio.</returns>
		//[Obsolete("GetColorCameraAspectRatio is deprecated, please use ServiceManager.Instance.GetColorCamera ().GetAspectRatio ();",false)]
		public float GetColorCameraAspectRatio () {
			return controller.GetColorCamera ().GetAspectRatio ();
		}

		/// <summary>
		/// Gets the color camera field of view.
		/// </summary>
		/// <returns>The color camera field of view.</returns>
		//[Obsolete("GetColorCameraFieldOfView is deprecated, please use ServiceManager.Instance.GetColorCamera ().GetFieldOfView ();",false)]
		public float GetColorCameraFieldOfView () {
			return controller.GetColorCamera ().GetFieldOfView ();
		}

		/// <summary>
		/// Gets the depth camera field of view.
		/// </summary>
		/// <returns>The depth camera field of view.</returns>
		//[Obsolete("GetDepthFieldOfView is deprecated, please use ServiceManager.Instance.GetDepthCamera ().GetFieldOfView ();",false)]
		public float GetDepthCameraFieldOfView () {
			return controller.GetDepthCamera ().GetFieldOfView ();
		}

		/// <summary>
		/// Gets the depth camera aspect ratio.
		/// </summary>
		/// <returns>The depth camera aspect ratio.</returns>
		//[Obsolete("GetDepthAspectRatio is deprecated, please use ServiceManager.Instance.GetDepthCamera ().GetAspectRatio ();",false)]
		public float GetDepthCameraAspectRatio () {
			return controller.GetDepthCamera ().GetAspectRatio ();
		}

		/// <summary>
		/// Gets the thermal camera field of view.
		/// </summary>
		/// <returns>The thermal camera field of view.</returns>
		//[Obsolete("GetThermalFieldOfView is deprecated, please use ServiceManager.Instance.GetThermalSensor ().GetFieldOfView ();",false)]
		public float GetThermalCameraFieldOfView () {
			return controller.GetThermalSensor ().GetFieldOfView ();
		}

		/// <summary>
		/// Gets the thermal camera aspect ratio.
		/// </summary>
		/// <returns>The thermal camera aspect ratio.</returns>
		//[Obsolete("GetThermalAspectRatio is deprecated, please use ServiceManager.Instance.GetThermalSensor ().GetAspectRatio ();",false)]
		public float GetThermalCameraAspectRatio () {
			return controller.GetThermalSensor ().GetAspectRatio ();
		}

		/// <summary>
		/// Gets a texture that is continuously rendered using the color camera.
		/// This is only available if you've registered a color camera user.
		/// </summary>
		/// <returns>The color camera texture.</returns>
		//[Obsolete("GetColorCameraTexture is deprecated, please use ServiceManager.Instance.GetColorCamera ().GetTexture ();",false)]
		public Texture2D GetColorCameraTexture () {
			return controller.GetColorCamera ().GetTexture ();
		}

		/// <summary>
		/// Gets a texture that is continuously rendered using the thermal camera.
		/// This is only available if you've registered a thermal camera user.
		/// </summary>
		/// <returns>The thermal camera texture.</returns>
		//[Obsolete("GetThermalCameraTexture is deprecated, please use ServiceManager.Instance.GetThermalSensor ().GetTexture ();",false)]
		public Texture2D GetThermalCameraTexture () {
			return controller.GetThermalSensor ().GetTexture ();
		}

		/// <summary>
		/// Gets a texture that is continuously rendered using the depth camera.
		/// This is only available if you've registered a depth camera user.
		/// </summary>
		/// <returns>The depth camera texture.</returns>
		//[Obsolete("GetDepthCameraTexture is deprecated, please use ServiceManager.Instance.GetDepthCamera ().GetTexture ();",false)]
		public Texture2D GetDepthCameraTexture () {
			return controller.GetDepthCamera ().GetTexture ();
		}

		/// <summary>
		/// Gets color camera buffer.
		/// This is only available if you've registered a color camera user.
		/// </summary>
		/// <returns>The color camera buffer.</returns>
		//[Obsolete("GetColorCameraBuffer is deprecated, please use ServiceManager.Instance.GetColorCamera().GetBuffer() instead.",false)]
		public byte[] GetColorCameraBuffer () {
			return controller.GetColorCamera ().GetBuffer ();
		}

		/// <summary>
		/// Gets Thermal camera buffer.
		/// This is only available if you've registered a thermal camera user.
		/// </summary>
		/// <returns>The thermal camera buffer.</returns>
		//[Obsolete("GetThermalCameraBuffer is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetBuffer() instead.",false)]
		public byte[] GetThermalCameraBuffer () {
			return controller.GetThermalSensor ().GetBuffer ();
		}

		/// <summary>
		/// Gets Depth camera buffer.
		/// This is only available if you've registered a depth camera user.
		/// </summary>
		/// <returns>The depth camera Buffer.</returns>
		//[ObsoleteAttribute("This method is obsolete. Call CallNewMethod instead.", true)]
		//[Obsolete("GetDepthCameraBuffer is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetBuffer() instead.",false)]
		public byte[] GetDepthCameraBuffer () {
			return controller.GetDepthCamera ().GetBuffer ();
		}

		/// <summary>
		/// Gets color camera buffer size.
		/// This is only available if you've registered a color camera user.
		/// </summary>
		/// <returns>The color camera buffer size.</returns>
		//[Obsolete("GetColorCameraBuffer is deprecated, please use ServiceManager.Instance.GetColorCamera().GetBuffer() instead.",false)]
		public int GetColorCameraBufferSize () {
			return controller.GetColorCamera ().GetBufferSize ();
		}

		/// <summary>
		/// Gets Thermal camera buffer size.
		/// This is only available if you've registered a thermal camera user.
		/// </summary>
		/// <returns>The thermal camera buffer size.</returns>
		//[Obsolete("GetThermalCameraBuffer is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetBuffer() instead.",false)]
		public int GetThermalCameraBufferSize () {
			return controller.GetThermalSensor ().GetBufferSize ();
		}

		/// <summary>
		/// Gets Depth camera buffer size.
		/// This is only available if you've registered a depth camera user.
		/// </summary>
		/// <returns>The depth camera buffer size.</returns>
		//[ObsoleteAttribute("This method is obsolete. Call CallNewMethod instead.", true)]
		//[Obsolete("GetDepthCameraBuffer is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetBuffer() instead.",false)]
		public int GetDepthCameraBufferSize () {
			return controller.GetDepthCamera ().GetBufferSize ();
		}


		/// <summary>
		/// Gets the dimensions of the color camera image.
		/// </summary>
		/// <returns>The dimensions of the color camera image.</returns>
		//[Obsolete("GetColorCameraDimensions is deprecated, please use ServiceManager.Instance.GetColorCamera().GetFrameSize() instead.",false)]
		public Vector2 GetColorCameraDimensions() {
			return controller.GetColorCamera ().GetFrameSize ();
		}

		/// <summary>
		/// Gets the dimensions of the depth camera image.
		/// </summary>
		/// <returns>The dimensions of the depth camera image.</returns>
		//[Obsolete("GetDepthCameraDimensions is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetFrameSize() instead.",false)]
		public Vector2 GetDepthCameraDimensions() {
			return controller.GetDepthCamera ().GetFrameSize ();
		}

		/// <summary>
		/// Gets the dimensions of the thermal camera image.
		/// </summary>
		/// <returns>The dimensions of the thermal camera image.</returns>
		//[Obsolete("GetThermalCameraDimensions is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetFrameSize() instead.",false)]
		public Vector2 GetThermalCameraDimensions() {
			return controller.GetThermalSensor ().GetFrameSize ();
		}

		/// <summary>
		/// Gets the position of the color camera image from VIO IMU
		/// </summary>
		/// <returns>The position of the color camera image from VIO IMU</returns>
		//[Obsolete("GetColorCameraPose_Position is deprecated, please use ServiceManager.Instance.GetColorCamera().GetPosePosition() instead.",false)]
		public Vector3 GetColorCameraPose_Position() {
			return controller.GetColorCamera ().GetPosePosition ();
		}

		/// <summary>
		/// Gets the position of the Depth camera image from VIO IMU
		/// </summary>
		/// <returns>The position of the Depth camera image from VIO IMU</returns>
		//[Obsolete("GetDepthCameraPose_Position is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetPosePosition() instead.",false)]
		public Vector3 GetDepthCameraPose_Position() {
			return controller.GetDepthCamera ().GetPosePosition ();
		}

		/// <summary>
		/// Gets the position of the Thermal camera image from VIO IMU
		/// </summary>
		/// <returns>The position of the Thermal camera image from VIO IMU</returns>
		//[Obsolete("GetThermalCameraPose_Position is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetPosePosition() instead.",false)]
		public Vector3 GetThermalCameraPose_Position() {
			return controller.GetThermalSensor ().GetPosePosition ();
		}

		/// <summary>
		/// Gets the rotation of the color camera image from VIO IMU
		/// </summary>
		/// <returns>The rotation of the color camera image from VIO IMU</returns>
		//[Obsolete("GetColorCameraPose_Rotation is deprecated, please use ServiceManager.Instance.GetColorCamera().GetPoseRotation() instead.",false)]
		public Quaternion GetColorCameraPose_Rotation() {
			return controller.GetColorCamera ().GetPoseRotation ();
		}

		/// <summary>
		/// Gets the rotation of the Depth camera image from VIO IMU
		/// </summary>
		/// <returns>The rotation of the Depth camera image from VIO IMU</returns>
		//[Obsolete("GetDepthCameraPose_Rotation is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetPoseRotation() instead.",false)]
		public Quaternion GetDepthCameraPose_Rotation() {
			return controller.GetDepthCamera ().GetPoseRotation ();
		}

		/// <summary>
		/// Gets the rotation of the Thermal camera image from VIO IMU
		/// </summary>
		/// <returns>The rotation of the Thermal camera image from VIO IMU</returns>
		//[Obsolete("GetThermalCameraPose_ is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetPoseRotation() instead.",false)]
		public Quaternion GetThermalCameraPose_Rotation() {
			return controller.GetThermalSensor ().GetPoseRotation ();
		}

		/// <summary>
		/// Gets the Theoretical frame rate of the color camera
		/// </summary>
		/// <returns>the Theoretical frame rate of the color camera</returns>
		//[Obsolete("GetDepthCameraPose_Rotation is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetPoseRotation() instead.",false)]
		public float GetColorCameraTheoreticalRate() {
			return controller.GetColorCamera ().GetTheoreticalRate ();
		}

		/// <summary>
		/// Gets the Theoretical frame rate of the Depth camera
		/// </summary>
		/// <returns>the Theoretical frame rate of the Depth camera</returns>
		//[Obsolete("GetDepthCameraPose_Rotation is deprecated, please use ServiceManager.Instance.GetDepthCamera().GetPoseRotation() instead.",false)]
		public float GetDepthCameraTheoreticalRate() {
			return controller.GetDepthCamera ().GetTheoreticalRate ();
		}

		/// <summary>
		/// Gets the Theoretical frame rate of the Depth camera
		/// </summary>
		/// <returns>the Theoretical frame rate of the Depth camera</returns>
		//[Obsolete("GetThermalSensorPose_Rotation is deprecated, please use ServiceManager.Instance.GetThermalSensor().GetPoseRotation() instead.",false)]
		public float GetThermalCameraTheoreticalRate() {
			return controller.GetThermalSensor ().GetTheoreticalRate ();
		}

        /// <summary>
        /// Gets the color camera data format.
        /// </summary>
        /// <returns>The color camera data format.</returns>
        public DATA_FORMAT GetColorCameraDataFormat () {
            return controller.GetColorCamera ().GetDataFormat ();
        }

        /// <summary>
        /// Gets the depth camera data format.
        /// </summary>
        /// <returns>The depth camera data format.</returns>
        public DATA_FORMAT GetDepthCameraDataFormat () {
            return controller.GetDepthCamera ().GetDataFormat ();
        }

        /// <summary>
        /// Gets the thermal camera data format.
        /// </summary>
        /// <returns>The thermal camera data format.</returns>
        public DATA_FORMAT GetThermalCameraDataFormat () {
            return controller.GetThermalSensor ().GetDataFormat ();
        }

		#endregion


		#region Setters
		/*
		/// <summary>
		/// Turns the color camera autoExposure on or off.
		/// </summary>
		/// <param name="bOn">If set to <c>true</c> the histogram will be turned on.</param>
		//[Obsolete("SetAutoExposureOnOff is deprecated, please use ServiceManager.Instance.GetColorCamera().SetAutoExposure(value) instead.",false)]
		public void SetAutoExposureOnOff(bool value){
			controller.GetColorCamera().SetAutoExposure(value);
		}

		/// <summary>
		/// Turns the color camera WhiteBalance on or off.
		/// </summary>
		/// <param name="bOn">If set to <c>true</c> the histogram will be turned on.</param>
		//[Obsolete("SetWhiteBalanceOnOff is deprecated, please use ServiceManager.Instance.GetColorCamera().SetAutoWhiteBalance(value) instead.",false)]
		public void SetWhiteBalanceOnOff(bool value){
			controller.GetColorCamera().SetAutoWhiteBalance(value);
		}
		*/
		/// <summary>
		/// Turns the depth histogram on or off.
		/// </summary>
		/// <param name="bOn">If set to <c>true</c> the histogram will be turned on.</param>
		//[Obsolete("SetDepthRenderType is deprecated, please use ServiceManager.Instance.GetDepthCamera ().SetHistogram (value) instead.",false)]
		public void SetDepthRenderType(DSH_DEPTH_RENDER_TYPE type){
			controller.GetDepthCamera ().SetHistogram (type == DSH_DEPTH_RENDER_TYPE.HISTOGRAM ? true : false);
		}

		/// <summary>
		/// Sets the distance to the near clip plane.
		/// Anything closer to the camera than the near clip plane will not be rendered.
		/// </summary>
		/// <param name="nearClipPlane">Distance to the near clip plane.</param>
		public void SetNearClipPlane (float nearClipPlane) {
			controller.SetNearClipPlane (nearClipPlane);
		}

		/// <summary>
		/// Sets the distance to the far clip plane.
		/// Anything farther from the camera than the far clip plane will not be rendered.
		/// </summary>
		/// <param name="farClipPlane">Distance to the far clip plane.</param>
		public void SetFarClipPlane (float farClipPlane) {
			controller.SetFarClipPlane (farClipPlane);
		}

		#endregion


		#region Target Tracking

		/// <summary>
		/// Registers a tracking target.
		/// </summary>
		/// <param name="targetPath">The file path of the target image.</param>
		/// <param name="trackedObject">The tracked object game object.</param>
		public void RegisterTarget (string targetPath, GameObject trackedObject) {
			controller.RegisterTarget (targetPath, trackedObject);
		}

		/// <summary>
		/// Registers multiple tracking targets.
		/// </summary>
		/// <param name="_trackedObject">The tracked objects.</param>
		public void RegisterTargetSet (List<TrackedObject> _trackedObject) {
			controller.RegisterTargetSet (_trackedObject);
		}

		/// <summary>
		/// Unregisters a tracking target.
		/// </summary>
		/// <param name="targetPath">The file path of the target image.</param>
		public void UnRegisterTarget(string targetPath) {
			controller.UnRegisterTarget (targetPath);
		}

		/// <summary>
		/// Gets the internally assigned ID of a target.
		/// </summary>
		/// <returns>The target ID.</returns>
		/// <param name="targetPath">The file path of the target image.</param>
		public int GetTargetId (string targetPath) {
			return controller.GetTargetId (targetPath);
		}

		/// <summary>
		/// Activates a target, allowing the system to recognize it.
		/// </summary>
		/// <param name="targetPath">The file path of the target image.</param>
		public void ActivateTarget(string targetPath) {
			controller.ActivateTarget (targetPath);
		}

		/// <summary>
		/// Activates a target, allowing the system to recognize it.
		/// </summary>
		/// <param name="targetId">The internally assigned target ID.</param>
		public void ActivateTarget(int targetId) {
			controller.ActivateTarget (targetId);
		}

		/// <summary>
		/// Deactivates a target. 
		/// The system will no longer recognize this target.
		/// </summary>
		/// <param name="targetPath">The file path of the target image.</param>
		public void DeactivateTarget(string targetPath) {
			controller.DeactivateTarget (targetPath);
		}

		/// <summary>
		/// Deactivates a target. 
		/// The system will no longer recognize this target.
		/// </summary>
		/// <param name="targetId">The internally assigned target ID.</param>
		public void DeactivateTarget(int targetId){
			controller.DeactivateTarget (targetId);
		}

		/// <summary>
		/// Notifies the system of the real-world size of the target in meters.
		/// </summary>
		/// <param name="targetId">The internally assigned target ID.</param>
		/// <param name="width">Target width in meters.</param>
		/// <param name="height">Target height in meters.</param>
		public void SetTargetSize (int targetId, float width, float height) {
			controller.SetTargetSize (targetId, width, height);
		}

		/// <summary>
		/// Checks if a target is currently visible to the camera.
		/// </summary>
		/// <returns><c>true</c>, if the target is visible, <c>false</c> otherwise.</returns>
		/// <param name="targetId">The internally assigned target ID.</param>
		public bool GetTargetVisibility (int targetId) {
			return controller.GetTargetVisibility (targetId);
		}

		/// <summary>
		/// Gets the position of a target.
		/// </summary>
		/// <returns>The position of the target.</returns>
		/// <param name="targetId">The internally assigned target ID.</param>
		public Vector3 GetTargetPosition (int targetId) {
			return controller.GetTargetPosition (targetId);
		}

		/// <summary>
		/// Gets the orientation of a target.
		/// </summary>
		/// <returns>The orientation of the target.</returns>
		/// <param name="targetId">The internally assigned target ID.</param>
		public Quaternion GetTargetOrientation (int targetId) {
			return controller.GetTargetOrientation (targetId);
		}

		#endregion


		#region VIO/IMU

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DAQRI.ServiceManager"/> is emulating VIO.
		/// </summary>
		/// <value><c>true</c> if VIO is being emulated; otherwise, <c>false</c>.</value>
		public bool VIOEmulation {
			set { controller.SetIsEmulatingVIO (value); }
			get { return controller.IsEmulatingVIO (); }
		}

		/// <summary>
		/// Gets the position of the device.
		/// </summary>
		/// <returns>The position of the device.</returns>
		public Vector3 GetPosition () {
			return controller.GetPosition ();
		}

		/// <summary>
		/// Gets the velocity of the device.
		/// </summary>
		/// <returns>The velocity of the device.</returns>
		public Vector3 GetWorldVelocity () {
			return controller.GetWorldVelocity ();
		}

		/// <summary>
		/// Gets the orientation of the device.
		/// </summary>
		/// <returns>The orientation of the device.</returns>
		public Quaternion GetOrientation () {
			return controller.GetOrientation ();
		}

		/// <summary>
		/// Checks if the %IMU has started and successfully returned data.
		/// </summary>
		/// <returns><c>true</c> if %IMU data is available; otherwise, <c>false</c>.</returns>
		public bool HasIMUData () {
			return controller.HasIMUData ();
		}

		/// <summary>
		/// Checks if the VIO %IMU has started and successfully returned pose data.
		/// </summary>
		/// <returns><c>true</c> if pose data is available; otherwise, <c>false</c>.</returns>
		public bool HasPoseData () {
			return controller.HasPoseData ();
		}

		/// <summary>
		/// Gets the latest %IMU gyro value.
		/// </summary>
		/// <returns>The %IMU gyro value.</returns>
		public Vector3 GetIMUGyro () {
			return controller.GetIMUGyro ();
		}

		/// <summary>
		/// Gets the latest %IMU rotation value.
		/// </summary>
		/// <returns>The %IMU rotation value.</returns>
		public Quaternion GetIMUQuaternion () {
			return controller.GetIMUQuaternion ();
		}

		/// <summary>
		/// Gets the latest %IMU magnetic field value.
		/// </summary>
		/// <returns>The %IMU magnetic field value.</returns>
		public Vector3 GetIMUMagneticField () {
			return controller.GetIMUMagneticField ();
		}

		/// <summary>
		/// Sets simulated data for the %IMU.
		/// </summary>
		/// <param name="gyro">Simulated %IMU gyro value.</param>
		/// <param name="quat">Simulated %IMU rotation value.</param>
		public void SimulateIMU (Vector3 gyro, Quaternion quat) {
			controller.SimulateIMU (gyro, quat);
		}

		/// <summary>
		/// Sets a simulated position for the %IMU.
		/// </summary>
		/// <param name="position">Simulated %IMU position.</param>
		public void SimulateIMUPosition (Vector3 position) {
			controller.SimulateIMUPosition (position);
		}

		#endregion


		#region Utility

		/// <summary>
		/// Creates a Unity Vector3 from an array of floats.
		/// </summary>
		/// <returns>A new <see cref="Vector3"/> with the given values.</returns>
		/// <param name="values">Array of three floats.</param>
		public static Vector3 Vector3FromFloatArray(float[] values) {
			if (values == null || values.Length < 3) throw new ArgumentException("Expected 3 elements in values array", "values");

			Vector3 v = new Vector3();
			for (int i = 0; i < 3; i++) v[i] = values[i];
			return v;
		}

		/// <summary>
		/// Creates a Unity Quaternion from an array of floats.
		/// </summary>
		/// <returns>A new <see cref="Quaternion"/> with the given values.</returns>
		/// <param name="values">Array of four floats to populate the quaternion..</param>
		public static Quaternion QuaternionFromFloatArray(float[] values) {
			if (values == null || values.Length < 4) throw new ArgumentException("Expected 4 elements in values array", "values");

			Quaternion q = new Quaternion();
			for (int i = 0; i < 4; i++) q[i] = values[i];
			return q;
		}

		#endregion
	}
}
