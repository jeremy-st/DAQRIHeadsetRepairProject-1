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
 *     File Purpose:        <todo>                                                                                                      *
 *                                                                                                                                      *
 *     Guide:               <todo>                                                                                                      *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DAQRI {
	
	public class TrackedObject : SceneSingleton {

		/// <summary>
		/// Invoked when tracker is found on this target.
		/// </summary>
		[SerializeField]
		public UnityEvent OnTrackerFound;

		/// <summary>
		/// Invoked when tracker is lost on this target.
		/// </summary>
		[SerializeField]
		public UnityEvent OnTrackerLost;

		/// <summary>
		/// The target ID.
		/// This will be assigned internally.
		/// </summary>
	    [HideInInspector]
	    public int targetId = -1;

		/// <summary>
		/// The preview image of the marker.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public Texture previewImage;

		/// <summary>
		/// Target image file path, with extension.
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public string targetPath;

		/// <summary>
		/// Target image file name, with extension.
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public string targetName;

		/// <summary>
		/// Real world unit in meters : 1.0f = 1 meter.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public float WidthInMeters = 1.0f;

		/// <summary>
		/// Real world unit in meters : 1.0f = 1 meter.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public float HeightInMeters = 1.0f;

		private List<ITrackedObjectEventHandler> handlers = new List<ITrackedObjectEventHandler>();

		private bool initialized = false;
		private float timesTracked;
		private Vector3 targetPos;
		private Quaternion targetQuat;
		private float lastTrackedtime;

		/// <summary>
		/// Tells you if the target is currently visible based on tracking.
		/// </summary>
		public bool IsVisible { get; private set; }

		/// <summary>
		/// Tells you whether VIO is currently active.
		/// </summary>
		/// <value><c>true</c> if VIO is active; otherwise, <c>false</c>.</value>
		public bool IsVIOActive { get; private set; }


		#region SceneSingleton

		private static int _numberInScene = 0;

		public override int NumberOfSingletonsInScene () {
			return _numberInScene;
		}

		public override void SetNumberOfSingletonsInScene (int number) {
			_numberInScene = number;
		}

		#endregion


		/// <summary>
		/// Gets the aspect ratio based on the image provided.
		/// </summary>
		/// <value>The aspect ratio.</value>
		public float Aspect
		{
			get
			{
				if (previewImage != null)
				{
					return (previewImage.width / (1.0f * previewImage.height));
				}

				return WidthInMeters / HeightInMeters;
			}
		}

		/// <summary>
		/// Gets the size of the image.
		/// </summary>
		/// <value>The size.</value>
		public Vector2 Size
		{
			get
			{
				if (previewImage != null)
				{
					return new Vector2(previewImage.width, previewImage.height);
				}

				return new Vector2(WidthInMeters, HeightInMeters);
			}
		}

		/// <summary>
		/// Extended tracking
		/// </summary>
		internal enum TRACKINGSTATE
		{
			MARKER_TRACKING = 0,
			MARKERLESS_TRACKING = 1,
			MARKERLESS_TRACKING_LOST = 2,
			MARKERLESS_TRACKING_GAINED = 3,
			IDLE = 4,
		}

		TRACKINGSTATE currState = TRACKINGSTATE.MARKER_TRACKING;
		//private int timesTracked = 0;

		#region MonoBehavior Events
		#if !UNITY_EDITOR || DAQRI_SMART_HELMET
		public override void Awake () {
			base.Awake ();

			// If the base awake method deactivated this object, don't execute any more code
			if (!gameObject.activeInHierarchy) {
				return;
			}

			MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
			if (meshRenderer != null) {
				meshRenderer.enabled = false;
			}
	    }
		#endif

		void Start () {
			IsVisible = false;
			IsVIOActive = false;

			string dir = Application.streamingAssetsPath;
			targetPath = System.IO.Path.Combine(dir, targetName);
			ServiceManager.Instance.RegisterTarget (targetPath,this.gameObject);
			ServiceManager.Instance.TargetIdsAvailable += FetchTargetId;
		}

		 void LateInitialization () {
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
			initialized = true;
		}

		void TrackerLost()
		{
			//Invoke Unity Event linked from the inspector
			if (OnTrackerLost != null) {
				OnTrackerLost.Invoke ();
			}
			foreach (ITrackedObjectEventHandler handler in handlers) {
				handler.OnTrackedObjectLost (this);
			}
		}

		void TrackerFound()
		{
			//Invoke Unity Event linked from the inspector
			if (OnTrackerFound != null) {
				OnTrackerFound.Invoke ();
			}

			// just became visible
			foreach (ITrackedObjectEventHandler handler in handlers) {

				handler.OnTrackedObjectFound (this);
			}
		}

		void Update () {
			if (!initialized) {
				LateInitialization ();
			}
		    
			bool previousVisiblity = IsVisible;
			bool previousVIOActive = IsVIOActive;
            IsVisible = ServiceManager.Instance.GetTargetVisibility (targetId);
			IsVIOActive = ServiceManager.Instance.GetVIOIMUEnabled ();

			if (Application.isEditor && ServiceManager.Instance.VIOEmulation) {
				IsVisible = TestVisibleEditor ();
			}

			/*if (Time.time - lastTrackedtime > 1.0f) {
				timesTracked = 0;
				//currState = TRACKINGSTATE.MARKER_TRACKING;
			}*/
			
			switch (currState) {
			case TRACKINGSTATE.MARKER_TRACKING:
				{		
					if (IsVisible && !previousVisiblity) {
						TrackerFound ();
					} 

					if (!IsVIOActive && previousVIOActive) {
						TrackerLost ();
					}

					if (IsVisible) {
						Quaternion quat = ServiceManager.Instance.GetTargetOrientation (targetId);
						if (!quat.Equals (Quaternion.identity)) {
							//lastTrackedtime = Time.time;
							if (ServiceManager.Instance.GetVIOIMUEnabled ()) {
								targetPos = ServiceManager.Instance.GetTargetPosition (targetId);
								targetQuat = ServiceManager.Instance.GetTargetOrientation (targetId);

								transform.position = targetPos;
								transform.rotation = targetQuat;

							} else {
								var pos = ServiceManager.Instance.GetTargetPosition (targetId);
								var rot = ServiceManager.Instance.GetTargetOrientation (targetId);

								transform.localPosition = pos;
								transform.localRotation = rot;
							}
						}
					}
				}
				break;
			case TRACKINGSTATE.IDLE:
				break;
			}
		}

		#endregion

		/// <summary>
		/// Requests an ID for this target, and assigns it to <see cref="targetId"/>.
		/// </summary>
	    public void FetchTargetId () {
	        targetId = ServiceManager.Instance.GetTargetId (targetPath);
			Debug.Log ("TrackedObject: " + targetPath + " - " + targetId);
			ServiceManager.Instance.SetTargetSize (targetId, WidthInMeters, WidthInMeters / Aspect);
	    }

		/// <summary>
		/// Add a callback to be executed on tracker events.
		/// </summary>
		/// <param name="handler">Callback.</param>
		public void RegisterToCallbacks(ITrackedObjectEventHandler handler)
		{
			if (!handlers.Contains (handler)) {
				handlers.Add (handler);
			}
		}

		/// <summary>
		/// Remove a tracker event callback.
		/// </summary>
		/// <param name="handler">Callback.</param>
		public void UnregisterToCallbacks(ITrackedObjectEventHandler handler)
		{
			if (handlers.Contains (handler)) {
				handlers.Remove (handler);
			}
		}

		/// <summary>
		/// Tests if the tracked object is visible in the editor.
		/// </summary>
		/// <returns><c>true</c>, if visible, <c>false</c> otherwise.</returns>
		public bool TestVisibleEditor()
		{
			// Find the viewpoint points of the bottom left and top right vertices
			Vector3 localPoint1 = new Vector3 (-WidthInMeters / 2.0f, -HeightInMeters / 2.0f, 0.0f);
			Vector3 localPoint2 = new Vector3 (WidthInMeters / 2.0f, HeightInMeters / 2.0f, 0.0f);
			Vector3 worldPoint1 = transform.TransformPoint (localPoint1);
			Vector3 worldPoint2 = transform.TransformPoint (localPoint2);
			Vector3 viewportPoint1 = Camera.main.WorldToViewportPoint (worldPoint1);
			Vector3 viewportPoint2 = Camera.main.WorldToViewportPoint (worldPoint2);

			return TestVisibleEditor (Screen.width, Screen.height, viewportPoint1, viewportPoint2);
		}

		/// <summary>
		/// Tests if the tracked object is visible in the editor.
		/// </summary>
		/// <returns><c>true</c>, if visible, <c>false</c> otherwise.</returns>
		/// <param name="screenWidth">Screen width.</param>
		/// <param name="screenHeight">Screen height.</param>
		/// <param name="viewportBottomLeft">Viewport bottom left.</param>
		/// <param name="viewportTopRight">Viewport top right.</param>
		public bool TestVisibleEditor (float screenWidth, float screenHeight, Vector3 viewportBottomLeft, Vector3 viewportTopRight) {
			// Clamp to visible viewport space
			float x1 = Mathf.Clamp (viewportBottomLeft.x, 0.0f, 1.0f);
			float y1 = Mathf.Clamp (viewportBottomLeft.y, 0.0f, 1.0f);
			float x2 = Mathf.Clamp (viewportTopRight.x, 0.0f, 1.0f);
			float y2 = Mathf.Clamp (viewportTopRight.y, 0.0f, 1.0f);

			// Find the percent of viewport width and height filled
			float onscreenWidth = x2 - x1;
			float onscreenHeight = y2 - y1;

			// Target must fill threshold percent of the smaller dimension of the screen
			float screenAspect = screenWidth / screenHeight;
			float threshold = 0.25f;
			float xThreshold, yThreshold;
			if (screenAspect > 1.0f) {
				yThreshold = threshold;
				xThreshold = threshold / screenAspect;
			} else {
				xThreshold = threshold;
				yThreshold = threshold * screenAspect;
			}

			if (onscreenWidth > xThreshold && onscreenHeight > yThreshold) {
				return true;
			}

			return false;
		}
	}
}
