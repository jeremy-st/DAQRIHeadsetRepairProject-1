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
 *     File Purpose:        Singleton class for managing the device. Gives access to sensors and tracking.                              *
 *                                                                                                                                      *
 *     Guide:               Call ServiceManager.Instance to access the singleton object.                                                   *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Linq;

namespace DAQRI {
	
	public class ServiceManager : MonoBehaviour {

		/// <summary>
		/// Occurs when target identifiers are available.
		/// </summary>
		public event System.Action TargetIdsAvailable;

		/// <summary>
		/// Occurs when vision parameters are available.
		/// </summary>
		public event System.Action VisionParametersAvailable;

	    private bool _initialized = false;

		private Color32[] _videoColor32Array = null;
	    private Color32[] _thermalColor32Array = null;
	    private Texture2D _videoTexture = null;
		private Texture2D _thermalTexture = null;

		private bool _enableVideoTextureUpdates = false;
	    private bool _enableThermalUpdates = false;
	    private bool _enableTracking = false;
		private bool _enableIMU = false;
		private bool _enablePose = false;

		/// <summary>
		/// performance mode uses VIN Imu
		/// </summary>
		private bool bVIOMode = true;

		// this is hardcoded temporarily
		//private string _cameraParameter = "camera_para_realsense_tusi_640x480-updated";
	    private float _nearPlane = 0.02f;
	    private float _farPlane = 1000.0f;

	    private bool _hasVisionParameters = false;
		private Matrix4x4 _visionProjectionMatrix = Matrix4x4.identity;
		private float _visionFieldOfView = 43.28f;
		private float _visionAspectRatio = 1.333f;

		// this is hardcoded temporarily
		private float _thermalFieldOfView = 42.32f;
		private float _thermalAspectRatio = 1.333f;

		private bool _hasIMUData = false;
		private bool _hasPoseData = false;
		private Vector3 _imuGyro = Vector3.zero;
		private Quaternion _imuQuaternion = Quaternion.identity;
		private Vector3 _imuWorldPosition = Vector3.zero;
		private Vector3 _imuWorldVelocity = Vector3.zero;

		private Vector3 prevPos;
		private Vector4 prevOrientation;
		private Vector3 finalGyro = Vector3.zero;

		private bool _useNativeGLTexture = true;
        private readonly int[] _markerIds = new int[1];

		private class Target{
	        public int id;
			public TargetInfo info;
			public float width;
			public float height;
	        public bool isVisible;
			public bool isTrackable;
			public Vector3 position;
			public Quaternion orientation;

	    }
		private struct TargetInfo : IEquatable<TargetInfo> {
			public string path;
			public GameObject trackableObject;
			public bool Equals(TargetInfo _path)
			{
				Debug.Log ("SDAFSDF");
				if (this.path.Equals(_path.path)){
					return true;
				}
				else {
					return false;
				}
			}
		}
		private List<TargetInfo> _targetInfos = new List<TargetInfo> ();
	    private Dictionary<int, Target> _targets = new Dictionary<int, Target> ();

		private HashSet<System.Object> videoBackgroundUsers = new HashSet<System.Object> ();
		private HashSet<System.Object> thermalBackgroundUsers = new HashSet<System.Object> ();
		private HashSet<System.Object> imuUsers = new HashSet<System.Object> ();
		private HashSet<System.Object> vioUsers = new HashSet<System.Object> ();

		//private IMUData imudata = new IMUData ();
		private VIOData viodata = new VIOData ();
		private bool bLandmarkTracking = false;
		private bool bVIOEmulation;

		#region Singleton

	    private static ServiceManager instance;

	    public static ServiceManager Instance {
	        get {
	            if (instance == null) {
	                instance = GameObject.FindObjectOfType<ServiceManager> ();
	                if (instance == null) {
						GameObject display = GameObject.Find ("Display");
						if (display != null) {
							instance = display.AddComponent<ServiceManager> ();
						} else {
							GameObject go = new GameObject ("ServiceManager");
							instance = go.AddComponent<ServiceManager> ();
						}
	                }
	            }
	            return instance;
	        }
	    }

