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
 *     File Purpose:        Checks the scene on hierarchy changes for multiple instances of singletons. If found, it sets               *
 *                          the appropriate flags on the singletons and logs warnings.                                                  *
 *                                                                                                                                      *
 *     Guide:               This is run automatically because it's setup from a static initializer. No implementation is needed.        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace DAQRI {

	[InitializeOnLoad]
	public class SingletonValidator {

		private static int previousNumberOfSingletons = 0;

		static SingletonValidator () {
			RunSingletonValidationIfNeeded ();
			EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChange;
		}

		private static void OnHierarchyWindowChange () {
			RunSingletonValidationIfNeeded ();
		}

		/// <summary>
		/// Checks if there are multiple singletons in the scene, and marks the duplicates as invalid
		/// </summary>
		private static void RunSingletonValidationIfNeeded () {
			if (Application.isPlaying) {
				return;
			}

			// Don't run the validation unless number of singletons has changed
			SceneSingleton[] singletons = Object.FindObjectsOfType<SceneSingleton> ();
			if (singletons.Length == previousNumberOfSingletons) {
				return;
			}

			previousNumberOfSingletons = singletons.Length;

			// Group the singletons by type
			Dictionary<System.Type, List<SceneSingleton>> singletonsByType = new Dictionary<System.Type, List<SceneSingleton>> ();
			foreach (SceneSingleton singleton in singletons) {
				System.Type type = singleton.GetType ();

				if (singletonsByType.ContainsKey (type)) {
					singletonsByType [type].Add (singleton);

				} else {
					List<SceneSingleton> list = new List<SceneSingleton> ();
					list.Add (singleton);
					singletonsByType.Add (type, list);
				}
			}

			// For each type of singleton, run validation if needed
			foreach (List<SceneSingleton> list in singletonsByType.Values) {
				if (list.Count == 0) {
					continue;
				}

				list [0].SetNumberOfSingletonsInScene (list.Count);

				if (list.Count == 1) {
					// Only one singleton in scene, therefore it's valid
					list [0].isInvalidDuplicate = false;

					// This is needed so the updated values will be set on the object when the scene is played
					EditorUtility.SetDirty (list [0]);

				} else {
					if (NeedsValidation (list)) {
						RunValidation (list);
						Debug.LogErrorFormat ("Multiple {0} objects are not allowed, please remove the copies.", list[0].GetType ().Name);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the list of singletons needs validation.
		/// The list should contain all instances in the scene of a SINGLE TYPE of singleton object.
		/// </summary>
		/// <returns><c>true</c>, if validation is needsed, <c>false</c> otherwise.</returns>
		/// <param name="singletonsOfOneType">All singletons of one type.</param>
		static private bool NeedsValidation (List<SceneSingleton> singletonsOfOneType) {
			bool foundValidSingleton = false;
			bool needsValidation = false;

			foreach (SceneSingleton singleton in singletonsOfOneType) {
				if (!singleton.isInvalidDuplicate) {
					if (foundValidSingleton) {
						needsValidation = true;
					} else {
						foundValidSingleton = true;
					}
				}
			}

			return needsValidation;
		}

		/// <summary>
		/// Runs validation on the singletons, setting their state flags as needed.
		/// The list should contain all instances in the scene of a SINGLE TYPE of singleton object.
		/// </summary>
		/// <param name="singletonsOfOneType">All singletons of one type.</param>
		static private void RunValidation (List<SceneSingleton> singletonsOfOneType) {
			bool isBadSingleton = false;

			foreach (SceneSingleton singleton in singletonsOfOneType) {
				singleton.isInvalidDuplicate = isBadSingleton;
				isBadSingleton = true;

				// This is needed so the updated values will be set on the object when the scene is played
				EditorUtility.SetDirty (singleton);
			}
		}
	}
}
