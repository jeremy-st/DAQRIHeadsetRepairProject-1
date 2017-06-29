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
using System.Runtime.InteropServices;
using System;

namespace DAQRI {

	public static class DSHUnityPlugin {

		public enum HCAMERA_TYPE
		{
			COLOR_CAMERA = 1 << 0,
			Z_CAMERA = 1 << 1,
			IR_CAMERA = 1 << 2,
			LWIR_CAMERA = 1 << 5,
			
		};

		public enum UNITY_RENDER_EVENTID {
			NOP = 0,
			UPDATE_TEXTURE_GL_COLOR = 1,
			UPDATE_TEXTURE_GL_THERMAL = 2,
		};

		public enum CAMERA_CONTROL {
			WHITE_BALANCE = 0,
			EXPOSURE = 1,
		}

		public enum COLOR_CAMERA_PRESET
		{
			RGB_240p = 10,
			RGB_480p = 11,
			RGB_480p_WIDE = 12,
			RGB_720p = 13,
			RGB_1080p = 14,
		};

		private static int DEFAULT_VALUE = -1;
		//for tracking
		public static bool enableGrayScale = true;

		//DO NOT CHANGE THESE VALUES, this will result in crash at this point
		private static int CAMERA_WIDTH = DEFAULT_VALUE;
		private static int CAMERA_HEIGHT = DEFAULT_VALUE;
		//DO NOT CHANGE THESE VALUES, this will result in crash at this point
		//private static int DEPTH_CAMERA_WIDTH = DEFAULT_VALUE;
		//private static int DEPTH_CAMERA_HEIGHT = DEFAULT_VALUE;
		
		private static int LWIR_CAMERA_WIDTH = DEFAULT_VALUE;
		private static int LWIR_CAMERA_HEIGHT = DEFAULT_VALUE;

		private static HCAMERA_TYPE camera_type;

		private class CameraData
		{
			public readonly byte[] data;
			private GCHandle handle;
			public IntPtr Address { get; private set; }
			internal CameraData(int width, int height, int color_size)
			{
				data = new byte[width * height * color_size];
				handle = GCHandle.Alloc(data, GCHandleType.Pinned);
				Address = handle.AddrOfPinnedObject();
			}

			~CameraData()
			{
				Address = IntPtr.Zero;
				handle.Free();
			}
		}

		private static CameraData colorData = null;
		private static CameraData thermalData = null;

		private static int[] cameraWidth = new int[1];
		private static int[] cameraHeight = new int[1];

		public static bool bCorrectDLLloaded = true;

		public static bool StartCamera()
		{
			int video_camera_preset = (int)COLOR_CAMERA_PRESET.RGB_1080p;
			bool ok = false;
			try {
				ok = DSHUnityAbstraction.L7_CameraStart(video_camera_preset);
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Start Camera failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
				return ok;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Start Camera failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			if (!ok) {
				Debug.LogWarning("Unity abstractions not started correctly, Realsense camera feeds may not work");
			}
			return ok;
		}

		public static void CameraSetControl(CAMERA_CONTROL control, int value) {
			switch (control) {
			default:
				Debug.LogWarning ("CameraSetControl(): Control not supported -- " + control.ToString ());
				break;
			}
		}

		public static void CameraSetAutoControlEnabled(CAMERA_CONTROL control, bool value) {
			switch (control) {
			default:
				Debug.LogWarning ("CameraSetAutoControlEnabled(): Control not supported -- " + control.ToString ());
				break;
			}
		}

		public static bool CameraIsAutoControlEnabled(CAMERA_CONTROL control) {
			switch (control) {
			default:
				Debug.LogWarning ("CameraIsAutoControlEnabled(): Control not supported -- " + control.ToString ());
				return false;
			}
		}

		public static bool Initialize() {
			bool ok = false;
			try
			{
				ok = DSHUnityAbstraction.L7_Initialize ();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Initialize failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
				return ok;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Initialize failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
				return ok;
			}
			if (!ok) {
				Debug.LogWarning("Initalize returned false, most likely because plugin is already initialized");
			}
			return ok;
		}

		public static void StopCamera()
		{
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_CameraStop();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Stop Camera failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
				return;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Stop Camera failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
				return;
			}
			if (!status) {
				Debug.LogWarning("Realsense camera didnt stop properly");
			}
		}

