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
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DAQRI {

	public class ServiceManagerController: IServiceManagerController {

		/*ColorCameraNativeCall colorCameraNativeCalls = new ColorCameraNativeCall (DEVICE_IDENTIFIER.FRONT_CAMERA);
		DepthCameraNativeCall depthCameraNativeCalls = new DepthCameraNativeCall (DEVICE_IDENTIFIER.DEPTH_SENSOR);
		ThermalSensorNativeCall thermalCameraNativeCalls = new ThermalSensorNativeCall (DEVICE_IDENTIFIER.LONG_WAVE_IR);*/


		private ColorCamera colorCamera = null;
		private DepthCamera depthCamera = null;
		private ThermalSensor thermalCamera = null;

		public event EventHandler OnTargetIdsAvailable;
		public event EventHandler OnVisionParametersAvailable;

		private IDSHUnityPlugin dshPlugin;
		private IVisionUnityPlugin visionPlugin;

		public bool enableTracking = false;
		public bool enableIMU = false;
		public bool enablePose = false;

		/// <summary>
		/// performance mode uses VIN Imu
		/// </summary>
		public bool bVIOMode = true;

		// this is hardcoded temporarily
		//private string _cameraParameter = "camera_para_realsense_tusi_640x480-updated";
		public float nearPlane = 0.02f;
		public float farPlane = 1000.0f;

		public bool hasVisionParameters = false;
		public Matrix4x4 visionProjectionMatrix = Matrix4x4.identity;
		public float visionFieldOfView = 43.28f;
		public float visionAspectRatio = 1.77f;

		public bool hasIMUData = false;
		public bool hasPoseData = false;
		public Vector3 imuGyro = Vector3.zero;
		public Quaternion imuQuaternion = Quaternion.identity;
		public Vector3 imuMagneticField = Vector3.zero;
		public Vector3 imuWorldPosition = Vector3.zero;
		public Vector3 imuWorldVelocity = Vector3.zero;

		public Vector3 prevPos;
		public Vector4 prevOrientation;

		public bool useNativeGLTexture = true;
		private readonly int[] _markerIds = new int[1];

		public class Target {
			public int id;
			public TargetInfo info;
			public float width;
			public float height;
			public bool isVisible;
			public bool isTrackable;
			public Vector3 position;
			public Quaternion orientation;
		}

		public struct TargetInfo : IEquatable<TargetInfo> {
			public string path;
			public GameObject trackableObject;
			public bool Equals(TargetInfo _path) {
				return (this.path.Equals (_path.path));
			}
		}

		public List<TargetInfo> targetInfos = new List<TargetInfo> ();
		public Dictionary<int, Target> targets = new Dictionary<int, Target> ();

		public HashSet<System.Object> videoBackgroundUsers = new HashSet<System.Object> ();
		public HashSet<System.Object> depthBackgroundUsers = new HashSet<System.Object> ();
		public HashSet<System.Object> thermalBackgroundUsers = new HashSet<System.Object> ();
		public HashSet<System.Object> imuUsers = new HashSet<System.Object> ();
		public HashSet<System.Object> vioUsers = new HashSet<System.Object> ();

		//private IMUData imudata = new IMUData ();
		private VIOData viodata = new VIOData ();
		public bool bVIOEmulation;

		private IRunEnvironmentInfo _runEnvironment = new RunEnvironmentInfo ();
		public void SetRunEnvironmentInfo (IRunEnvironmentInfo info) {
			_runEnvironment = info;
		}

		#region Setup

		public ServiceManagerController (IDSHUnityPlugin dshPlugin, IVisionUnityPlugin visionPlugin) {
			this.dshPlugin = dshPlugin;
			this.visionPlugin = visionPlugin;

			visionProjectionMatrix = Matrix4x4.Perspective (visionFieldOfView, visionAspectRatio, nearPlane, farPlane);


			#region colorcamera_setup

			colorCamera = new ColorCamera (DEVICE_IDENTIFIER.FRONT_CAMERA,
				"Texture/colordefault");

			colorCamera.startDeviceFunc 		    = DSHUnityAbstraction.L7_CameraStart;
			colorCamera.stopDeviceFunc 			= DSHUnityAbstraction.L7_CameraStop;
			colorCamera.getEffectiveRateFunc 	= DSHUnityAbstraction.L7_CameraGetEffectiveRate;
			colorCamera.getTheoreticalRateFunc 	= DSHUnityAbstraction.L7_CameraGetRate;
			colorCamera.getDataFormatFunc 		= DSHUnityAbstraction.L7_CameraGetDataFormat;

			colorCamera.getParametersFunc 		= DSHUnityAbstraction.L7_CameraGetParams;
			colorCamera.getPoseFunc 				= DSHUnityAbstraction.L7_CameraGetPose;
			colorCamera.setRenderEventTextureIDFunc = DSHUnityAbstraction.SetUnityColorRenderEventTextureID;
			colorCamera.getRenderEventFunc 		= DSHUnityAbstraction.GetRenderEventFunc;
			colorCamera.getCameraDataFunc 		= DSHUnityAbstraction.L7_CameraGetData;

			colorCamera.setAutoExposureFunc 		= DSHUnityAbstraction.L7_CameraSetAutoExposureEnabled;
			colorCamera.setAutoWhiteBalanceFunc 	= DSHUnityAbstraction.L7_CameraSetAutoWhiteBalanceEnabled;
			colorCamera.setExposureFunc 			= DSHUnityAbstraction.L7_CameraSetExposure;
			colorCamera.setWhiteBalanceFunc 		= DSHUnityAbstraction.L7_CameraSetWhiteBalance;
			colorCamera.updatePresetFunc 		= DSHUnityAbstraction.L7_CameraUpdatePreset;

			//colorCamera.SetAutoExposure (true);
			//colorCamera.SetAutoWhiteBalance (true);

			#endregion

			#region depthcamera_setup

			depthCamera = new DepthCamera (DEVICE_IDENTIFIER.DEPTH_SENSOR,
				"Texture/depthdefault");
			
			depthCamera.startDeviceFunc = DSHUnityAbstraction.L7_DepthStart;
			depthCamera.stopDeviceFunc = DSHUnityAbstraction.L7_DepthStop;
			depthCamera.getEffectiveRateFunc = DSHUnityAbstraction.L7_DepthGetEffectiveRate;
			depthCamera.getTheoreticalRateFunc = DSHUnityAbstraction.L7_DepthGetRate;
			depthCamera.getDataFormatFunc = DSHUnityAbstraction.L7_DepthGetDataFormat;

			depthCamera.getParametersFunc = DSHUnityAbstraction.L7_DepthGetParams;
			depthCamera.getRenderEventFunc = DSHUnityAbstraction.GetRenderEventFunc;
			depthCamera.getPoseFunc = DSHUnityAbstraction.L7_CameraGetPose;

			depthCamera.setRenderEventTextureIDFunc = DSHUnityAbstraction.SetUnityDepthRenderEventTextureID;
			depthCamera.updatePresetFunc = DSHUnityAbstraction.L7_DepthUpdatePreset;
			depthCamera.getCameraDataFunc = DSHUnityAbstraction.L7_DepthGetData;

			#endregion

			#region thermakcamera_setup

			thermalCamera = new ThermalSensor (DEVICE_IDENTIFIER.LONG_WAVE_IR,
				"Texture/thermaldefault");
			
			thermalCamera.startDeviceFunc = DSHUnityAbstraction.L7_ThermalStart;
			thermalCamera.stopDeviceFunc = DSHUnityAbstraction.L7_ThermalStop;
			thermalCamera.getParametersFunc = DSHUnityAbstraction.L7_ThermalGetParams;
			thermalCamera.setRenderEventTextureIDFunc = DSHUnityAbstraction.SetUnityThermalRenderEventTextureID;
			thermalCamera.getRenderEventFunc = DSHUnityAbstraction.GetRenderEventFunc;

			thermalCamera.getEffectiveRateFunc = DSHUnityAbstraction.L7_ThermalGetEffectiveRate;
			thermalCamera.getTheoreticalRateFunc = DSHUnityAbstraction.L7_ThermalGetRate;
			thermalCamera.getDataFormatFunc = DSHUnityAbstraction.L7_ThermalGetDataFormat;
			thermalCamera.getCameraDataFunc = DSHUnityAbstraction.L7_ThermalGetData;
			thermalCamera.getPoseFunc = DSHUnityAbstraction.L7_ThermalGetPose;

			#endregion
		}

		#endregion


		#region Service Registration

		bool bIsHD = false;
		public void RegisterVideoTextureUser (System.Object user, bool bHD) {
			if ((user == null || videoBackgroundUsers.Contains (user)) && bHD == bIsHD) {
				return;
			}

			if (videoBackgroundUsers.Count == 0) {
				if (bHD) {
					colorCamera.StartDevice ((int)DSH_COLOR_CAMERA_PRESET.RGB_1080p);
				} else {
					colorCamera.StartDevice ((int)DSH_COLOR_CAMERA_PRESET.RGB_480p);
				}
			} else {
				if (bHD && !bIsHD) {
					colorCamera.StartDevice ((int)DSH_COLOR_CAMERA_PRESET.RGB_1080p);
				} else if (!bHD && bIsHD) {
					colorCamera.StartDevice ((int)DSH_COLOR_CAMERA_PRESET.RGB_480p);
				}
			}

			bIsHD = bHD;
			videoBackgroundUsers.Add (user);
		}

		public void UnregisterVideoTextureUser (System.Object user) {
			if (user == null || !videoBackgroundUsers.Contains (user)) {
				return;
			}

			videoBackgroundUsers.Remove (user);

			if (videoBackgroundUsers.Count == 0) {
				colorCamera.StopDevice ();
			}
		}

		public void RegisterDepthTextureUser (System.Object user) {
			if (user == null || depthBackgroundUsers.Contains (user)) {
				return;
			}

			if (depthBackgroundUsers.Count == 0) {
				depthCamera.StartDevice ((int)DSH_COLOR_CAMERA_PRESET.RGB_480p);
			}

			depthBackgroundUsers.Add (user);
		}

		public void UnregisterDepthTextureUser (System.Object user) {
			if (user == null || !depthBackgroundUsers.Contains (user)) {
				return;
			}

			depthBackgroundUsers.Remove (user);

			if (depthBackgroundUsers.Count == 0) {
				//Debug.Log ("ServiceManager: Disabling depth texture updates");
				depthCamera.StopDevice();
			}
		}

		public void RegisterThermalTextureUser (System.Object user) {
			if (user == null || thermalBackgroundUsers.Contains (user)) {
				return;
			}

			if (thermalBackgroundUsers.Count == 0) {
				thermalCamera.StartDevice ();
			}

			thermalBackgroundUsers.Add (user);
		}
		
		public void UnregisterThermalTextureUser (System.Object user) {
			if (user == null || !thermalBackgroundUsers.Contains (user)) {
				return;
			}

			thermalBackgroundUsers.Remove (user);

			if (thermalBackgroundUsers.Count == 0) {
				thermalCamera.StopDevice ();
			}
		}

		public bool RegisterIMUUser (System.Object user) {
			if (user == null || imuUsers.Contains (user)) {
				return false;
			}

			if (imuUsers.Count == 0) {
				StartIMU ();// (vioIMU == true) ? (int)DSHUnityPlugin.DeviceOptimization.PERFORMANCE : (int)DSHUnityPlugin.DeviceOptimization.LOW_POWER); //default it starts in low power mode
			}

			imuUsers.Add (user);
			return enableIMU;
		}
		
		public void UnregisterIMUUser (System.Object user) {
			if (user == null || !imuUsers.Contains (user)) {
				return;
			}

			imuUsers.Remove (user);
			if (imuUsers.Count == 0) {
				StopIMU ();
			}
		}

		public bool RegisterVIOUser (System.Object user) {
			if (user == null || vioUsers.Contains (user)) {
				return false;
			}

			if (vioUsers.Count == 0) {
				if (_runEnvironment.CurrentEnvironment () != RunEnvironmentType.EditorSmartGlassesPreview) {
					StartIMU ();
				}

				StartPositionMonitor();
			}

			vioUsers.Add (user);
			return enablePose;
		}

		public void UnregisterVIOUser (System.Object user) {
			if (user == null || !vioUsers.Contains (user)) {
				return;
			}

			vioUsers.Remove (user);
			if (vioUsers.Count == 0) {
				StopIMU ();
				StopPositionMonitor();
			}
		}

		#endregion


		#region Target Tracking

		public bool HasTargetInfo () {
			return (targetInfos.Count > 0);
		}

		public void RegisterTarget (string targetPath, GameObject trackedObject) {
			bool duplicate = false;
			for (int i = 0; i < targetInfos.Count; i++) {
				if (targetInfos [i].path.Equals (targetPath)) {
					duplicate = true;
				}
			}

			if (!duplicate) {
				if (System.IO.File.Exists (targetPath)) {
					TargetInfo _targetinfo;
					_targetinfo.path = targetPath;
					_targetinfo.trackableObject = trackedObject;
					targetInfos.Add (_targetinfo);

				} else {
					Debug.LogWarning ("Could not load the trackable image from " + targetPath + ". File could have moved or corrupted. Please reload the image in TO_" + targetPath.Split ('.') [0] + " trackable object");
				}

			} else {
				Debug.LogWarning (targetPath + " is already registered");
			}
		}

		public void RegisterTargetSet (List<TrackedObject> _trackedObject) {
			StopTracking ();
			RemoveAllTrackable ();
			foreach (TrackedObject trackedObject in _trackedObject) {
				TargetInfo _targetinfo;
				_targetinfo.path = trackedObject.targetPath;
				_targetinfo.trackableObject = trackedObject.gameObject;
				targetInfos.Add (_targetinfo);
			}

			StartTracking ();
		}

		public void UnRegisterTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			if (targetId != -1) {
				targets.Remove (targetId);

			} else {
				//Debug.LogWarning ("could not unregister " + targetPath);
			}
		}

		public int GetTargetId (string targetPath) {
			foreach (Target target in targets.Values) {
				if (target.info.path == targetPath) {
					return target.id;
				}
			}
			return -1;
		}

		public void ActivateTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			ActivateTarget (targetId);
		}

		public void ActivateTarget(int targetId) {
			if (targetId != -1 && targets.ContainsKey (targetId)) {
				targets [targetId].isTrackable = true;
			} else {
				//Debug.LogWarning ("Target ID is invalid");
			}
		}

		public void DeactivateTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			DeactivateTarget (targetId);
		}

		public void DeactivateTarget(int targetId){
			if (targetId != -1 && targets.ContainsKey (targetId)) {
				targets [targetId].isTrackable = false;
			} else {
				//Debug.LogWarning ("Target ID is invalid");
			}
		}

		private void RemoveAllTrackable () {
			targetInfos.Clear ();
			targets.Clear ();
		}

		public void SetTargetSize (int targetId, float width, float height) {
			Target target;
			if (targets.TryGetValue (targetId, out target)) {
				target.width = width;
				target.height = height;
			}
		}

		public bool GetTargetVisibility (int targetId) {
			Target target;
			if (targets.TryGetValue (targetId, out target)) {
				return target.isVisible;
			}
			return false;
		}

		public Vector3 GetTargetPosition (int targetId) {
			Target target;
			if (targets.TryGetValue (targetId, out target)) {
				return target.position;
			}
			return Vector3.zero;
		}

		public Quaternion GetTargetOrientation (int targetId) {
			Target target;
			if (targets.TryGetValue (targetId, out target)) {
				return target.orientation;
			}
			return Quaternion.identity;
		}

		public void UpdateTracking () {
			visionPlugin.UpdateAR ();

			//Set all the targets to be invisible
			foreach (var entry in targets) {
				Target target = entry.Value;
				int targetId = entry.Key;

				Vector3 position = Vector3.zero;
				Quaternion orientation = Quaternion.identity;
				bool visible = visionPlugin.GetPose (ref position, ref orientation, targetId);

				if (visible) {
					//float scale = Math.Min (target.width, target.height);
					target.isVisible = true;
					target.position = position;
					target.orientation = orientation;
				} else {
					target.isVisible = false;
					target.position = Vector3.zero;
					target.orientation = Quaternion.identity;
				}
			}
		}

		#endregion


		#region Sensors

		public void Dispose(){
			colorCamera.StopDevice();
			thermalCamera.StopDevice();
			depthCamera.StopDevice();
		}

		public ColorCamera GetColorCamera(){
			return colorCamera;
		}

		public DepthCamera GetDepthCamera(){
			return depthCamera;
		}

		public ThermalSensor GetThermalSensor(){
			return thermalCamera;
		}

		public bool StartTracking () {
			bool status = InitTracking ();
			enableTracking = status;
			return status;
		}

		public void StopTracking () {
			RemoveAllTrackable ();
			visionPlugin.StopAR ();
			enableTracking = false;
		}

		public bool StartIMU () {
			bool status = dshPlugin.StartIMU ();
			if (status) {
				enableIMU = true;
			}
			return status;
		}

		public void StopIMU () {
			dshPlugin.StopIMU ();
			enableIMU = false;
			hasIMUData = false;
		}

		public bool StartPositionMonitor () {
			bool status = dshPlugin.StartPositionMonitor();
			if (status) {
				enablePose = true;
			}
			return status;
		}

		public void StopPositionMonitor () {
			dshPlugin.StopPositionMonitor ();
			enablePose = false;
			hasPoseData = false;
		}

		#endregion


		#region State

		public bool IsIMUEnabled () {
			return enableIMU;
		}

		public bool IsPoseEnabled () {
			return enablePose;
		}

		public bool IsTrackingEnabled () {
			return enableTracking;
		}

		public bool IsVIOIMUEnabled () {
			return bVIOMode;
		}

		#endregion


		#region Accessors

		public float GetNearClipPlane () {
			return nearPlane;
		}

		public float GetFarClipPlane () {
			return farPlane;
		}

		public bool HasVisionParameters () {
			return hasVisionParameters;
		}

		public Matrix4x4 GetVisionProjectionMatrix () {
			return visionProjectionMatrix;
		}

		public float GetVisionFieldOfView () {
			return visionFieldOfView;
		}

		public float GetVisionAspectRatio () {
			return visionAspectRatio;
		}

		#endregion


		#region Setters

		public void SetColorCameraAutoExposureOnOff(bool value){
			colorCamera.SetAutoExposure (value);
		}

		public void SetColorCameraWhiteBalanceOnOff(bool value){
			colorCamera.SetAutoWhiteBalance (value);
		}

		public void SetDepthRenderType(DSH_DEPTH_RENDER_TYPE type)
		{
			if (type == DSH_DEPTH_RENDER_TYPE.RAW) {
				depthCamera.SetHistogram (false);
			} else if(type == DSH_DEPTH_RENDER_TYPE.HISTOGRAM){
				depthCamera.SetHistogram (true);
			}
		}

		public void SetNearClipPlane (float nearClipPlane) {
			this.nearPlane = nearClipPlane;
		}

		public void SetFarClipPlane (float farClipPlane) {
			this.farPlane = farClipPlane;
		}

		#endregion


		#region VIO/IMU

		public bool IsEmulatingVIO () {
			return bVIOEmulation;
		}

		public void SetIsEmulatingVIO (bool isEmulating) {
			bVIOEmulation = isEmulating;
		}

		public bool HasIMUData () {
			return hasIMUData;
		}

		public bool HasPoseData () {
			return hasPoseData;
		}

		public Vector3 GetPosition () {
			return imuWorldPosition;
		}

		public Vector3 GetWorldVelocity () {
			return imuWorldVelocity;
		}

		public Vector3 GetIMUGyro () {
			return imuGyro;
		}

		public Quaternion GetOrientation () {
			// return orientation from VIO eventually
			// for now the quaternion from the %IMU is a good stand in
			return imuQuaternion;
		}

		public Quaternion GetIMUQuaternion () {
			return imuQuaternion;
		}

		public Vector3 GetIMUMagneticField() {
			return imuMagneticField;
		}

		float[] quatTemp = new float[4];
		float[] gyroTemp = new float[3];
		float[] magneticfieldTemp = new float[3];

		public void UpdateMagneticField(){
			dshPlugin.GetIMUMagneticField (magneticfieldTemp);
			imuMagneticField.x = magneticfieldTemp[0];
			imuMagneticField.y = magneticfieldTemp[1];
			imuMagneticField.z = magneticfieldTemp[2];
		}

		public void UpdateIMU () {
			dshPlugin.GetIMUQuaternion (quatTemp);
			imuQuaternion.x = quatTemp [0];
			imuQuaternion.y = quatTemp [1];
			imuQuaternion.z = quatTemp [2];
			imuQuaternion.w = quatTemp [3];

			dshPlugin.GetIMUGyro (gyroTemp);
			imuGyro.x = gyroTemp[0];
			imuGyro.y = gyroTemp[1];
			imuGyro.z = gyroTemp[2];

			hasIMUData = true;
		}

		public void UpdatePose() {
			if (dshPlugin.GetVIOData (ref viodata)) {
				imuQuaternion = viodata.Quat;
				imuWorldPosition = viodata.WorldPosition;
				imuWorldVelocity = viodata.WorldVelocity;
				hasPoseData = true;
			}

			if (enableIMU) {
				UpdateMagneticField ();
			}

			if (isVIODataInvalid () && enableIMU) {
				UpdateIMU ();
			}
		}

		public void SimulateIMU (Vector3 gyro, Quaternion quat) {
			imuGyro = gyro;
			imuQuaternion = quat;
			hasIMUData = true;
		}

		public void SimulateIMUPosition (Vector3 position) {
			imuWorldPosition = position;
		}

		Quaternion prevQuat;
		int sameCount = 0;
		//TODO: Temp fallback this should happen on native side
		public virtual bool isVIODataInvalid () {
			if (imuQuaternion.x == prevQuat.x && imuQuaternion.y == prevQuat.y && imuQuaternion.z == prevQuat.z && imuQuaternion.w == prevQuat.w) {
				sameCount++;
				if (sameCount > 10) {
					bVIOMode = false;
					sameCount = 10;
				}

			} else {
				sameCount--;
				if (sameCount < 0) {
					bVIOMode = true;
					sameCount = 0;
				}
			}

			prevQuat = imuQuaternion;
			if (sameCount == 10) {
				return true;
			}

			return false;
		}

		#endregion


		#region Updates

		public void UpdateVideoTexture () {
			colorCamera.UpdateDevice ();
		}

		public void UpdateDepthTexture () {
			depthCamera.UpdateDevice ();
		}

		public void UpdateThermalTexture () {
			thermalCamera.UpdateDevice ();
		}

		#endregion


		#region Helper Methods

		private bool InitTracking () {
			if (targetInfos.Count == 0) {
				return false;
			}

			int numOfValidMarkers = targetInfos.Count;

			int[] markerIds = new int[numOfValidMarkers]; //TODO : should be move ouside of this function
			float[] markerWidths = new float[numOfValidMarkers]; //TODO : should be move ouside of this function
			float[] markerHeights = new float[numOfValidMarkers]; //TODO : should be move ouside of this function

			for (int index = 0; index < numOfValidMarkers; index++) {
				TrackedObject toObj = targetInfos [index].trackableObject.GetComponent<TrackedObject>();
				markerWidths [index] = toObj.WidthInMeters;
				markerHeights [index] = toObj.HeightInMeters;
			}

			visionPlugin.InitAR (targetInfos.Select(x=>x.path).ToArray(),numOfValidMarkers,markerWidths,markerHeights, markerIds);

			for (int index = 0; index < numOfValidMarkers; index++) {
				Target target = new Target ();
				target.id = _markerIds [index];
				target.info = targetInfos [index];
				Debug.Log (targetInfos [index].path);
				target.isVisible = false;
				target.isTrackable = true;
				if(!targets.ContainsKey(target.id))
					targets.Add (target.id, target);
			}

			visionProjectionMatrix = dshPlugin.GetProjectionMatrix(ProjectionMatrixEye.Mono);

			visionFieldOfView = visionProjectionMatrix.ProjectionFieldOfView ();
			visionAspectRatio = visionProjectionMatrix.ProjectionAspectRatio ();
			hasVisionParameters = true;

			if (OnTargetIdsAvailable != null) {
				OnTargetIdsAvailable (this, null);
			}

			if (OnVisionParametersAvailable != null) {
				OnVisionParametersAvailable (this, null);
			}

			return true;
		}

		#endregion
	}
}
