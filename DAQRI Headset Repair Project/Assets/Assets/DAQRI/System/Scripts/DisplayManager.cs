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
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DAQRI {
	
	public class DisplayManager : MonoBehaviour {
		

		public DisplayCamera mainDisplayCamera = null;
		public event System.Action ScreenSizeChanged;

		private GameObject videoBackdrop;
		private GameObject thermalBackdrop;

		private bool bInternalDeveloperAccess = false;

		private int prevScreenWidth = 0;
		private int prevScreenHeight = 0;
		private Vector3 prevPos;
		private Vector4 prevOrientation;

		#region Singleton

		private static DisplayManager instance;

		public static DisplayManager Instance {
			get {
				if (instance == null) {
					instance = GameObject.FindObjectOfType<DisplayManager> ();
				}
				return instance;
			}
		}

		#endregion


		#region MonoBehavior Events

		void Awake () {
			InitCameras ();
			InitReticle ();
			InitCameraBackdrops ();
		}

		void Start () {	
			Screen.SetResolution(2736, 768, true);
			if (Application.isEditor && !bInternalDeveloperAccess) {
				gameObject.AddComponent<VIOEmulator> ();
				ServiceManager.Instance.VIOEmulation = true;
			} else {
				ServiceManager.Instance.RegisterVIOUser (this);
			}
		}

		void Update () {
			if (Screen.width != prevScreenWidth || Screen.height != prevScreenHeight) {
				if (ScreenSizeChanged != null) {
					ScreenSizeChanged.Invoke ();
				}
				prevScreenWidth = Screen.width;
				prevScreenHeight = Screen.height;
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
		/// Turns the video background on.
		/// </summary>
		public void TurnVideoBackgroundOn () {
			videoBackdrop.SetActive (true);
			thermalBackdrop.SetActive (false);
		}

		/// <summary>
		/// Turns the video background off.
		/// </summary>
		public void TurnVideoBackgroundOff () {
			videoBackdrop.SetActive (false);
		}

		/// <summary>
		/// Turns the thermal background on.
		/// </summary>
		public void TurnThermalBackgroundOn () {
			thermalBackdrop.SetActive (true);
			videoBackdrop.SetActive (false);
		}

		/// <summary>
		/// Turns the thermal background off.
		/// </summary>
		public void TurnThermalBackgroundOff () {
			thermalBackdrop.SetActive (false);
		}

		#endregion


		#region Private Methods

		// Cameras are created at runtime
		// Currently we create a mono camera in the Unity editor and a stereo camera on the device
		// In the Unity editor we show the video background by default
		private void InitCameras () {
			if (Application.isEditor && !bInternalDeveloperAccess) {
				mainDisplayCamera = new MonoCamera (this, true);
			} else {
				mainDisplayCamera = new StereoCamera (this, true);
			}
			mainDisplayCamera.MakeMainCamera ();
		}

		// For optical see-through we use a world-space quad to represent the color and thermal cameras
		// The quad is sized using the field of view and aspect ratio of its respective camera
		// This allows for easy registration
		private void InitCameraBackdrops () {
			videoBackdrop = GameObject.CreatePrimitive (PrimitiveType.Quad);
			videoBackdrop.name = "Video Backdrop";
			videoBackdrop.transform.SetParent (transform, false);
			GameObject.Destroy (videoBackdrop.GetComponent<Collider> ());
			videoBackdrop.AddComponent<VideoBackdrop> ();
			videoBackdrop.SetActive (false);

			thermalBackdrop = GameObject.CreatePrimitive (PrimitiveType.Quad);
			thermalBackdrop.name = "Thermal Backdrop";
			thermalBackdrop.transform.SetParent (transform, false);
			GameObject.Destroy (thermalBackdrop.GetComponent<Collider> ());
			thermalBackdrop.AddComponent<ThermalBackdrop> ();
			thermalBackdrop.SetActive (false);
		}

		// The reticle is a world-space object
		// It uses the EventSystem to simulate a pointer
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
			reticleInputModule.reticleCamera = Camera.main;
		}

		#endregion
	}
}
