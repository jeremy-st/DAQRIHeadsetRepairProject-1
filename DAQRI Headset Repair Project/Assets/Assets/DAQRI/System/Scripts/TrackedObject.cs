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
	
	public class TrackedObject : MonoBehaviour {

		/// <summary>
		/// Invoked when tracker is found on this target
		/// </summary>
		[SerializeField]
		public UnityEvent OnTrackerFound;

		/// <summary>
		/// Invoked when tracker is lost on this target
		/// </summary>
		[SerializeField]
		public UnityEvent OnTrackerLost;

		/*/// <summary>
		/// Invoked when tracker is found on this target
		/// </summary>
		[SerializeField]
		public UnityEvent OnMarkerlessTrackingGained;

		/// <summary>
		/// Invoked when tracker is lost on this target
		/// </summary>
		[SerializeField]
		public UnityEvent OnMarkerlessTrackingLost;*/

	    [HideInInspector]
	    public int targetId = -1;

		[SerializeField]
		[HideInInspector]
		public Texture previewImage;

		/// <summary>
		/// Target image filePath, with extension
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public string targetPath;

		/// <summary>
		/// Target image filename, with extension
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public string targetName;

		/// <summary>
		/// Real world unit in meters : 1.0f = 1 meter
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public float WidthInMeters = 1.0f;

		/// <summary>
		/// Real world unit in meters : 1.0f = 1 meter
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
		/// Tells you if the target is currently visible based on tracking
		/// </summary>
		public bool IsVisible { get; private set; }
		public bool IsVIOActive { get; private set; }

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
		public enum TRACKINGSTATE
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
		#if !UNITY_EDITOR
		void Awake () {
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

		public void LateInitialization () {
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
			IsVIOActive = ServiceManager.Instance.VIOMode;

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

					} /*else if (!IsVisible && previousVisiblity) {
						TrackerLost ();
					}*/
					/*else if (IsVIOActive && !previousVIOActive) {
						TrackerFound ();
					}else */if (!IsVIOActive && previousVIOActive) {
						TrackerLost ();
					}

					if (IsVisible) {
						Quaternion quat = ServiceManager.Instance.GetTargetOrientation (targetId);
						if (!quat.Equals(Quaternion.identity)) {
							//lastTrackedtime = Time.time;
							if (ServiceManager.Instance.isLandmarkTracking || !ServiceManager.Instance.VIOMode) {
								var pos = ServiceManager.Instance.GetTargetPosition (targetId);
								var rot = ServiceManager.Instance.GetTargetOrientation (targetId);

								transform.localPosition = pos;
								transform.localRotation = rot;
							}

							if (timesTracked++ < 30 && !ServiceManager.Instance.isLandmarkTracking && ServiceManager.Instance.VIOMode) {
								targetPos = ServiceManager.Instance.GetTargetPosition (targetId);
								targetQuat = ServiceManager.Instance.GetTargetOrientation (targetId);

								transform.position = targetPos;
								transform.rotation = targetQuat;

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

	    public void FetchTargetId () {
	        targetId = ServiceManager.Instance.GetTargetId (targetPath);
			Debug.Log ("TrackedObject: " + targetPath + " - " + targetId);
			ServiceManager.Instance.SetTargetSize (targetId, WidthInMeters, WidthInMeters / Aspect);
	    }
			
		public void RegisterToCallbacks(ITrackedObjectEventHandler handler)
		{
			if (!handlers.Contains (handler)) {
				handlers.Add (handler);
			}
		}

		public void UnregisterToCallbacks(ITrackedObjectEventHandler handler)
		{
			if (handlers.Contains (handler)) {
				handlers.Remove (handler);
			}
		}

		public bool TestVisibleEditor()
		{
			// Find the viewpoint points of the bottom left and top right vertices
			Vector3 localPoint1 = new Vector3 (-WidthInMeters / 2.0f, -HeightInMeters / 2.0f, 0.0f);
			Vector3 localPoint2 = new Vector3 (WidthInMeters / 2.0f, HeightInMeters / 2.0f, 0.0f);
			Vector3 worldPoint1 = transform.TransformPoint (localPoint1);
			Vector3 worldPoint2 = transform.TransformPoint (localPoint2);
			Vector3 viewportPoint1 = Camera.main.WorldToViewportPoint (worldPoint1);
			Vector3 viewportPoint2 = Camera.main.WorldToViewportPoint (worldPoint2);

			// Clamp to visible viewport space
			float x1 = Mathf.Clamp (viewportPoint1.x, 0.0f, 1.0f);
			float y1 = Mathf.Clamp (viewportPoint1.y, 0.0f, 1.0f);
			float x2 = Mathf.Clamp (viewportPoint2.x, 0.0f, 1.0f);
			float y2 = Mathf.Clamp (viewportPoint2.y, 0.0f, 1.0f);

			// Find the percent of viewport width and height filled
			float onscreenWidth = x2 - x1;
			float onscreenHeight = y2 - y1;

			// Target must fill threshold percent of the smaller dimension of the screen
			float screenAspect = Screen.width / Screen.height;
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
