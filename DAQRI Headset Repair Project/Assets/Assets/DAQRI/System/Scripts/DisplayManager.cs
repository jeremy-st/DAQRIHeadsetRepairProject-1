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
 *     File Purpose:        Singleton class for initializing the Display. Responsible for configuring cameras, video background, and    *
 *                          reticle. Transform is updated each frame to match current VIO/IMU position and orientation.                 *
 *                                                                                                                                      *
 *     Guide:               Call DisplayManager.Instance to access the singleton object.                                                *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

namespace DAQRI {

	public enum DeviceType {
		DAQRISmartHelmet = 0, 
		DAQRISmartGlasses = 1
	};
	
	public class DisplayManager : SceneSingleton {

		private const float DEFAULT_NEAR_CLIP_PLANE = 0.02f;
		private const float DEFAULT_FAR_CLIP_PLANE = 1000.0f;

		/// <summary>
		/// When the reticle is not targeting, this should be the distance at which it returns to.
		/// </summary>
		public const float DefaultReticlePosition = 2f;

		/// <summary>
		/// The near clip plane for the reticle.
		/// </summary>
        public const float EnforcedReticleNearPlane = 0.5f;

		/// <summary>
		/// Occurs when screen size has changed.
		/// </summary>
		public event System.Action ScreenSizeChanged;

		[HideInInspector]
		[SerializeField]
		/// <summary>
		/// The device type to simulate when running in the editor.
		/// </summary>
		public DeviceType previewDeviceType = DeviceType.DAQRISmartHelmet;

		[HideInInspector]
		[SerializeField]
		/// <summary>
		/// Allows overriding of clip planes.
		/// </summary>
		public bool overrideClipPlanes = false;

		[HideInInspector]
		[SerializeField]
		/// <summary>
		/// Value of the near clip plane.
		/// Anything closer to the camera than the near clip plane will not be rendered.
		/// </summary>
		public float nearClipPlane = DEFAULT_NEAR_CLIP_PLANE;

		[HideInInspector]
		[SerializeField]
		/// <summary>
		/// Value of the far clip plane.
		/// Anything farther from the camera than the far clip plane will not be rendered.
		/// </summary>
		public float farClipPlane = DEFAULT_FAR_CLIP_PLANE;

	    [SerializeField, Header("Leave as false for Unity 5.5 builds."), Tooltip("This will not work in Unity 5.5 on linux runtime at the moment due to a Unity bug.\n\nOnce it has been fixed this should be removed.")]

		/// <summary>
		/// Turns on/off the use of Unity's VR libraries.
		/// </summary>
		private bool useSplitStereoVR;

        private GameObject videoBackdrop;
		private GameObject depthBackdrop;
		private GameObject thermalBackdrop;
		#if DAQRI_SMART_HELMET
		private bool bEditorSensorAccess = true;
		#else 
		private bool bEditorSensorAccess = false;
		#endif

		/// <summary>
		/// Gets or sets a value indicating whether real sensors and cameras should be used when running in the editor.
		/// Only set this to true if you're running the Unity editor on a DAQRI smart device.
		/// </summary>
		/// <value><c>true</c> if sensors are accessible when running in the editor; otherwise, <c>false</c>.</value>
		public bool EditorSensorAccess {
			get{ return bEditorSensorAccess; }
			set{ 
				bEditorSensorAccess = value;
			}
		}

		/// <summary>
		/// Gets the width of the screen.
		/// </summary>
		/// <value>The width of the screen.</value>
		public int ScreenWidth { get; private set; }

		/// <summary>
		/// Gets the height of the screen.
		/// </summary>
		/// <value>The height of the screen.</value>
		public int ScreenHeight { get; private set; }
		
		private Vector3 prevPos;
		private Vector4 prevOrientation;

		#region Singleton

		private static DisplayManager instance;

