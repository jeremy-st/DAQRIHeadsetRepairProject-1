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
using System.Collections.Generic;

namespace DAQRI {
	
	public interface IServiceManagerController {

		event System.EventHandler OnTargetIdsAvailable;
		event System.EventHandler OnVisionParametersAvailable;


		#region Service Registration

		void RegisterVideoTextureUser (System.Object user, bool bHD);

		void UnregisterVideoTextureUser (System.Object user);

		void RegisterDepthTextureUser (System.Object user);

		void UnregisterDepthTextureUser (System.Object user);

		void RegisterThermalTextureUser (System.Object user);

		void UnregisterThermalTextureUser (System.Object user);

		bool RegisterIMUUser (System.Object user);

		void UnregisterIMUUser (System.Object user);

		bool RegisterVIOUser (System.Object user);

		void UnregisterVIOUser (System.Object user);

		#endregion


		#region Target Tracking

		bool HasTargetInfo ();

		void RegisterTarget (string targetPath, GameObject trackedObject);

		void RegisterTargetSet (List<TrackedObject> _trackedObject);

		void UnRegisterTarget (string targetPath);

		int GetTargetId (string targetPath);

		void ActivateTarget (string targetPath);

		void ActivateTarget (int targetId);

		void DeactivateTarget (string targetPath);

		void DeactivateTarget (int targetId);

		void SetTargetSize (int targetId, float width, float height);

		bool GetTargetVisibility (int targetId);

		Vector3 GetTargetPosition (int targetId);

		Quaternion GetTargetOrientation (int targetId);

		void UpdateTracking ();

		#endregion


		#region Sensors

		void Dispose () ;

		ColorCamera GetColorCamera ();

		DepthCamera GetDepthCamera ();

		ThermalSensor GetThermalSensor();

		bool StartTracking ();

		void StopTracking ();

		bool StartIMU ();

		void StopIMU ();

		bool StartPositionMonitor ();

		void StopPositionMonitor ();

		#endregion


		#region State

		bool IsIMUEnabled ();

		bool IsPoseEnabled ();

		bool IsTrackingEnabled ();

		bool IsVIOIMUEnabled ();

		#endregion


		#region Accessors

		float GetNearClipPlane ();

		float GetFarClipPlane ();

		bool HasVisionParameters ();

		Matrix4x4 GetVisionProjectionMatrix ();

		float GetVisionFieldOfView ();

		float GetVisionAspectRatio ();

		#endregion


		#region Setters
		void SetColorCameraWhiteBalanceOnOff (bool value);

		void SetColorCameraAutoExposureOnOff (bool value);

		void SetDepthRenderType (DSH_DEPTH_RENDER_TYPE type);

		void SetNearClipPlane (float nearClipPlane);

		void SetFarClipPlane (float farClipPlane);

		#endregion


		#region VIO/IMU

		bool IsEmulatingVIO ();

		void SetIsEmulatingVIO (bool isEmulating);

		bool HasIMUData ();

		bool HasPoseData ();

		Vector3 GetPosition ();

		Vector3 GetWorldVelocity ();

		Vector3 GetIMUGyro ();

		Quaternion GetOrientation ();

		Quaternion GetIMUQuaternion ();

		Vector3 GetIMUMagneticField ();

		void UpdateIMU ();

		void UpdatePose ();

		void SimulateIMU (Vector3 gyro, Quaternion quat);

		void SimulateIMUPosition (Vector3 position);

		#endregion


		#region Updates

		void UpdateVideoTexture ();

		void UpdateDepthTexture ();

		void UpdateThermalTexture ();

		#endregion
	}
}
