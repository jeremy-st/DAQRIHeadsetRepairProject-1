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
 *     File Purpose:        Abstract Implementation for TrackedObjectEventHandler Callbacks
 *     Guide:               Extend this class into a component and override its methods and attach it to the respective TrackedObject   *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using System.Collections;

namespace DAQRI
{
	public abstract class AbstractTrackedObjectEventHandler : MonoBehaviour, ITrackedObjectEventHandler
	{
		private TrackedObject trackedObject;

		void Start()
		{
			trackedObject = GetComponent<TrackedObject>();
			if (trackedObject)
			{
				trackedObject.RegisterToCallbacks(this);
			}
		}

		void Destroy()
		{
			trackedObject.UnregisterToCallbacks(this);
		}

		public virtual void OnTrackedObjectFound(TrackedObject to) { }

		public virtual void OnTrackedObjectLost(TrackedObject to) { }

		//public virtual void OnMarkerlessTrackingGained (TrackedObject to) { }

		//public virtual void OnMarkerlessTrackingLost (TrackedObject to) { }
	}
}