		/// <summary>
		/// The accessor for the <see cref="DAQRI.DisplayManager"/> singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		public static DisplayManager Instance {
			get {
				if (instance == null) {
					// Since more than one singleton may be in the scene, find the valid one
					DisplayManager[] instances = GameObject.FindObjectsOfType<DisplayManager> ();
					foreach (DisplayManager manager in instances) {
						if (!manager.isInvalidDuplicate) {
							instance = manager;
							break;
						}
					}
				}
				return instance;
			}
		}

		#endregion


		#region SceneSingleton

		private static int numberOfSingletonsInScene = 0;

		public override int NumberOfSingletonsInScene () {
			return numberOfSingletonsInScene;
		}

		public override void SetNumberOfSingletonsInScene (int number) {
			numberOfSingletonsInScene = number;
		}

		#endregion


		#region MonoBehavior Events

		public override void Awake () {
			base.Awake ();

			// If the base awake method deactivated this object, don't execute any more code
			if (!gameObject.activeInHierarchy) {
				return;
			}

			// Make sure we use default clip planes if not explicitly overriding
			if (!overrideClipPlanes) {
				nearClipPlane = DEFAULT_NEAR_CLIP_PLANE;
				farClipPlane = DEFAULT_FAR_CLIP_PLANE;
			}

			RunEnvironmentInfo.SetPreviewDeviceType (previewDeviceType);

			ServiceManager.Instance.SetNearClipPlane (nearClipPlane);
			ServiceManager.Instance.SetFarClipPlane (farClipPlane);

            Screen.SetResolution(2736, 768, true);
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            if (Application.isPlaying) {
				InitCameras ();
				InitReticle ();
            }
			//InitCameraBackdrops ();
		}

		void Start () {
			if (Application.isEditor && !bEditorSensorAccess) {
				gameObject.AddComponent<VIOEmulator> ();
				ServiceManager.Instance.VIOEmulation = true;
			} else {
				ServiceManager.Instance.RegisterVIOUser (this);
			}
			//Create audio listerner
			CreateAudioListerner ();
		}

		void Update () {
			if (Screen.width != ScreenWidth || Screen.height != ScreenHeight) {
                ScreenWidth = Screen.width;
                ScreenHeight = Screen.height;

                if (ScreenSizeChanged != null) {
					ScreenSizeChanged.Invoke ();
				}
			}

			//at any instance, hitting 'esc' on the keyboard takes you out of the application
			if (Input.GetKey(KeyCode.Escape))
			{
				Debug.Log("Quitting application");
				Application.Quit();
			}
		}

		void OnApplicationQuit()
		{
			if (ServiceManager.InstanceExists) {
				ServiceManager.Instance.UnregisterVIOUser (this);
			}
		}
		#endregion


		#region Public Methods

		/// <summary>
		/// Turns the full-screen video background on.
		/// </summary>
		public void TurnVideoBackgroundOn () {
			if (videoBackdrop == null) {
				CreateVideoBackdrop ();
			}
			if (thermalBackdrop != null) {
				thermalBackdrop.SetActive (false);
			}
			if (videoBackdrop != null) {
				videoBackdrop.SetActive (true);
			}
			if (depthBackdrop != null) {
				depthBackdrop.SetActive (false);
			}
		}

		/// <summary>
		/// Turns the full-screen video background off.
		/// </summary>
		public void TurnVideoBackgroundOff () {
			if (videoBackdrop != null) {
				videoBackdrop.SetActive (false);
			}
		}

		/// <summary>
		/// Turns the full-screen thermal background on.
		/// </summary>
		public void TurnThermalBackgroundOn () {
			if (thermalBackdrop == null) {
				CreateThermalBackdrop ();
			}
			if (thermalBackdrop != null) {
				thermalBackdrop.SetActive (true);
			}
			if (videoBackdrop != null) {
				videoBackdrop.SetActive (false);
			}
			if (depthBackdrop != null) {
				depthBackdrop.SetActive (false);
			}
		}

		/// <summary>
		/// Turns the full-screen thermal background off.
		/// </summary>
		public void TurnThermalBackgroundOff () {
			if (thermalBackdrop != null) {
				thermalBackdrop.SetActive (false);
			}
		}