		public static void CameraGetPose(float[] pos, float[] rot)
		{
			try{
				DSHUnityAbstraction.L7_CameraGetPose(pos, rot);
                return;
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Get Camera Pose failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Camera Get Pose failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
            pos [0] = 0.04725968f;
			pos [1] = -0.02074674f;
			pos [2] = -0.004146449f;
			rot [0] = -0.0001961226f;
			rot [1] = 0.003372774f;
			rot [2] = 0.01508114f;
			rot [3] = -0.9998806f;
		}

		public static bool GetColorCamData([In, Out]Color32[] colors32)
		{
			bool ok = false;

			if (CAMERA_WIDTH == DEFAULT_VALUE || CAMERA_HEIGHT == DEFAULT_VALUE) {
				ok = GetColorCamParams (out CAMERA_WIDTH, out CAMERA_HEIGHT);
				if (!ok) {
					Debug.LogWarning ("Could not get the camera parameter");
					return false;
				}
				else
					colorData = new CameraData(CAMERA_WIDTH, CAMERA_HEIGHT,3);
			}

			ok = DSHUnityAbstraction.L7_CameraGetData(colorData.Address);
			if (ok == true) {
				int x = 0;
				for (int i = 0; i < CAMERA_HEIGHT; ++i) {
					for (int j = 0; j < CAMERA_WIDTH; ++j) {
						int index = i * CAMERA_WIDTH + j;
						colors32 [index].r = colorData.data [x++];
						colors32 [index].g = colorData.data [x++];
						colors32 [index].b = colorData.data [x++];	
						colors32 [index].a = 255;
					}
				}
			}

			return ok;
		}

		public static bool GetColorCamParams(out int width, out int height)
		{
			width = DEFAULT_VALUE;
			height = DEFAULT_VALUE;
			return getCameraParameter(HCAMERA_TYPE.COLOR_CAMERA,out width,out height);
		}

		public static bool StartIMU() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_IMUStart ();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("IMU start failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("IMU start failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}

		public static bool StopIMU() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_IMUStop ();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("IMU stop failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("IMU stop failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}

		public static bool StartPositionMonitor() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_PositionMonitorStart ();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Position monitor start failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Position monitor start failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}

		public static bool StopPositionMonitor() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_PositionMonitorStop ();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Position monitor stop failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Position monitor stop failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}
			
		public static bool GetIMUQuaternion(float[] quat)
		{
			bool status = DSHUnityAbstraction.L7_IMUGetRotation (quat);
			if (!status) {
				Debug.Log ("Issues in grabbing Rotation data from IMU");
			}
			return status;
		}

		public static bool GetIMUAcceleration(float[] accel)
		{
			bool status = DSHUnityAbstraction.L7_IMUGetAcceleration (accel);
			if (!status) {
				Debug.Log ("Issues in grabbing Acceleration data from IMU");
			}
			return status;
		}

		public static bool GetIMUGyro(float[] gyro)
		{
			bool status = DSHUnityAbstraction.L7_IMUGetGyro (gyro);
			if (!status) {
				Debug.Log ("Issues in grabbing Gyro data from IMU");
			}
			return status;
		}

		/*public static bool GetIMUData(ref IMUData imuData) {
			return false;
		}*/


		static float[] pos = new float[3];
		static float[] rot = new float[4];
		public static bool GetVIOData(ref VIOData vioData) {
			bool status = DSHUnityAbstraction.L7_PositionMonitorUpdate (0.03f);
			if (status) {
				DSHUnityAbstraction.L7_PositionMonitorGetPosition (pos);
				DSHUnityAbstraction.L7_PositionMonitorGetRotation (rot);
				vioData.SetNativeData (pos, rot);
			} else {
				Debug.Log ("Issues in grabbing data from IMU");
			}
			return status;
		}
			
