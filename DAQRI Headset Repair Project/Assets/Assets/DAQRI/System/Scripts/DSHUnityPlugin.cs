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

namespace DAQRI {
	

	public partial class DSHUnityPlugin : IDSHUnityPlugin {

		// Constants
		private const string GLASSES_THERMAL_CALL_ERROR = "The DAQRI Smart Glasses do not support thermal APIs. Please ensure your code uses the correct guards to be cross-device compatible.";
		private const string GLASSES_IMU_CALL_ERROR = "The DAQRI Smart Glasses do not support IMU APIs. Please use the VIO APIs, and ensure your code uses the correct guards to be cross-device compatible.";

		private IRunEnvironmentInfo runEnvironment = new RunEnvironmentInfo ();

		// Singleton
		private static DSHUnityPlugin instance;
		public static DSHUnityPlugin Instance {
			get {
				if (instance == null) {
					instance = new DSHUnityPlugin ();
				}

				return instance;
			}
		}

		//for tracking
		public bool enableGrayScale = true;

		private DSH_HCAMERA_TYPE camera_type;

		public bool bCorrectDLLloaded = true;


		#region Video Camera

		public bool Initialize() {
			bool ok = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				const string MethodName = "Initialize";
				try {
					ok = DSHUnityAbstraction.L7_Initialize ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
					return ok;
				}
				catch(EntryPointNotFoundException e){
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
					return ok;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
					return ok;
				}

				if (!ok) {
					Debug.LogWarning("Initalize returned false, most likely because plugin is already initialized");
				}
				break;

			default:
				break;
			}

			return ok;
		}

		public bool Dispose() {
			bool ok = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				const string MethodName = "Dispose";
				try {
					ok = DSHUnityAbstraction.L7_Dispose ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
					return ok;
				}
				catch(EntryPointNotFoundException e){
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
					return ok;
				}
				catch(Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
					return ok;
				}
				break;

			default:
				break;
			}

