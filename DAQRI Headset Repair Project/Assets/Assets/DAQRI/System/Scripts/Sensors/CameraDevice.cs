using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace DAQRI {

	abstract public class CameraDevice : Device, ICameraDevice, ICameraDeviceNativeCall {
		public class CameraData{

			public byte[] buffer;
			public IntPtr Address { get; private set; }

			private GCHandle handle;
			internal CameraData(int arraysize)
			{
				buffer = new byte[arraysize];
				handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				Address = handle.AddrOfPinnedObject();
			}

			public void Resize(int size){
				Reset();
				Array.Resize<byte> (ref buffer, size);
				handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				Address = handle.AddrOfPinnedObject();
			}

			private void Reset(){
				Address = IntPtr.Zero;
				handle.Free();
			}

			~CameraData()
			{
				Reset();
			}
		}

		private int raw_byte_size;
		private int DEFAULT_VALUE = -1;
		private int[] cameraWidth = new int[1];
		private int[] cameraHeight = new int[1];
		private string defaultTexturePath = "";

		private CameraData cameraData;
		private Texture2D texture;
		private float fov;
		private Vector3 posePosition;
		private Quaternion poseRotation;

		public CameraDevice (DEVICE_IDENTIFIER value,
			string _defaultTexturePath) : base(value){

			defaultTexturePath = _defaultTexturePath;
			if (texture == null) {
				texture = new Texture2D (1, 1, TextureFormat.RGBA32, false);
			}
			if (defaultTexturePath != "") {
				LoadDefaultTexture ();
			}
		}


		#region native_calls

		public Func<int[],int[], bool> getParametersFunc = null;
		public Action<float[], float[]> getPoseFunc = null;
		public Action<IntPtr> setRenderEventTextureIDFunc = null;
		public Func<IntPtr> getRenderEventFunc = null;
		public Func<IntPtr, bool> getCameraDataFunc = null;


		/// <summary>
		/// Gets Parameter of the Device (width and height) (native call)
		/// </summary>
		public bool GetParametersNativeCall(int[] width,int[] height){
			return (bool)InvokeMethod (getParametersFunc, new object[] { width, height });
		}

		/// <summary>
		/// Gets pose position and rotation of the Device from VIO IMU(native call)
		/// </summary>
		public void GetPoseNativeCall(float[] position, float[] rotation){
			InvokeMethodNoReturn (getPoseFunc, new object[] {position, rotation });
		}

		/// <summary>
		/// Gets Texture ID from unity to native (native call)
		/// </summary>
		public void SetRenderEventTextureIDNativeCall(IntPtr texture_Handle){
			InvokeMethodNoReturn (setRenderEventTextureIDFunc, new object[] { texture_Handle });
		}

		/// <summary>
		/// Gets RenderEvent for texture (native call)
		/// </summary>
		public IntPtr GetRenderEventNativeCall(){
			string MethodName = "";
			try{
				return getRenderEventFunc();
			}
			catch (DllNotFoundException e) {
				MethodName = GetDeviceType() + " " + getRenderEventFunc.Method.ToString();
				Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
			}
			catch (EntryPointNotFoundException e) {
				MethodName = GetDeviceType() + " " + getRenderEventFunc.Method.ToString();
				Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
			}
			catch (Exception e) {
				MethodName = GetDeviceType() + " " + getRenderEventFunc.Method.ToString();
				Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
			}
			return default(IntPtr);
		}

		/// <summary>
		/// Gets Camera Data (native call)
		/// </summary>
		public bool GetCameraDataNativeCall(IntPtr bytes){
			string MethodName = " ";
			try{
				return getCameraDataFunc(bytes);
			}
			catch (DllNotFoundException e) {
				MethodName = GetDeviceType().ToString() + " " + getCameraDataFunc.Method.ToString();
				Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
			}
			catch (EntryPointNotFoundException e) {
				MethodName = GetDeviceType().ToString() + " " + getCameraDataFunc.Method.ToString();
				Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
			}
			catch (Exception e) {
				MethodName = GetDeviceType().ToString() + " " + getCameraDataFunc.Method.ToString();
				Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
			}
			return false;
		}
		#endregion


		abstract protected bool updateCameraData ();

		override public bool StopDevice(){
			bool result = false;
			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				{
					result = StopDeviceNativeCall ();//(bool)InvokeMethod (stopDeviceFunc, new object[] { });
				}
				break;
			}
			return result;
		}

		#region Setters
		/// <summary>
		/// Set Default Texture Path.
		/// </summary>
		public void SetDefaultTexturePath(string path){
			defaultTexturePath = path;
		}

		/// <summary>
		/// Set Field of View of the camera device.
		/// </summary>
		public void SetFieldOfView(float value){
			fov = value;
		}

		/// <summary>
		/// Set Aspect Ratio of the camera device.
		/// </summary>
		public void SetAspectRatio(float value){
			aspectRatio = value;
		}

		/// <summary>
		/// Set Texture of the camera device.
		/// </summary>
		public void SetTexture(Texture2D tex){
			texture = tex;
		}

		/// <summary>
		/// Set Raw Bytes Size of the data, Example : RGB = raw byte size is 3, Depth Camera Raw render format has raw byte size of 2
		/// </summary>
		protected void setRawByteSize(int size)
		{
			raw_byte_size = size;
			updateCameraRawData ();
		}
		#endregion


		#region Getters
		/// <summary>
		/// Gets Field of View of the camera device.
		/// </summary>
		/// <returns>The Field of view of the camera device.</returns>
		public float GetFieldOfView(){
			return fov;
		}

		/// <summary>
		/// Gets Aspect ratio of the camera device.
		/// </summary>
		/// <returns>The Aspect ratio of the camera device.</returns>
		public float GetAspectRatio(){
			return aspectRatio;
		}

		/// <summary>
		/// Gets Texture2D used for rendering of the camera feed.
		/// </summary>
		/// <returns>The Texture2D used for rendering of the camera feed.</returns>
		public Texture2D GetTexture(){
			return texture;
		}

		/// <summary>
		/// Gets Buffer of the camera device data in raw format
		/// size of the Buffer is calulated by width * height * raw_byte_size;
		/// </summary>
		/// <returns>The Buffer of the camera device data in raw format.</returns>
		public byte[] GetBuffer(){
			updateCameraData ();
			return cameraData.buffer;
		}

		/// <summary>
		/// Gets Buffer Size of the camera device data 
		/// size of the Buffer is calulated by width * height * raw_byte_size;
		/// </summary>
		/// <returns>The Buffer size of the camera device.</returns>
		public int GetBufferSize(){
			return cameraData.buffer.Length;
		}

		/// <summary>
		/// Gets Pose position of the camera device in relation to the VIO IMU.
		/// </summary>
		/// <returns>The Pose Position of the camera device in relation to the VIO IMU.</returns>
		public Vector3 GetPosePosition (){
			return posePosition;
		}

		/// <summary>
		/// Gets Pose Rotation of the camera device in relation to the VIO IMU.
		/// </summary>
		/// <returns>The Pose Rotation of the camera device in relation to the VIO IMU.</returns>
		public Quaternion GetPoseRotation (){
			return poseRotation;
		}

		public void LoadDefaultTexture(){
			Texture2D tex = Resources.Load (defaultTexturePath, typeof(Texture2D)) as Texture2D;
			texture.Resize (tex.width, tex.height,TextureFormat.RGBA32,false);
			texture.SetPixels32(tex.GetPixels32());
			texture.Apply ();

			//update dimensions
			width = tex.width;
			height = tex.height;

			//calculate aspect ratio
			calculateAspectRatio ();
		}
		#endregion

		#region protected

		protected CameraData getCameraData(){
			return cameraData;
		}

		protected void onCameraDeviceStart(bool success)
		{
			if (success) {
				SetDeviceCurrentState(DEVICE_STATE.RUNNING);
				init ();
				getPose ();
			} else {
				SetDeviceCurrentState (DEVICE_STATE.FAILED);
			}
		}

		protected void init(){
			bool status = updateCameraParameter ();
			if (status) {
				createTexture ();
				setRenderEventTextureID ();
			}
		}

		virtual protected void setRenderEventTextureID()
		{
			if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
				SetRenderEventTextureIDNativeCall (texture.GetNativeTexturePtr ());
			}
		}

		#endregion

		#region private

		private void getPose(){
			if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
				float[] _posePosition = new float[3];
				float[] _poseRotation = new float[4];
				GetPoseNativeCall (_posePosition, _poseRotation);
				posePosition = new Vector3 ((float)_posePosition [0], (float)_posePosition [1], (float)_posePosition [2]);
				poseRotation = new Quaternion ((float)_poseRotation [0], (float)_poseRotation [1], (float)_poseRotation [2], (float)_poseRotation[3]);
			}
		}

		private void createTexture()
		{
			if (texture == null) {
				texture = new Texture2D (width, height, TextureFormat.RGBA32, false);
			}
			else{
				resizeTexture ();
			}
			texture.hideFlags = HideFlags.None;
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.anisoLevel = 0;
		}

		private void resizeTexture()
		{
			if (texture.width != width || texture.height != height) {
				texture.Resize (width, height, TextureFormat.RGBA32, false);
				texture.Apply ();
			}
		}

		private void calculateAspectRatio(){
            aspectRatio = (float)width / height;
		}

		private void updateCameraRawData()
		{
			if (cameraData != null) {
				cameraData.Resize (width * height * raw_byte_size);
			} else {
				cameraData = new CameraData (width * height * raw_byte_size);
			}
		}

		private bool updateCameraParameter()
		{
			bool result = false;
			int localWidth = 0, localHeight = 0;

			switch (runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				if (GetDeviceCurrentState () == DEVICE_STATE.RUNNING) {
					bool okParams = false;

					okParams = GetParametersNativeCall (cameraWidth, cameraHeight);

					if (okParams) {
						if (cameraWidth [0] != DEFAULT_VALUE && cameraHeight [0] != DEFAULT_VALUE) {
							localWidth = cameraWidth [0];
							localHeight = cameraHeight [0];
							result = true;
						} else {
							Debug.LogError ("Get Camera parameter for " + (deviceType) + "returned invalid data");
							result = false;
						}

					} else {
						Debug.LogError ("Get Camera parameter for " + (deviceType) + " failed.");
						SetDeviceCurrentState (DEVICE_STATE.FAILED);
						return false;
					}
				}
				break;

			default:
				localWidth = texture.width;
				localHeight = texture.height;
				break;
			}

			if (localWidth != width || localHeight != height) {
				width = localWidth;
				height = localHeight;

				updateCameraRawData ();

				calculateAspectRatio ();
			}

			return result;
		}
		#endregion
	}
}