using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI {

	public interface IDepthCameraNativeCall : ICameraDeviceNativeCall{
		void SetRenderEventTextureIDNativeCall(IntPtr texture_handle, bool value);
		bool GetCameraDataNativeCall(IntPtr bytes, bool value);
		bool UpdatePresetNativeCall (int preset);
	}

	public interface IDepthCamera : ICameraDevice{

		bool GetHistogramOnOffStatus ();

		void SetHistogram(bool value);
	}
}