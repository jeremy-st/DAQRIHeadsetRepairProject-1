using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI{
	public interface IThermalCameraNativeCall : ICameraDeviceNativeCall{
		bool StartDeviceNativeCall();
		bool GetCameraDataNativeCall(IntPtr bytes, int[] width, int[] height);
	}

	public interface IThermalSensor : ICameraDevice{
	}
}