		public static bool StartThermal() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_ThermalStart();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Thermal start failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Thermal start failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}

		public static bool StopThermal() {
			bool status = false;
			try{
				status = DSHUnityAbstraction.L7_ThermalStop();
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Thermal stop failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Thermal stop failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return status;
		}

		public static bool GetThermalCamParams(out int width, out int height)
		{
			width = DEFAULT_VALUE;
			height = DEFAULT_VALUE;
			return getCameraParameter(HCAMERA_TYPE.LWIR_CAMERA,out width,out height);
		}

		public static bool GetThermalData([In, Out]Color32[] colors32) {
			bool ok = false;
			if (LWIR_CAMERA_WIDTH == DEFAULT_VALUE || LWIR_CAMERA_HEIGHT == DEFAULT_VALUE) {
				ok = GetThermalCamParams (out LWIR_CAMERA_WIDTH, out LWIR_CAMERA_HEIGHT);
				if (!ok) {
					Debug.LogWarning ("Could not get the thermal parameter");
					return false;
				}
				else
					thermalData = new CameraData(LWIR_CAMERA_WIDTH, LWIR_CAMERA_WIDTH,3);
			}

			ok = DSHUnityAbstraction.L7_ThermalGetData(thermalData.Address,cameraWidth,cameraHeight);

			if (ok == true) {
				int x = 0;
				for (int i = 0; i < LWIR_CAMERA_HEIGHT; ++i)
				{
					for (int j = 0; j < LWIR_CAMERA_WIDTH; ++j)
					{
						int index = i* LWIR_CAMERA_WIDTH + j;
						colors32[index].r = thermalData.data[x++];
						colors32[index].g = thermalData.data[x++];
						colors32[index].b = thermalData.data[x++];
						colors32[index].a = 255;
					}
				}
			}
			return ok;
		}

		public static void ThermalGetPose(float[] pos, float[] rot)
		{
			try{
				DSHUnityAbstraction.L7_ThermalGetPose(pos, rot);
                return;
            }
            catch (DllNotFoundException e) {
				Debug.LogWarning ("Get Thermal Pose failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
            }
            catch (EntryPointNotFoundException e){
				Debug.LogWarning ("Thermal Get Pose failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			// use default thermal
			pos [0] = 0.01373458f;
			pos [1] = -0.002731353f;
			pos [2] = -0.003632645f;
			rot [0] = -0.0001961226f;
			rot [1] = 0.003372774f;
			rot [2] = 0.01508114f;
			rot [3] = -0.9998806f;
		}

		public static void GetProjectionMatrix(float[] projMatrix)
		{
			VisionUnityAbstraction.L7_TrackerGetProjectionMatrix (projMatrix);
		}

        public static void GetDisplayProjectionMatrices(float near, float far, float[] matrixLeft, float[] matrixRight)
        {
			try{
				DSHUnityAbstraction.L7_GetDisplayProjectionMatrices(near, far, matrixLeft, matrixRight);
                return;
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Get Display Projection Matrices failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Get Display PRojection Matrices failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
            matrixLeft[0] = 3.189697f;
            matrixLeft[1] = 0;
            matrixLeft[2] = 0;
            matrixLeft[3] = 0;
            matrixLeft[4] = 0;
            matrixLeft[5] = 5.671282f;
            matrixLeft[6] = 0;
            matrixLeft[7] = 0;
            matrixLeft[8] = 0;
            matrixLeft[9] = 0;
            matrixLeft[10] = -1.00004f;
            matrixLeft[11] = -1;
            matrixLeft[12] = 0;
            matrixLeft[13] = 0;
            matrixLeft[14] = -0.04000008f;
            matrixLeft[15] = 0;

            matrixRight[0] = 3.189697f;
            matrixRight[1] = 0;
            matrixRight[2] = 0;
            matrixRight[3] = 0;
            matrixRight[4] = 0;
            matrixRight[5] = 5.671282f;
            matrixRight[6] = 0;
            matrixRight[7] = 0;
            matrixRight[8] = 0;
            matrixRight[9] = 0;
            matrixRight[10] = -1.00004f;
            matrixRight[11] = -1;
            matrixRight[12] = 0;
            matrixRight[13] = 0;
            matrixRight[14] = -0.04000008f;
            matrixRight[15] = 0;
        }

        public static void GetDisplayPoses(float[] posLeft, float[] rotLeft, float[] posRight, float[] rotRight)
        {
			try{
				DSHUnityAbstraction.L7_GetDisplayPoses(posLeft, rotLeft, posRight, rotRight);
                return;
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Get Display Poses failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Get Display Poses failed, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
            posLeft[0] = -0.01876234f;
            posLeft[1] = -0.06813736f;
            posLeft[2] = -0.04304444f;
            rotLeft[0] = 0.005479251f;
            rotLeft[1] = 0.01076406f;
            rotLeft[2] = 0.01019455f;
            rotLeft[3] = -0.9998751f;

            posRight[0] = 0.0447242f;
            posRight[1] = -0.06902217f;
            posRight[2] = -0.04305821f;
            rotRight[0] = 0.00835955f;
            rotRight[1] = 0.002099279f;
            rotRight[2] = 0.01005393f;
            rotRight[3] = -0.9999123f;
        }

        private static bool getCameraParameter(HCAMERA_TYPE cam_type, out int width, out int height)
		{
			width = DEFAULT_VALUE;
			height = DEFAULT_VALUE;
			bool okParams = false;
			bool result = false;

			try{
			switch (cam_type) {
			case HCAMERA_TYPE.COLOR_CAMERA:
			case HCAMERA_TYPE.IR_CAMERA:
			case HCAMERA_TYPE.Z_CAMERA:
				okParams = DSHUnityAbstraction.L7_CameraGetParams (cameraWidth, cameraHeight);
				break;
			case HCAMERA_TYPE.LWIR_CAMERA:
				okParams = DSHUnityAbstraction.L7_ThermalGetParams (cameraWidth, cameraHeight);
				break;
			}

			if (okParams) {
				if (cameraWidth[0] != DEFAULT_VALUE && cameraHeight[0] != DEFAULT_VALUE) {
					width = cameraWidth[0];
					height = cameraHeight[0];
					result = true;
				} else {
					Debug.LogError ("Get Camera parameter for" + ((HCAMERA_TYPE)cam_type) + "returned invalid data");
					result = false;
				}

			} else {
				Debug.LogError ("Get Camera parameter for" + ((HCAMERA_TYPE)cam_type) + "failed");
				result = false;
			}
			}
			catch(DllNotFoundException e) {
				Debug.LogWarning ("Couldn't get camera parameter, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed " + e);
				bCorrectDLLloaded = false;
			}
			catch(EntryPointNotFoundException e){
				Debug.LogWarning ("Couldn't get camera parameter, signature missing in the library, please update the library" + e);
				bCorrectDLLloaded = false;
			}
			return result;
		}
	}
}
