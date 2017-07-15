using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI {
	public class DepthCamera : CameraDevice, IDepthCamera, IDepthCameraNativeCall {
	
		private bool histogramOnOff;

		public DepthCamera(DEVICE_IDENTIFIER value, 
			string _defaultTexturePath) : base(value,_defaultTexturePath){

			setRawByteSize (2);
			SetFieldOfView (43.28f);
			histogramOnOff = false;
		}

		private int histogram_byte_size = 3;
		private int raw_data_byte_size = 2;

		public new Action<IntPtr,bool> setRenderEventTextureIDFunc = null;
		public new Func<IntPtr, bool, bool> getCameraDataFunc = null;
		public Func<int,bool> updatePresetFunc = null;

		public void SetRenderEventTextureIDNativeCall(IntPtr texture_handle, bool value){
			InvokeMethodNoReturn (setRenderEventTextureIDFunc, new object[] { texture_handle, value });
		}
		public bool GetCameraDataNativeCall(IntPtr bytes, bool value){
			return (bool)InvokeMethod (getCameraDataFunc, new object[] { bytes, value });
		}
		public bool UpdatePresetNativeCall(int preset){
			return (bool)InvokeMethod (updatePresetFunc, new object[] { preset });
		}

		#region getters

		/// <summary>
		/// Gets the DepthCamera current render type (Histogram/Raw)
		/// </summary>
		/// <returns> <c>true</c> if histogram rendertype histogram is enabled, and <c>false</c> if raw rendertype is enabled</returns>
		public bool GetHistogramOnOffStatus() {
			return histogramOnOff;
		}

		#endregion

		#region setters

		/// <summary>
		/// Set Histogram on off
		/// </summary>
		public void SetHistogram(bool value) {
			histogramOnOff = value;
			if (histogramOnOff) {
				setRawByteSize (histogram_byte_size);
			} else {
				setRawByteSize (raw_data_byte_size);
			}
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetRenderEventTextureIDNativeCall(GetTexture().GetNativeTexturePtr(),histogramOnOff);
				}
				break;
			}
		}

		/// <summary>
		/// Set depth resolution to RGB_480p
		/// </summary>
		public void SetCameraResolution(DSH_COLOR_CAMERA_PRESET value){
			bool success = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					success = (bool)UpdatePresetNativeCall((int)value);// InvokeMethod (updatePresetFunc, new object[] { (int)value });
				}
				break;
			}
			if (!success) {
				Debug.Log("set camera resolution for " + GetDeviceType().ToString() + " failed");
			}
		}

		#endregion

		override protected bool updateCameraData()
		{
			bool result = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
						result = GetCameraDataNativeCall (getCameraData ().Address,histogramOnOff);// getCameraDataFunc (getCameraData ().Address, histogramOnOff);
					}
				}
				break;
			}
			return result;
		}

		override protected void setRenderEventTextureID()
		{
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					SetRenderEventTextureIDNativeCall (GetTexture ().GetNativeTexturePtr (), histogramOnOff);// setRenderEventTextureIDFunc (GetTexture ().GetNativeTexturePtr (), histogramOnOff);
				}
				break;
			}
		}

		override public bool StartDevice(int preset){
			bool status = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					status = StartDeviceNativeCall (preset);// startDeviceFunc (preset);
				}
				break;
			}

			//if status is true set state to running and init the camera device
			//if status is false set state to failed and assign default texture
			onCameraDeviceStart (status);
			return status;
		}

		override public void UpdateDevice(){
			switch(GetDeviceCurrentState())
			{
			case DEVICE_STATE.IDLE:
				break;
			case DEVICE_STATE.RUNNING:
				GL.IssuePluginEvent (GetRenderEventNativeCall(), (int)DSH_UNITY_RENDER_EVENTID.UPDATE_TEXTURE_GL_DEPTH);
				break;
			case DEVICE_STATE.FAILED:
				LoadDefaultTexture ();
				SetDeviceCurrentState (DEVICE_STATE.IDLE);
				break;
			}
		}
	}
}