using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace DAQRI {

	abstract public class Device : IDevice
	{
		protected DEVICE_IDENTIFIER deviceType;
		protected IRunEnvironmentInfo runEnvironment = new RunEnvironmentInfo ();
		private DEVICE_STATE currState = DEVICE_STATE.IDLE;

		protected int width = 0;
		protected int height = 0;
		protected float aspectRatio;
		protected float effectiveRate;
		protected float theoreticalRate;
		protected DATA_FORMAT dataFormat;

		public Device(DEVICE_IDENTIFIER value){

			deviceType = value;
			currState = DEVICE_STATE.IDLE;
		}
	
		#region native_calls

		public const string DllNotFoundErrorFormat =
			"{0} failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed.\n{1}";

		public const string EntryPointNotFoundErrorFormat =
			"{0} failed, Signature missing in the library, please update the library.\n{1}";

		public const string GeneralErrorFormat = "{0} failed.\n{1}";

		public Func<int, bool> startDeviceFunc = null;
		public Func<bool> stopDeviceFunc = null;
		public Action<float[]> getEffectiveRateFunc = null;
		public Action<float[]> getTheoreticalRateFunc = null;
		public Action<int[],bool> getDataFormatFunc = null;

		/// <summary>
		/// Starts the Device (native call)
		/// </summary>
		public bool StartDeviceNativeCall(int preset){
			return (bool)InvokeMethod (startDeviceFunc, new object[] { preset });
		}

		/// <summary>
		/// Stops the Device (native call)
		/// </summary>
		public bool StopDeviceNativeCall(){
			return (bool)InvokeMethod (stopDeviceFunc, new object[] {});
		}

		/// <summary>
		/// Gets Effective Rate of the Device (native call)
		/// </summary>
		public void GetEffectiveRateNativeCall(float[] rate){
			string MethodName = " ";
			try{
				getEffectiveRateFunc(rate);
			}
			catch (DllNotFoundException e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getEffectiveRateFunc.Method.ToString ();
				Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
			}
			catch (EntryPointNotFoundException e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getEffectiveRateFunc.Method.ToString ();
				Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
			}
			catch (Exception e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getEffectiveRateFunc.Method.ToString ();
				Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
			}
		}

		/// <summary>
		/// Gets theoretical Rate of the Device (native call)
		/// </summary>
		public void GetTheoreticalRateNativeCall(float[] rate){
			string MethodName = " ";
			try{
				getTheoreticalRateFunc(rate);
			}
			catch (DllNotFoundException e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getTheoreticalRateFunc.Method.ToString ();
				Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
			}
			catch (EntryPointNotFoundException e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getTheoreticalRateFunc.Method.ToString ();
				Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
			}
			catch (Exception e) {
				MethodName = "Device Type : " + deviceType.ToString () + " " + getTheoreticalRateFunc.Method.ToString ();
				Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
			}
		}
		public void GetDataFormatNativeCall(int[] format, bool requested){
			InvokeMethodNoReturn (getDataFormatFunc, new object[] { format, true });
		}

		/// <summary>
		/// Note : Dynamic invoking is expensive, please use dynamic invoking only
		/// when required
		/// </summary>
		public object InvokeMethod(Delegate method, params object[] args)
		{
			string MethodName = "";
			try
			{
				return method.DynamicInvoke(args);
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException is DllNotFoundException) {
					MethodName = "Device Type : " + deviceType.ToString() + " " + method.Method.Name;
					Debug.LogWarningFormat (DllNotFoundErrorFormat, MethodName, e);
				} else if (e.InnerException is EntryPointNotFoundException) {
					MethodName = "Device Type : " + deviceType.ToString() + ": " + method.Method.Name;
					Debug.LogWarningFormat (EntryPointNotFoundErrorFormat, MethodName, e);
				} else {
					MethodName = "Device Type : " + deviceType.ToString() + ": " + method.Method.Name;
				}
			}
			return default(object);
		}

		/// <summary>
		/// Note : Dynamic invoking is expensive, please use dynamic invoking only
		/// when required
		/// </summary>
		public  void InvokeMethodNoReturn(Delegate method, params object[] args)
		{
			string MethodName = " ";
			try
			{
				method.DynamicInvoke(args);
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException is DllNotFoundException) {
					MethodName = "Device Type : " + deviceType.ToString() + " " + method.Method.Name;
					Debug.LogWarningFormat (DllNotFoundErrorFormat, MethodName, e);
				} else if (e.InnerException is EntryPointNotFoundException) {
					MethodName = "Device Type : " + deviceType.ToString() + " " + method.Method.Name;
					Debug.LogWarningFormat (EntryPointNotFoundErrorFormat, MethodName, e);
				} else {
					MethodName = "Device Type : " + deviceType.ToString() + " " + method.Method.Name;
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);	
				}
			}
		}
		#endregion


		public DEVICE_IDENTIFIER GetDeviceType(){
			return deviceType;
		}

		#region Setters

		public void SetDeviceCurrentState(DEVICE_STATE state){
			currState = state;
		}

		#endregion

		#region Getters

		public DEVICE_STATE GetDeviceCurrentState(){
			return currState;
		}

		public Vector2 GetFrameSize(){
			return new Vector2 (width, height);
		}

		public float GetEffectiveRate(){
			getEffectiveRate ();
			return effectiveRate;
		}

		public float GetTheoreticalRate(){
			getTheoreticalRate ();
			return theoreticalRate;
		}

		public DATA_FORMAT GetDataFormat(){
            getDataFormat ();
			return dataFormat;
		}
		#endregion

		abstract public bool StartDevice (int preset);
	
		abstract public bool StopDevice ();

		abstract public void UpdateDevice ();

		#region private
		private void getEffectiveRate(){
			float[] rate = new float[1];
			GetEffectiveRateNativeCall(rate);
			effectiveRate = rate [0];
		}

		private void getTheoreticalRate(){
			float[] rate = new float[1];
			GetTheoreticalRateNativeCall(rate);
			theoreticalRate = rate [0];
		}

		private void getDataFormat(){
			int[] fmt = new int[1];
			GetDataFormatNativeCall (fmt, true);
			dataFormat = (DATA_FORMAT)fmt [0];
		}
		#endregion

		#region static


		#endregion

		/*
		//PerformServiceOperationNoReturn(() => getDataFormatFunc (fmt,true));
		 
		protected T PerformServiceOperation<T>(Func<T> func)
		{
			string MethodName = deviceType.ToString() + " " + func.ToString();
			try
			{
				return func.Invoke();
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
			return default(T);
		}*/

		/*protected void PerformServiceOperationNoReturn(Func func)
		{
			string MethodName = deviceType.ToString() + " " + func.ToString();
			try
			{
				func.Invoke();
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
		}*/
	}
}