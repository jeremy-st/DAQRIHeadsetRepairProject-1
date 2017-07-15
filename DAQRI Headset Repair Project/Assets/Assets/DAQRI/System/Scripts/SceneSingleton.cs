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
 *     File Purpose:        Works with the SceneSingletonEditor and SceneSingletonValidator scripts to enforce only one                 *
 *                          component being active in the scene.                                                                        *
 *                                                                                                                                      *
 *     Guide:               Implement this abstract class. The other two scripts will only allow one component of that type             *
 *                          to be active in the scene at a time.                                                                        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI {

	public abstract class SceneSingleton : MonoBehaviour {

		private const string MULTIPLE_INSTANCES_WARNING_FORMAT = "Multiple instances of {0} found in scene. This is not allowed, please remove the duplicate objects. Setting '{1}' to inactive as a temporary solution.";

		/// <summary>
		/// If true, there are multiple instances of the implemented type in the scene, 
		/// and this instance has been deemed invalid.
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public bool isInvalidDuplicate = false;

		/// <summary>
		/// Implement this to return the value set via the SetNumberOfSingletonsInScene method.
		/// </summary>
		/// <returns>The number of singletons of the child class type in scene.</returns>
		public abstract int NumberOfSingletonsInScene ();

		/// <summary>
		/// Implement this to keep track of the number of instances of the child class singleton.
		/// It's recommended you do this using a static variable on the child class.
		/// </summary>
		/// <param name="number">Number of singletons of that type in the scene.</param>
		public abstract void SetNumberOfSingletonsInScene (int number);

		/// <summary>
		/// This logs a warning during app play.
		/// Make sure you call this from the child class.
		/// </summary>
		public virtual void Awake () {
			if (Application.isPlaying) {
				if (isInvalidDuplicate) {
					Debug.LogWarningFormat (MULTIPLE_INSTANCES_WARNING_FORMAT, GetType ().Name, gameObject.name.ToString ());
					gameObject.SetActive (false);
				}
			}
		}
	}
}