		public static bool InstanceExists
		{
			get { return instance != null; }
		}

		#endregion


		#region MonoBehavior Events

		void Awake () {
			// Initialize the camera early so the textures are ready to be accessed
			// by other classes on Start
			DSHUnityPlugin.Initialize ();

			//Get the default parameters for color camera
			//InitCamera ();
			//Get the default parameters for thermal camera
			//InitThermal ();

			_visionProjectionMatrix = Matrix4x4.Perspective (_visionFieldOfView, _visionAspectRatio, _nearPlane, _farPlane);
		}

		void Update() {
			if (!DSHUnityPlugin.bCorrectDLLloaded || VIOEmulation) {
				return;
			}

			if (!_initialized)
			{
				LateInitialization();
			}

			if (_enableVideoTextureUpdates)
			{
				UpdateVideoTexture();
			}

			if (_enableThermalUpdates)
			{
				UpdateThermalTexture();
			}

			if (_enableIMU && !_enablePose)
			{
				UpdateIMU();
			}
		}

		void LateUpdate () {
			if (!DSHUnityPlugin.bCorrectDLLloaded || VIOEmulation) {
				DisplayManager.Instance.transform.position = ServiceManager.Instance.GetPosition();
				DisplayManager.Instance.transform.rotation = ServiceManager.Instance.GetOrientation ();
				return;
			}

			if (!_initialized)
			{
				LateInitialization();
			}

			if (_enablePose) 
			{
				UpdatePose ();
			}

			if (_enableTracking)
			{
				UpdateTracking();
			}

			//if (ServiceManager.InstanceTarget target = new Target ();
			//VIOMode)
			{
				finalGyro += ServiceManager.Instance.GetIMUGyro() * Time.deltaTime;

				DisplayManager.Instance.transform.position = ServiceManager.Instance.GetPosition();
				DisplayManager.Instance.transform.rotation = ServiceManager.Instance.GetOrientation ();

			}
		}

	    public void LateInitialization () {
			// Initialize the tracker late to give other classes a chance to register
			// target paths
			if (_targetInfos.Count > 0 && !VIOEmulation) {
	            bool status = StartTracking ();
				_initialized = status;
	        }
	    }

	    void OnApplicationQuit () {
			StopCamera ();
			StopThermal ();
			StopIMU ();
			StopPositionMonitor ();

			/*//Debug.Log ("OnApplicationQuit");
			if (_enableVideoTextureUpdates)
			{
				if (videoBackgroundUsers.Count != 0) {
					StopCamera ();
				}
			}

			if (_enableThermalUpdates)
			{
				if (thermalBackgroundUsers.Count != 0) {
					StopThermal ();
				}
			}

			if (_enableIMU)
			{
				if (imuUsers.Count != 0) {
					StopIMU ();
				}
			}

			if (_enablePose){
				if (vioUsers.Count != 0) {
					StopIMU ();
					StopPositionMonitor ();
				}
			}*/
	        if (_enableTracking) {
	            VisionUnityPlugin.StopAR ();
	        }
	    }

		#endregion


		#region Service Registration

		/// <summary>
		/// Registers a video texture user and starts the service if required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public void RegisterVideoTextureUser (System.Object user) {
			if (user == null || videoBackgroundUsers.Contains (user)) {
				//Debug.Log ("RegisterVideoTextureUser: requires instance of an object");
				return;
			}
			if (videoBackgroundUsers.Count == 0) {
				//Debug.Log ("ServiceManager: Enabling video texture updates");
				StartCamera ();
			}
			videoBackgroundUsers.Add (user);
		}

