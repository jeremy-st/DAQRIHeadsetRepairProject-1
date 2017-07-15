using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI {

	public interface ICameraDeviceNativeCall : IDeviceNativeCall{
		bool GetParametersNativeCall(int[] width,int[] height);
		void GetPoseNativeCall(float[] position, float[] rotation);
		void SetRenderEventTextureIDNativeCall(IntPtr texture_Handle);
		IntPtr GetRenderEventNativeCall();
		bool GetCameraDataNativeCall(IntPtr bytes);
	}

	public interface ICameraDevice : IDevice{
	
		float GetFieldOfView();
	
		float GetAspectRatio();
	
		Texture2D GetTexture();
	
		byte[] GetBuffer();

		int GetBufferSize();

		Vector3 GetPosePosition ();

		Quaternion GetPoseRotation ();

		void SetDefaultTexturePath (string path);
			
		}
}
