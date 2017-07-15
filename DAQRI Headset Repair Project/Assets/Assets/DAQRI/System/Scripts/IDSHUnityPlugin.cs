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
using System;
using System.Collections;

namespace DAQRI {

	public enum DSH_HCAMERA_TYPE {
		COLOR_CAMERA = 1 << 0,
		Z_CAMERA = 1 << 1,
		IR_CAMERA = 1 << 2,
		LWIR_CAMERA = 1 << 5,

	};

	public enum DSH_UNITY_RENDER_EVENTID {
		NOP = 0,
		UPDATE_TEXTURE_GL_COLOR = 1,
		UPDATE_TEXTURE_GL_THERMAL = 2,
		UPDATE_TEXTURE_GL_DEPTH = 3,
	};

	public enum DSH_CAMERA_CONTROL {
		WHITE_BALANCE = 0,
		EXPOSURE = 1,
	}

	public enum DSH_COLOR_CAMERA_PRESET {
		RGB_240p = 10,
		RGB_480p = 11,
		RGB_480p_WIDE = 12,
		RGB_720p = 13,
		RGB_1080p = 14,
		UNKNOWN = -1,
	};

	public enum DSH_DEPTH_RENDER_TYPE {
		RAW = 0,
		HISTOGRAM = 1
	}

	public enum PLATFORM {
		UNKNOWN = -1,
		HELMET = 0,
		HEADSET = 1,
		HUD = 2,
		OTHER = 3,
	};

	public enum DEVICE_IDENTIFIER {
		UNKNOWN_DEVICE = -1,
		FRONT_CAMERA = 0,
		VIO_IMU_POSE_SENSOR,
		VIO_TRACKER_SENSOR,
		VIO_CAMERA,
		DEPTH_SENSOR,
		IR_CAMERA,
		IMU_SENSOR,
		LONG_WAVE_IR,
	};

	public enum DATA_FORMAT {
		UNKNOWN_FORMAT = -1,                /*!< Bad image format */
		IMU_DATA = 0x494D5520, //'IMU '               
		VIO_IMU_DATA = 0x56495053, //'VIPS' 
		VIO_TRACKER_DATA = 0x5654524B, //'VTRK'
		GRAYSCALE = 0x4C554D38, //'LUM8'
		RGB = 0x52474238, //'RGB8'
		RGBA = 0x52474241, //'RGBA'
		YUYV = 0x59555956, //'YUYV'
		DEPTH = 0x5A313620, //'Z16 '
		LUM16 = 0x4C554D57, //'LUMW'
	}

	public interface IDSHUnityPlugin {

		#region Setup

		bool Initialize ();

		bool Dispose ();

		#endregion

		#region Motion

		bool StartIMU ();

		bool StopIMU ();

		void GetIMURate (out float rate);

		DATA_FORMAT GetIMUDataFormat ();

		bool StartPositionMonitor ();

		bool StopPositionMonitor ();

		bool GetIMUQuaternion (float[] quat);

		bool GetIMUMagneticField (float[] magneticfield);

		bool GetIMUAcceleration (float[] accel);

		bool GetIMUGyro (float[] gyro);

		bool GetVIOData (ref VIOData vioData);

        #endregion


        #region Display

        Matrix4x4 GetProjectionMatrix(ProjectionMatrixEye eye);

	    Matrix4x4 GetViewMatrix(ProjectionMatrixEye eye);

	    Vector3 GetViewPosition(ProjectionMatrixEye eye);

	    Quaternion GetViewRotation(ProjectionMatrixEye eye);

	    Vector3 GetTransformPosition(bool stereo);

	    Quaternion GetTransformRotation(bool stereo);

		#endregion
	}
}
