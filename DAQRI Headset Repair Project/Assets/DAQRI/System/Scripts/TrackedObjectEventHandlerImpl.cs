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
 *     File Purpose:       An example of a custom event handler that implements ITrackedObjectEventHandler interface to get callbacks   *
 *                         for target found and lost events                                                                             *
 *     Guide:              Attach this component to TrackedObject Prefab instance in the scene hierarchy                                *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI {
	
	public class TrackedObjectEventHandlerImpl : AbstractTrackedObjectEventHandler {

		private bool wasFound = false;

		public override void OnTrackedObjectFound(TrackedObject to)
		{
			Debug.Log(to.gameObject.name + " found at [" + Time.time + "]");
			// this is just one approach...
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
				wasFound = true;
			}
		}

		public override void OnTrackedObjectLost(TrackedObject to)
		{
			Debug.Log(to.gameObject.name + " lost at [" + Time.time + "]");
			if (wasFound) {
				foreach (Transform child in transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		/*public override void OnMarkerlessTrackingGained(TrackedObject to)
		{
			Debug.Log(to.gameObject.name + " Markerless Tracking gained at [" + Time.time + "]");
		}

		public override void OnMarkerlessTrackingLost(TrackedObject to)
		{
			Debug.Log(to.gameObject.name + " Markerless Tracking lost at [" + Time.time + "]");
			if (!ServiceManager.Instance.VIOMode || !wasFound) {
				foreach (Transform child in transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}*/

	}
}