			return ok;
		}

		#endregion

		#region Depth Camera

		#endregion

		#region IMU

		public bool StartIMU() {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "IMU start";
				try {
					status = DSHUnityAbstraction.L7_IMUStart ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(EntryPointNotFoundException e){
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return status;
		}

		public bool StopIMU() {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				// Uncomment when we have the ability to check device type
//				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "IMU stop";
				try {
					status = DSHUnityAbstraction.L7_IMUStop ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(EntryPointNotFoundException e){
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return status;
		}

		public void GetIMURate(out float rate) {
			rate = 0.0f;
			getRate (DEVICE_IDENTIFIER.IMU_SENSOR, out rate);
		}
		public DATA_FORMAT GetIMUDataFormat () {
			return getDataFormat (DEVICE_IDENTIFIER.IMU_SENSOR);
		}

		public bool StartPositionMonitor() {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				const string MethodName = "Position monitor start";
				try {
					status = DSHUnityAbstraction.L7_PositionMonitorStart ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(EntryPointNotFoundException e){
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;

			default:
				break;
			}

			return status;
		}

		public bool StopPositionMonitor() {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				const string MethodName = "Position monitor stop";
				try {
					status = DSHUnityAbstraction.L7_PositionMonitorStop ();
				}
				catch(DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch(Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;

			default:
				break;
			}

			return status;
		}

		public bool GetIMUQuaternion(float[] quat) {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				// Uncomment when we have the ability to check device type
//				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				status = DSHUnityAbstraction.L7_IMUGetRotation (quat);
				if (!status) {
					Debug.Log ("Issues in grabbing rotation data from IMU");
				}
				break;
			}

			return status;
		}

		public bool GetIMUMagneticField(float[] magneticfield) {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				// Uncomment when we have the ability to check device type
				//				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				status = DSHUnityAbstraction.L7_IMUGetMagneticField (magneticfield);
				if (!status) {
					Debug.Log ("Issues in grabbing magnetic field from IMU");
				}
				break;
			}

			return status;
		}

		public bool GetIMUAcceleration(float[] accel) {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				// Uncomment when we have the ability to check device type
//				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				status = DSHUnityAbstraction.L7_IMUGetAcceleration (accel);
				if (!status) {
					Debug.Log ("Issues in grabbing acceleration data from IMU");
				}
				break;
			}

			return status;
		}

		public bool GetIMUGyro(float[] gyro) {
			bool status = false;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				// Uncomment when we have the ability to check device type
//				Debug.LogWarning (GLASSES_IMU_CALL_ERROR);
				break;

			case (RunEnvironmentType.OnDevice):
				status = DSHUnityAbstraction.L7_IMUGetGyro (gyro);
				if (!status) {
					Debug.Log ("Issues in grabbing gyro data from IMU");
				}
				break;
			}

			return status;
		}

		/*public bool GetIMUData(ref IMUData imuData) {
			return false;
		}*/


		float[] pos = new float[3];
		float[] rot = new float[4];
		public bool GetVIOData(ref VIOData vioData) {
			bool status = DSHUnityAbstraction.L7_PositionMonitorUpdate (0.03f); 
			if (status) {
				DSHUnityAbstraction.L7_PositionMonitorGetPosition (pos);
				DSHUnityAbstraction.L7_PositionMonitorGetRotation (rot);
				vioData.SetNativeData (pos, rot);
			} else {
				Debug.Log ("Issues in grabbing VIO data");
			}
			return status;
		}

	    #endregion

		#region Thermal Camera

        #endregion

        /// <summary>Retrieves the <see cref="T:Matrix4x4"/> to be used for the <see cref="T:Camera"/>'s projections.</summary>
        /// <param name="eye"></param>
        public Matrix4x4 GetProjectionMatrix(ProjectionMatrixEye eye) {
            retrievedProjectionMatrices();
            return projectionMatrices[(int)eye];
        }

        /// <summary>Retrieves the <see cref="T:Matrix4x4"/> to be used for the <see cref="T:Camera"/>'s view offset.</summary>
        /// <param name="eye"></param>
        public Matrix4x4 GetViewMatrix(ProjectionMatrixEye eye) {
            retrievedViewMatrices();
            return viewMatrices[(int)eye];
        }

        /// <summary>Retrieves the position to be used for the <see cref="T:Camera"/>'s <see cref="T:Transform"/>.</summary>
        /// <param name="eye"></param>
        public Vector3 GetViewPosition(ProjectionMatrixEye eye) {
            retrievedViewsAndTransforms();
            return viewPositions[(int)eye];
        }

        /// <summary>Retrieves the rotation to be used for the <see cref="T:Camera"/>'s <see cref="T:Transform"/>.</summary>
        /// <param name="eye"></param>
        public Quaternion GetViewRotation(ProjectionMatrixEye eye) {
            retrievedViewsAndTransforms();
            return viewRotations[(int)eye];
        }

        /// <summary>
        /// <para>Used to set the <see cref="T:Transform"/> of the <see cref="T:Camera"/>.</para>
        /// <para>
        /// Will be the middle of the pose positions retrieved by
        /// <see cref="M:DSHUnityAbstraction.L7_GetDisplayPoses()"/>
        /// </para>
        /// </summary>
        /// <param name="stereo">
        /// If true will return the position for the
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Left"/> and
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Right"/>.
        /// Otherwise it returns the position for the
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Mono"/>.
        /// </param>
        public Vector3 GetTransformPosition(bool stereo) {
            retrievedViewsAndTransforms();
            return transformPositions[stereo ? 1 : 0];
        }

        /// <summary>
        /// <para>Used to set the <see cref="T:Transform"/> of the <see cref="T:Camera"/>.</para>
        /// <para>
        /// Will be the middle of the pose rotations retrieved by
        /// <see cref="M:DSHUnityAbstraction.L7_GetDisplayPoses()"/>
        /// </para>
        /// </summary>
        /// <param name="stereo">
        /// If true will return the rotation for the
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Left"/> and
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Right"/>.
        /// Otherwise it returns the rotation for the
        /// <see cref="T:Camera.MonoOrStereoscopicEye.Mono"/>.
        /// </param>
        public Quaternion GetTransformRotation(bool stereo) {
            retrievedViewsAndTransforms();
            return transformRotations[stereo ? 1 : 0];
        }

		private void getRate(DEVICE_IDENTIFIER devId, out float rate)
		{
			rate = 0.0f;
			float[] fps = new float[1];
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetRate";
				try {
					switch (devId) {
					case DEVICE_IDENTIFIER.FRONT_CAMERA:
						DSHUnityAbstraction.L7_CameraGetRate(fps);
						rate = fps[0];
						break;
					case DEVICE_IDENTIFIER.DEPTH_SENSOR:
						DSHUnityAbstraction.L7_DepthGetRate(fps);
						rate = fps[0];
						break;
					case DEVICE_IDENTIFIER.LONG_WAVE_IR:
						DSHUnityAbstraction.L7_ThermalGetRate(fps);
						rate = fps[0];
						break;
					case DEVICE_IDENTIFIER.IMU_SENSOR:
						DSHUnityAbstraction.L7_IMUGetRate(fps);
						rate = fps[0];
						break;
					default:
						Debug.Log("Not defined for device: " + devId);
						break;
					}
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}
		}

		private DATA_FORMAT getDataFormat(DEVICE_IDENTIFIER devId)
		{
			DATA_FORMAT format = DATA_FORMAT.UNKNOWN_FORMAT;
			int[] fmt = new int[1];

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetFormat";
				try {
					switch (devId) {
					case DEVICE_IDENTIFIER.FRONT_CAMERA:
						DSHUnityAbstraction.L7_CameraGetDataFormat(fmt, true);
						format = (DATA_FORMAT)fmt[0];
						break;
					case DEVICE_IDENTIFIER.DEPTH_SENSOR:
						DSHUnityAbstraction.L7_DepthGetDataFormat(fmt, true);
						format = (DATA_FORMAT)fmt[0];
						break;
					case DEVICE_IDENTIFIER.LONG_WAVE_IR:
						DSHUnityAbstraction.L7_ThermalGetDataFormat(fmt, true);
						format = (DATA_FORMAT)fmt[0];
						break;
					case DEVICE_IDENTIFIER.IMU_SENSOR:
						DSHUnityAbstraction.L7_IMUGetDataFormat(fmt, true);
						format = (DATA_FORMAT)fmt[0];
						break;
					default:
						Debug.Log("Not defined for device: " + devId);
						break;
					}
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return format;
		}

		#region Device Info

		public string GetDeviceModel () {
			string deviceModel = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetDeviceModel";
				try {
					DSHUnityAbstraction.L7_GetDeviceModel(deviceModel);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return deviceModel;
		}

		public string GetDeviceMake () {
			string deviceMake = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetDeviceMake";
				try {
					DSHUnityAbstraction.L7_GetDeviceMake(deviceMake);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return deviceMake;
		}

		public string GetHardwareVariant() {
			string hardwareVariant = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetHardwareVariant";
				try {
					DSHUnityAbstraction.L7_GetHardwareVariant(hardwareVariant);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return hardwareVariant;
		}

		public string GetDeviceSN() {
			string deviceSN = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetDeviceSN";
				try {
					DSHUnityAbstraction.L7_GetDeviceSerialNumber(deviceSN);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return deviceSN;
		}

		public string GetAPIVersion() {
			string apiVersion = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetAPIVersion";
				try {
					DSHUnityAbstraction.L7_GetAPIVersion(apiVersion);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return apiVersion;
		}

		public string GetRuntimeVersion() {
			string runtimeVersion = string.Empty;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetRuntimeVersion";
				try {
					DSHUnityAbstraction.L7_GetRuntimeVersion(runtimeVersion);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return runtimeVersion;
		}

		public PLATFORM GetPlatform() {
			PLATFORM platform = PLATFORM.UNKNOWN;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetPlatform";
				try {
					int[] pltfrm = new int[1];
					DSHUnityAbstraction.L7_GetPlatform(pltfrm);
					platform = (PLATFORM)pltfrm[0];				
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			return platform;
		}

		public List<DEVICE_IDENTIFIER> GetAvailableDevices() {
			int numDevices = Enum.GetNames (typeof(DEVICE_IDENTIFIER)).Length;
			int[] availableDevices = new int[numDevices];

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.EditorSmartHelmetPreview):
				break;

			case (RunEnvironmentType.EditorSmartGlassesPreview):
				break;

			case (RunEnvironmentType.OnDevice):
				const string MethodName = "GetAvailableDevices";
				try {
					DSHUnityAbstraction.L7_GetAvailableDevices(availableDevices, numDevices);
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;
			}

			List<DEVICE_IDENTIFIER> list = new List<DEVICE_IDENTIFIER> ();

			for (int i = 0; i < numDevices; ++i) {
				if (availableDevices [i] >= 0 && availableDevices [i] <= numDevices) {
					list.Add ((DEVICE_IDENTIFIER)availableDevices [i]);
				}
			}
			return list;
		}
		#endregion
	}
}