		/// <summary>
		/// Unregisters the video texture user and stops the service if no longer required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public void UnregisterVideoTextureUser (System.Object user) {
			if (user == null || !videoBackgroundUsers.Contains (user)) {
				//Debug.Log ("UnregisterVideoTextureUser: requires instance of an object");
				return;
			}
			videoBackgroundUsers.Remove (user);
			if (videoBackgroundUsers.Count == 0) {
				//Debug.Log ("ServiceManager: Disabling video texture updates");
				StopCamera (); //stop the camera from here breaks the camera feed
			}
		}

		/// <summary>
		/// Registers a thermal texture user and starts the service if required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public void RegisterThermalTextureUser (System.Object user) {
			if (user == null || thermalBackgroundUsers.Contains (user)) {
				//Debug.Log ("RegisterThermalTextureUser: requires instance of an object");
				return;
			}
			if (thermalBackgroundUsers.Count == 0) {
				//Debug.Log ("ServiceManager: Enabling thermal texture updates");
				StartThermal ();
			}
			thermalBackgroundUsers.Add (user);
		}

		/// <summary>
		/// Unregisters the thermal texture user and stops the service if no longer required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public void UnregisterThermalTextureUser (System.Object user) {
			if (user == null || !thermalBackgroundUsers.Contains (user)) {
				//Debug.Log ("UnregisterThermalTextureUser: requires instance of an object");
				return;
			}
			thermalBackgroundUsers.Remove (user);
			if (thermalBackgroundUsers.Count == 0) {
				//Debug.Log ("ServiceManager: Disabling thermal texture updates");
				StopThermal ();
			}
		}

		/// <summary>
		/// Registers an IMU user and starts the service if required.
		/// </summary>
		/// <returns>True if the IMU is available.</returns>
		/// <param name="user">The object using this service (e.g. "this")</param>
		private bool RegisterIMUUser (System.Object user) {
			if (user == null || imuUsers.Contains (user)) {
				//Debug.Log ("RegisterIMUUser: requires instance of an object");
				return false;
			}
			if (imuUsers.Count == 0) {
				StartIMU ();// (vioIMU == true) ? (int)DSHUnityPlugin.DeviceOptimization.PERFORMANCE : (int)DSHUnityPlugin.DeviceOptimization.LOW_POWER); //default it starts in low power mode
			}
			imuUsers.Add (user);
			return _enableIMU;
		}

		/// <summary>
		/// Unregisters the IMU user and stops the service if no longer required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		private void UnregisterIMUUser (System.Object user) {
			if (user == null || !imuUsers.Contains (user)) {
				//Debug.Log ("UnregisterIMUUser: requires instance of an object");
				return;
			}
			imuUsers.Remove (user);
			if (imuUsers.Count == 0) {
				StopIMU ();
			}
		}

		// <summary>
		/// Registers an IMU user and starts the service if required.
		/// </summary>
		/// <returns>True if the IMU is available.</returns>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public bool RegisterVIOUser (System.Object user) {
			if (user == null || vioUsers.Contains (user)) {
				//Debug.Log ("RegisterIMUUser: requires instance of an object");
				return false;
			}
			if (vioUsers.Count == 0) {
				StartIMU ();
				StartPositionMonitor();
			}
			vioUsers.Add (user);
			return _enablePose;
		}

		/// <summary>
		/// Unregisters the IMU user and stops the service if no longer required.
		/// </summary>
		/// <param name="user">The object using this service (e.g. "this")</param>
		public void UnregisterVIOUser (System.Object user) {
			if (user == null || !vioUsers.Contains (user)) {
				//Debug.Log ("UnregisterIMUUser: requires instance of an object");
				return;
			}
			vioUsers.Remove (user);
			if (vioUsers.Count == 0) {
				StopIMU ();
				StopPositionMonitor();
			}
		}

		#endregion


		#region Getters

	    public float GetNearClipPlane () {
	        return _nearPlane;
	    }

	    public float GetFarClipPlane () {
	        return _farPlane;
	    }

	    public bool HasVisionParameters () {
	        return _hasVisionParameters;
	    }

	    public Matrix4x4 GetVisionProjectionMatrix () {
	        return _visionProjectionMatrix;
	    }

	    public float GetVisionFieldOfView () {
	        return _visionFieldOfView;
	    }

	    public float GetVisionAspectRatio () {
	        return _visionAspectRatio;
	    }

		public float GetThermalFieldOfView () {
			return _thermalFieldOfView;
		}

		public float GetThermalAspectRatio () {
			return _thermalAspectRatio;
		}

	    public Texture2D GetColorCameraTexture () {
	        return _videoTexture;
	    }

	    public Texture2D GetThermalCameraTexture () {
			return _thermalTexture;
	    }
			
		#endregion


		#region Target Tracking

		public void RegisterTarget (string targetPath,GameObject trackedObject) {
			bool duplicate = false;
			for (int i = 0; i < _targetInfos.Count; i++) {
				if (_targetInfos [i].path.Equals (targetPath)) {
					duplicate = true;
				}
			}
			if (!duplicate) {
				if (System.IO.File.Exists (targetPath)) {
					TargetInfo _targetinfo;
					_targetinfo.path = targetPath;
					_targetinfo.trackableObject = trackedObject;
					_targetInfos.Add (_targetinfo);
				}
				else {
					Debug.LogWarning ("Could not load the trackable image from " + targetPath + ". File could have moved or corrupted. Please reload the image in TO_" + targetPath.Split ('.') [0] + " trackable object");
				}
			} else {
				Debug.LogWarning (targetPath + " is already registered");
			}
	    }

		public void RegisterTargetSet (List<TrackedObject> _trackedObject)
		{
			StopTracking ();
			RemoveAllTrackable ();
			foreach (TrackedObject trackedObject in _trackedObject) {
				TargetInfo _targetinfo;
				_targetinfo.path = trackedObject.targetPath;
				_targetinfo.trackableObject = trackedObject.gameObject;
			}
			StartTracking ();
		}

		public void UnRegisterTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			if (targetId != -1) {
				_targets.Remove (targetId);
			} else {
				//Debug.LogWarning ("could not unregister " + targetPath);
			}
		}

	    public int GetTargetId (string targetPath) {
	        foreach (Target target in _targets.Values) {
				if (target.info.path == targetPath) {
	                return target.id;
	            }
	        }
	        return -1;
	    }

		public void ActivateTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			ActivateTarget (targetId);
		}

		public void ActivateTarget(int targetId) {
			if (targetId != -1 && _targets.ContainsKey (targetId)) {
				_targets [targetId].isTrackable = true;
			} else {
				//Debug.LogWarning ("Target ID is invalid");
			}
		}

		public void DeactivateTarget(string targetPath) {
			int targetId = GetTargetId (targetPath);
			DeactivateTarget (targetId);
		}

		public void DeactivateTarget(int targetId){
			if (targetId != -1 && _targets.ContainsKey (targetId)) {
				_targets [targetId].isTrackable = false;
			} else {
				//Debug.LogWarning ("Target ID is invalid");
			}
		}

		void RemoveAllTrackable ()
		{
			_targetInfos.Clear ();
			_targets.Clear ();
		}

		public void SetTargetSize (int targetId, float width, float height) {
			Target target;
			if (_targets.TryGetValue (targetId, out target)) {
				target.width = width;
				target.height = height;
			}
		}

	    public bool GetTargetVisibility (int targetId) {
	        Target target;
	        if (_targets.TryGetValue (targetId, out target)) {
	            return target.isVisible;
	        }
	        return false;
	    }
			
	    public Vector3 GetTargetPosition (int targetId) {
	        Target target;
	        if (_targets.TryGetValue (targetId, out target)) {
				return target.position;
	        }
	        return Vector3.zero;
	    }

	    public Quaternion GetTargetOrientation (int targetId) {
	        Target target;
	        if (_targets.TryGetValue (targetId, out target)) {
				return target.orientation;
	        }
	        return Quaternion.identity;
	    }

		#endregion


		#region VIO/IMU

		Quaternion prevQuat;
		int sameCount = 0;
		//TODO: Temp fallback this should happen on native side
		public bool isVIODataInvalid()
		{
			if (_imuQuaternion.x == prevQuat.x && _imuQuaternion.y == prevQuat.y && _imuQuaternion.z == prevQuat.z && _imuQuaternion.w == prevQuat.w) {
				sameCount++;
				if (sameCount > 10) {
					bVIOMode = false;
					sameCount = 10;
				} 
			} else {
				sameCount--;
				if (sameCount < 0) {
					bVIOMode = true;
					sameCount = 0;
				}
			}
			prevQuat = _imuQuaternion;
			if (sameCount == 10) {
				return true;
			}

			return false;
		}

		public bool VIOEmulation
		{
			set { bVIOEmulation = value;}
			get { return bVIOEmulation; }
		}

		public bool isLandmarkTracking {
			set { }
			get{
				return bLandmarkTracking;
			}
		}

		public bool VIOMode
		{
			set {}
			get { return bVIOMode; }
		}

		public Vector3 GetPosition () {
			// return position from VIO eventually
			return _imuWorldPosition;
		}

		public Vector3 GetWorldVelocity () {
			// return position from VIO eventually
			return _imuWorldVelocity;
		}

		public Quaternion GetOrientation () {
			// return orientation from VIO eventually
			// for now the quaternion from the IMU is a good stand in
			return _imuQuaternion;
		}

		public bool HasIMUData () {
			return _hasIMUData;
		}

		public bool HasPoseData () {
			return _hasPoseData;
		}

		public Vector3 GetIMUGyro () {
			return _imuGyro;
		}

		public Quaternion GetIMUQuaternion () {
			return _imuQuaternion;
		}

		public void SimulateIMU (Vector3 gyro, Quaternion quat) {
			_imuGyro = gyro;
			_imuQuaternion = quat;
			_hasIMUData = true;
		}

		public void SimulateIMUPosition (Vector3 position) {
			_imuWorldPosition = position;
		}

		#endregion


		#region Sensors

		private bool StartCamera () {
			//Debug.Log ("ServiceManager: Start Camera");
			bool status = DSHUnityPlugin.StartCamera ();
			_enableVideoTextureUpdates = status;
			InitCamera ();
			return status;
		}

		private void StopCamera() {
			//Debug.Log ("ServiceManager: Stop Camera");
			DSHUnityPlugin.StopCamera ();
			_enableVideoTextureUpdates = false;
		}

		private bool StartThermal () {
			//Debug.Log ("ServiceManager: Start Thermal");
			bool status = DSHUnityPlugin.StartThermal ();
			_enableThermalUpdates = status;
			if (!status || VIOEmulation) {
				int defaultThermalWidth = 160;
				int defaultThermalHeight = 120;
				_thermalTexture = new Texture2D (defaultThermalWidth, defaultThermalHeight, TextureFormat.RGBA32, false);
				_thermalTexture = LoadPNG ("Texture/ThermalDefault");//Application.dataPath + "/DAQRI/System/Textures/ThermalDefault.png");
			} else {
				InitThermal ();
			}
			return status;
		}

		private void StopThermal() {
			//Debug.Log ("ServiceManager: Stop Thermal");
			DSHUnityPlugin.StopThermal ();
			_enableThermalUpdates = false;
		}

		private bool StartTracking () {
			//Debug.Log ("ServiceManager: Start Tracking");
			bool status = InitTracking ();
			_enableTracking = status;
			return status;
		}

		private void StopTracking () {
			//Debug.Log ("ServiceManager: Stop Tracking");
			RemoveAllTrackable ();
			VisionUnityPlugin.StopAR ();
			_enableTracking = false;
		}

		private bool StartIMU () {
			//Debug.Log ("ServiceManager: Start IMU");
			bool status = DSHUnityPlugin.StartIMU ();
			if (status) {
				_enableIMU = true;
			}
			return status;
		}

		private void StopIMU () {
			//Debug.Log ("ServiceManager: Stop IMU");
			DSHUnityPlugin.StopIMU ();
			_enableIMU = false;
			_hasIMUData = false;
		}

		private bool StartPositionMonitor () {
			//Debug.Log ("ServiceManager: Start VIO IMU");
			bool status = DSHUnityPlugin.StartPositionMonitor();
			if (status) {
				_enablePose = true;
			}
			return status;
		}

		private void StopPositionMonitor () {
			//Debug.Log ("ServiceManager: Stop VIO IMU");
			DSHUnityPlugin.StopPositionMonitor ();
			_enablePose = false;
			_hasPoseData = false;
		}

		#endregion

	    
		#region Private Methods

	    private void InitCamera () {

			//Debug.Log ("ServiceManager: Initializing Camera");

			int videoWidth, videoHeight;
			bool okVP = DSHUnityPlugin.GetColorCamParams(out videoWidth, out videoHeight);
			if (!okVP) {
				//Debug.LogWarning ("Failed to get Camera parameter, please make sure you have Color camera connected");
				return;
			}
	        else {
	            //Debug.Log ("Color camera Params: " + videoWidth + "x" + videoHeight);
	        }
			_videoColor32Array = null;
	        _videoColor32Array = new Color32[videoWidth * videoHeight];
			_videoTexture = null;
	        _videoTexture = new Texture2D (videoWidth, videoHeight, TextureFormat.RGB24, false);
	        _videoTexture.hideFlags = HideFlags.HideAndDontSave;
	        _videoTexture.filterMode = FilterMode.Bilinear;
	        _videoTexture.wrapMode = TextureWrapMode.Clamp;
	        _videoTexture.anisoLevel = 0;
			if (_useNativeGLTexture) {
				DAQRI.DSHUnityAbstraction.SetUnityColorRenderEventTextureID (_videoTexture.GetNativeTexturePtr ());
			}

			_visionAspectRatio = (float)videoWidth / (float)videoHeight;

			// This is temporary until we've decoupled the vision projection matrix from tracking
			_visionProjectionMatrix = Matrix4x4.Perspective (_visionFieldOfView, _visionAspectRatio, _nearPlane, _farPlane);
	    }

		private void InitThermal () {

			//Debug.Log ("ServiceManager: Initializing Thermal Camera");

			int thermalWidth, thermalHeight;

			bool okVP = DSHUnityPlugin.GetThermalCamParams(out thermalWidth, out thermalHeight);
			if (!okVP || !_enableThermalUpdates){
				//Debug.LogWarning ("Failed in InitThermal(), please make sure you have Thermal camera connected");
				return;
			}
			else{
				//Debug.Log ("Thermal camera Params: " + thermalWidth + "x" + thermalHeight);
			}
			_thermalTexture = null;
			_thermalColor32Array = new Color32[thermalWidth * thermalHeight];
			_thermalTexture = null; 
			_thermalTexture = new Texture2D (thermalWidth, thermalHeight, TextureFormat.RGBA32, false);

			_thermalTexture.hideFlags = HideFlags.None;
			_thermalTexture.filterMode = FilterMode.Bilinear;
			_thermalTexture.wrapMode = TextureWrapMode.Clamp;
			_thermalTexture.anisoLevel = 0;

			if (_useNativeGLTexture) {
				DAQRI.DSHUnityAbstraction.SetUnityThermalRenderEventTextureID (_thermalTexture.GetNativeTexturePtr ());
			}

			_thermalAspectRatio = (float)thermalWidth / (float)thermalHeight;
		}

		public static Texture2D LoadPNG(string filePath) {
			Texture2D tex = null;
			tex = Resources.Load (filePath, typeof(Texture2D)) as Texture2D;
			return tex;
		}

	    private void UpdateVideoTexture () {
			if (_useNativeGLTexture) {
				GL.IssuePluginEvent (DAQRI.DSHUnityAbstraction.GetRenderEventFunc(), (int)DSHUnityPlugin.UNITY_RENDER_EVENTID.UPDATE_TEXTURE_GL_COLOR);
			}
			else if (_videoColor32Array != null) {
	            bool updatedTexture = DSHUnityPlugin.GetColorCamData (_videoColor32Array);
	            if (updatedTexture) {
	                _videoTexture.SetPixels32 (_videoColor32Array);
	                _videoTexture.Apply ();
	            }
	        }
	    }

		private void UpdateThermalTexture () {
			if (_useNativeGLTexture) {
				GL.IssuePluginEvent (DAQRI.DSHUnityAbstraction.GetRenderEventFunc(), (int)DSHUnityPlugin.UNITY_RENDER_EVENTID.UPDATE_TEXTURE_GL_THERMAL);
			}
			else if (_thermalColor32Array != null) {
				bool updatedTexture = DSHUnityPlugin.GetThermalData (_thermalColor32Array);
				if (updatedTexture) {
					_thermalTexture.SetPixels32 (_thermalColor32Array);
					_thermalTexture.Apply ();
				}
			}
		}

		private bool InitTracking () {
	
			//Debug.Log ("Springboard: InitTracking");

			if (_targetInfos.Count == 0) {
    	        return false;
    	    }
			int numOfValidMarkers = _targetInfos.Count;

			int[] markerIds = new int[numOfValidMarkers]; //TODO : should be move ouside of this function
			float[] markerWidths = new float[numOfValidMarkers]; //TODO : should be move ouside of this function
			float[] markerHeights = new float[numOfValidMarkers]; //TODO : should be move ouside of this function

			for (int index = 0; index < numOfValidMarkers; index++) {
				TrackedObject toObj = _targetInfos [index].trackableObject.GetComponent<TrackedObject>();
				markerWidths [index] = toObj.WidthInMeters;
				markerHeights [index] = toObj.HeightInMeters;
			}
			VisionUnityPlugin.InitAR (_targetInfos.Select(x=>x.path).ToArray(),numOfValidMarkers,markerWidths,markerHeights, markerIds);
			
			for (int index = 0; index < numOfValidMarkers; index++) {
				//Debug.Log ("Unity MarkerId[0]:" + markerIds [index] + " " + finalPaths [index]);//assumed to be in order
				Target target = new Target ();
				target.id = _markerIds [index];
				target.info = _targetInfos [index];
				Debug.Log (_targetInfos [index].path);
				target.isVisible = false;
				target.isTrackable = true;
				if(!_targets.ContainsKey(target.id))
					_targets.Add (target.id, target);
			}
				
			float[] projMatrixRawArray = new float[16];
			VisionUnityPlugin.GetProjectionMatrix (projMatrixRawArray);
			Matrix4x4 projMatrixRaw = MatrixFromFloatArray (projMatrixRawArray);
			Matrix4x4 projectionMatrix = projMatrixRaw;
			_visionProjectionMatrix = projectionMatrix;
			
			_visionFieldOfView = FieldOfViewFromProjection (_visionProjectionMatrix);
			_visionAspectRatio = AspectRatioFromProjection (_visionProjectionMatrix);
			_hasVisionParameters = true;

			if (TargetIdsAvailable != null) {
				TargetIdsAvailable.Invoke ();
			}
			if (VisionParametersAvailable != null) {
				VisionParametersAvailable.Invoke ();
			}

			return true;
		}

		private void UpdateTracking () {
			
			VisionUnityPlugin.UpdateAR ();
			
			//Set all the targets to be invisible
			foreach (var entry in _targets) {
				Target target = entry.Value;
				int targetId = entry.Key;

				Vector3 position = Vector3.zero;
				Quaternion orientation = Quaternion.identity;
				bool visible = VisionUnityPlugin.GetPose (ref position, ref orientation, targetId);

				// Debug.Log ("visible " + targetId + "/" + visible + "/" + position);

				if (visible) {
					//float scale = Math.Min (target.width, target.height);
					target.isVisible = true;
					target.position = position;
					target.orientation = orientation;
				} else {
					target.isVisible = false;
					target.position = Vector3.zero;
					target.orientation = Quaternion.identity;
				}
			}
        }

		float[] quatTemp = new float[4];
		float[] gyroTemp = new float[3];
	    private void UpdateIMU () {
			DSHUnityPlugin.GetIMUQuaternion (quatTemp);
			_imuQuaternion.x = quatTemp [0];
			_imuQuaternion.y = quatTemp [1];
			_imuQuaternion.z = quatTemp [2];
			_imuQuaternion.w = quatTemp [3];

			DSHUnityPlugin.GetIMUGyro (gyroTemp);
			_imuGyro.x = gyroTemp[0];
			_imuGyro.y = gyroTemp[1];
			_imuGyro.z = gyroTemp[2];

			_hasIMUData = true;
	    }

		private void UpdatePose() {
			if (DSHUnityPlugin.GetVIOData (ref viodata)) {
				_imuQuaternion = viodata.Quat;
				_imuWorldPosition = viodata.WorldPosition;
				_imuWorldVelocity = viodata.WorldVelocity;
				_hasPoseData = true;
			}
			if (isVIODataInvalid ()) {
				UpdateIMU ();
			}
		}
			
		#endregion


		#region Utility

		/// <summary>
		/// Creates a Unity matrix from an array of floats.
		/// </summary>
		/// <param name="values">Array of 16 floats to populate the matrix.</param>
		/// <returns>A new <see cref="Matrix4x4"/> with the given values.</returns>
		public static Matrix4x4 MatrixFromFloatArray(float[] values)
		{
			if (values == null || values.Length < 16) throw new ArgumentException("Expected 16 elements in values array", "values");

			Matrix4x4 mat = new Matrix4x4();
			for (int i = 0; i < 16; i++) mat[i] = values[i];
			return mat;
		}
        /// <summary>
        /// Creates a Unity vector3 from an array of floats.
        /// </summary>
        /// <param name="values">Array of 3 floats to populate the vector3.</param>
        /// <returns>A new <see cref="Vector3"/> with the given values.</returns>
        public static Vector3 Vector3FromFloatArray(float[] values)
        {
            if (values == null || values.Length < 3) throw new ArgumentException("Expected 3 elements in values array", "values");

            Vector3 v = new Vector3();
            for (int i = 0; i < 3; i++) v[i] = values[i];
            return v;
        }
        /// <summary>
        /// Creates a Unity quaternion from an array of floats.
        /// </summary>
        /// <param name="values">Array of 4 floats to populate the quaternion.</param>
        /// <returns>A new <see cref="Vector3"/> with the given values.</returns>
        public static Quaternion QuaternionFromFloatArray(float[] values)
        {
            if (values == null || values.Length < 4) throw new ArgumentException("Expected 4 elements in values array", "values");

            Quaternion q = new Quaternion();
            for (int i = 0; i < 4; i++) q[i] = values[i];
            return q;
        }
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			// Trap the case where the matrix passed in has an invalid rotation submatrix.
			if (m.GetColumn(2) == Vector4.zero) {
				//ARController.Log("QuaternionFromMatrix got zero matrix.");
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}

		public static Vector3 PositionFromMatrix(Matrix4x4 m)
		{
			return m.GetColumn(3);
		}

		public float FieldOfViewFromProjection (Matrix4x4 projection) {
			float t = projection.m11;
			return Mathf.Atan (1.0f / t) * 2.0f * Mathf.Rad2Deg;
		}

		public float AspectRatioFromProjection (Matrix4x4 projection) {
			return projection.m11 / projection.m00;
		}

		#endregion
	}
}
