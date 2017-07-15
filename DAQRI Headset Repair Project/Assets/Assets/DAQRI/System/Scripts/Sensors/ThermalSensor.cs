using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DAQRI {
	public class ThermalSensor : CameraDevice, IThermalSensor, IThermalCameraNativeCall {
		private int[] tempWidth = new Int32[1];
		private int[] tempHeight = new Int32[1];

		public ThermalSensor(DEVICE_IDENTIFIER value, 
			string _defaultTexturePath):base(value,_defaultTexturePath){

			setRawByteSize (3);
			SetFieldOfView (43.6038f);
		}

		public new Func<bool> startDeviceFunc = null;
		public new Func<IntPtr, int[], int[], bool> getCameraDataFunc = null;

		public bool StartDeviceNativeCall(){
			return (bool)InvokeMethod (startDeviceFunc, new object[] { });
		}

		public bool GetCameraDataNativeCall(IntPtr bytes, int[] width, int[] height){
			//return (bool)InvokeMethod (getCameraDataFunc, new object[] { bytes, width, height });
			string MethodName = GetDeviceType().ToString() + " " + getCameraDataFunc.Method.ToString();
			try{
				return getCameraDataFunc(bytes,width,height);
			}
			catch (DllNotFoundException e) {
				Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
			}
			catch (EntryPointNotFoundException e) {
				Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
			}
			catch (Exception e) {
				Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
			}
			return false;
		}

		override public bool StartDevice(int preset = -1){
			bool result = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					result = StartDeviceNativeCall ();// startDeviceFunc ();
				}
				break;
			}

			onCameraDeviceStart (result);

			return result;
		}

		override protected bool updateCameraData()
		{
			bool result = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
						result = GetCameraDataNativeCall (getCameraData ().Address, tempWidth, tempHeight);// getCameraDataFunc (getCameraData ().Address, tempWidth, tempHeight);
					}
				}
				break;
			}
			return result;
		}

		override public void UpdateDevice(){
			switch(GetDeviceCurrentState())
			{
			case DEVICE_STATE.IDLE:
				break;
			case DEVICE_STATE.RUNNING:
				GL.IssuePluginEvent (GetRenderEventNativeCall(), (int)DSH_UNITY_RENDER_EVENTID.UPDATE_TEXTURE_GL_THERMAL);
				break;
			case DEVICE_STATE.FAILED:
				LoadDefaultTexture ();
				SetDeviceCurrentState (DEVICE_STATE.IDLE);
				break;
			}
		}
	}
}