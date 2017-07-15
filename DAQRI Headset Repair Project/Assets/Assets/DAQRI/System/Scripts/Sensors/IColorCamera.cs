using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAQRI {

	public interface IColorCameraNativeCall : ICameraDeviceNativeCall{
		void SetAutoExposureNativeCall(bool value);
		void SetAutoWhiteBalanceNativeCall(bool value);
		void SetExposureNativeCall(int value);
		void SetWhiteBalanceNativeCall(int value);
		bool UpdatePresetNativeCall (int preset);
	}

	public interface IColorCamera : ICameraDevice{
	
		bool GetAutoExposureStatus ();
	
		bool GetAutoWhiteBalanceStatus ();
	
		bool GetColorCameraHDOnOffStatus ();
	
		void SetAutoExposure(bool value);
	
		void SetAutoWhiteBalance (bool value);
	
		void SetExposureValue(int value);

		void SetWhiteBalanceValue (int value);

		void SetCameraResolution (DSH_COLOR_CAMERA_PRESET value);
	}
}