		/// <summary>
		/// Turns the full-screen depth background on.
		/// </summary>
		public void TurnDepthBackgroundOn () {
			if (depthBackdrop == null) {
				CreateDepthBackdrop ();
			}
			if (thermalBackdrop != null) {
				thermalBackdrop.SetActive (false);
			}
			if (videoBackdrop != null) {
				videoBackdrop.SetActive (false);
			}
			if (depthBackdrop != null) {
				depthBackdrop.SetActive (true);
			}
		}

		/// <summary>
		/// Turns the full-screen depth background off.
		/// </summary>
		public void TurnDepthBackgroundOff () {
			if (depthBackdrop != null) {
				depthBackdrop.SetActive (false);
			}
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Creates an audio listener if there is not one in the scene.
		/// </summary>
		private void CreateAudioListerner()
		{
			int count = FindObjectsOfType (typeof(AudioListener)).Length;
			if(count == 0)
			{
				AudioListener audioListerner = null;
				if (audioListerner == null) {
					gameObject.AddComponent<AudioListener> ();
				}
			}
		}

		/// <summary>
		/// Cameras are created at runtime.
		/// </summary>
		private void InitCameras () {
            DisplayCamera mainDisplayCamera = new GameObject("Display Camera").AddComponent<DisplayCamera>();
            mainDisplayCamera.transform.SetParent(transform, false);
            mainDisplayCamera.Initialize(
                !Application.isEditor || bEditorSensorAccess,
                !Application.isEditor,
                useSplitStereoVR);
		}

		/// <summary>
		/// Creates the video backdrop.
		/// </summary>
		private void CreateVideoBackdrop()
		{
			videoBackdrop = CreateBackdropQuad ();
			videoBackdrop.name = "Video Backdrop";
			videoBackdrop.AddComponent<VideoBackdrop> ();
		}

		/// <summary>
		/// Creates the thermal backdrop.
		/// </summary>
		private void CreateThermalBackdrop()
		{
			thermalBackdrop = CreateBackdropQuad ();
			thermalBackdrop.name = "Thermal Backdrop";
			thermalBackdrop.AddComponent<ThermalBackdrop> ();
		}

		/// <summary>
		/// Creates the depth backdrop.
		/// </summary>
		private void CreateDepthBackdrop()
		{
			depthBackdrop = CreateBackdropQuad ();
			depthBackdrop.name = "Depth Backdrop";
			depthBackdrop.AddComponent<DepthBackdrop> ();
		}

		/// <summary>
		/// For optical see-through we use a world-space quad to represent the cameras.
		/// The quad is sized using the field of view and aspect ratio of its respective camera.
		/// This allows for easy registration.
		/// </summary>
		private GameObject CreateBackdropQuad () {
			GameObject backdrop = GameObject.CreatePrimitive (PrimitiveType.Quad);
			backdrop.transform.SetParent (transform, false);
			GameObject.Destroy (backdrop.GetComponent<Collider> ());
			return backdrop;
		}

		/// <summary>
		/// Initializes the reticle.
		/// The reticle is a world-space object.
		/// It uses the EventSystem to simulate a pointer.
		/// </summary>
		private void InitReticle () {

			GameObject reticleObject = GameObject.Instantiate (Resources.Load ("Prefabs/Reticle", typeof(GameObject))) as GameObject;
			reticleObject.name = "Reticle";
			reticleObject.transform.SetParent (transform, false);

			EventSystem eventSystem = EventSystem.current;

			if (eventSystem == null) {
				GameObject eventSystemObject = new GameObject ("EventSystem");
				eventSystem = eventSystemObject.AddComponent<EventSystem> ();
			}

			StandaloneInputModule standaloneModule = eventSystem.GetComponent<StandaloneInputModule> ();
			if (standaloneModule != null) {
				standaloneModule.enabled = false;
			}

			ReticleInputModule reticleInputModule = eventSystem.gameObject.AddComponent<ReticleInputModule> ();
			reticleInputModule.reticle = reticleObject.GetComponent<Reticle> ();
		}

		#endregion
	}
}
