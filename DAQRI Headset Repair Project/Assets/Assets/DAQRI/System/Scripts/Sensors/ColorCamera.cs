using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI {

	public class ColorCamera : CameraDevice,IColorCamera, IColorCameraNativeCall {

		private bool autoExposureOnOff;
		private bool autoWhiteBalanceOnOff;
		private int exposureValue;
		private int whiteBalanceValue;

		private bool hdOnOff;

		public ColorCamera (DEVICE_IDENTIFIER value, 
			string _defaultTexturePath) : base(value,_defaultTexturePath){

			setRawByteSize (3);
			SetFieldOfView (43.28f);

			autoExposureOnOff = true;

			autoWhiteBalanceOnOff = true;

			hdOnOff = false;
		}

		public Action<bool> setAutoExposureFunc = null;
		public Action<bool> setAutoWhiteBalanceFunc = null;
		public Action<int> setExposureFunc = null;
		public Action<int> setWhiteBalanceFunc = null;
		public Func<int, bool> updatePresetFunc = null;

		/// <summary>
		/// Sets Auto Exposure of the Color camera (native call)
		/// </summary>
		public void SetAutoExposureNativeCall(bool value){
			InvokeMethodNoReturn (setAutoExposureFunc, new object[] { value });
		}

		/// <summary>
		/// Set White Balance Rate of the Color camera (native call)
		/// </summary>
		public void SetAutoWhiteBalanceNativeCall(bool value){
			InvokeMethodNoReturn (setAutoWhiteBalanceFunc, new object[] { value });
		}

		/// <summary>
		/// Sets Auto Exposure of the Color camera (native call)
		/// </summary>
		public void SetExposureNativeCall(int value){
			InvokeMethodNoReturn (setExposureFunc, new object[] { value });
		}

		/// <summary>
		/// Set White Balance Rate of the Color camera (native call)
		/// </summary>
		public void SetWhiteBalanceNativeCall(int value){
			InvokeMethodNoReturn (setWhiteBalanceFunc, new object[] { value });
		}

		/// <summary>
		/// Update Preset of the Device (native call)
		/// </summary>
		public bool UpdatePresetNativeCall(int preset){
			return (bool)InvokeMethod (updatePresetFunc, new object[] { preset });
		}

		override public bool StartDevice(int preset){
			bool status = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					status = (bool)StartDeviceNativeCall(preset);// InvokeMethod (startDeviceFunc, new object[] { preset });
				}
				break;
			}

			//if status is true set state to running and init the camera device
			//if status is false set state to failed and assign default texture
			onCameraDeviceStart (status);

			if (status) {
				if (preset == (int)DSH_COLOR_CAMERA_PRESET.RGB_1080p) {
					hdOnOff = true;
				} else {
					hdOnOff = false;
				}
			}

			return status;
		}

		override protected bool updateCameraData()
		{
			bool status = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
						status = (bool)GetCameraDataNativeCall(getCameraData().Address);// InvokeMethod (getCameraDataFunc, new object[] { getCameraData ().Address });
					}
				}
				break;
			}
			return status;
		}

		override public void UpdateDevice(){
			switch(GetDeviceCurrentState())
			{
			case DEVICE_STATE.IDLE:
				break;
			case DEVICE_STATE.RUNNING:
				GL.IssuePluginEvent (GetRenderEventNativeCall()/* getRenderEventFunc ()*/, (int)DSH_UNITY_RENDER_EVENTID.UPDATE_TEXTURE_GL_COLOR);
				break;
			case DEVICE_STATE.FAILED:
				LoadDefaultTexture ();
				SetDeviceCurrentState (DEVICE_STATE.IDLE);
				break;
			}
		}

		#region Setters
		/// <summary>
		/// Gets Auto Exposure on off status.
		/// </summary>
		/// <returns>The Auto Exposure On Off status.</returns>
		public bool GetAutoExposureStatus(){
			return autoExposureOnOff;
		}

		/// <summary>
		/// Gets Auto White Balance on off status.
		/// </summary>
		/// <returns>The Auto White Balance On Off status.</returns>
		public bool GetAutoWhiteBalanceStatus(){
			return autoWhiteBalanceOnOff;
		}

		/// <summary>
		/// Gets Auto Exposure on off status.
		/// </summary>
		/// <returns>The Auto Exposure On Off status.</returns>
		public int GetExposureValue(){
			return exposureValue;
		}

		/// <summary>
		/// Gets Auto White Balance on off status.
		/// </summary>
		/// <returns>The Auto White Balance On Off status.</returns>
		public int GetWhiteBalanceValue(){
			return whiteBalanceValue;
		}

		/// <summary>
		/// Gets color camra HD on off status.
		/// </summary>
		/// <returns>The ColorCamera HD On Off status.</returns>
		public bool GetColorCameraHDOnOffStatus(){
			return hdOnOff;
		}

		#endregion

		#region Setters
		/// <summary>
		/// Set Auto Exposure on off.
		/// </summary>
		public void SetAutoExposure(bool value){
			autoExposureOnOff = value;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetAutoExposureNativeCall (value);
				}
				break;
			}
		}

		/// <summary>
		/// Set Auto White Balance on off.
		/// </summary>
		public void SetAutoWhiteBalance(bool value){
			autoWhiteBalanceOnOff = value;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetAutoWhiteBalanceNativeCall (value);
				}
				break;
			}
		}

		/// <summary>
		/// Set Auto Exposure on off.
		/// </summary>
		public void SetExposureValue(int value){
			exposureValue = value;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetExposureNativeCall (value);
				}
				break;
			}
		}

		/// <summary>
		/// Set Auto White Balance on off.
		/// </summary>
		public void SetWhiteBalanceValue(int value){
			whiteBalanceValue = value;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetWhiteBalanceNativeCall (value);
				}
				break;
			}
		}

		/// <summary>
		/// Set camera resolution to RGB_1080p, RGB_480p
		/// </summary>
		public void SetCameraResolution(DSH_COLOR_CAMERA_PRESET value){
			bool result = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					result = (bool)UpdatePresetNativeCall((int)value);// InvokeMethod (updatePresetFunc, new object[] { (int)value });
				}
				break;
			}
			if (result == true) {
				if (value == DSH_COLOR_CAMERA_PRESET.RGB_1080p) {
					hdOnOff = true;
				} else {
					hdOnOff = false;
				}
			}
		}
		#endregion

	}
}