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
 *     File Purpose:        Draws errors and warnings if multiple singleton instances are in the scene.                                 *
 *                                                                                                                                      *
 *     Guide:               Implement this abstract class. This class will decide when to draw different content based on the           *
 *                          number of singletons in the scene.                                                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace DAQRI {

	public abstract class SceneSingletonEditor : Editor {

		private const string IS_DUPLICATE_ERROR_FORMAT = "There is more than one {0} object in your scene. This {0} object will be ignored.";
		private const string DUPLICATES_EXIST_WARNING_FORMAT = "There is more than one {0} object in your scene. The other {0} objects will be ignored.";


		#region Abstract

		/// <summary>
		/// Implement to draw the valid editor content.
		/// </summary>
		internal abstract void DrawValidEditorContent ();

		#endregion


		public override void OnInspectorGUI () {
			SceneSingleton singleton = target as SceneSingleton;
			if (singleton == null) {
				return;
			}

			bool isPrefab = (PrefabUtility.GetPrefabParent(singleton.gameObject) == null && PrefabUtility.GetPrefabObject(singleton.gameObject) != null);
			if (isPrefab) {
				DrawValidEditorContent ();
				return;
			}

			if (singleton.isInvalidDuplicate) {
				string message = String.Format (IS_DUPLICATE_ERROR_FORMAT, singleton.GetType ().Name);
				EditorGUILayout.HelpBox (message, MessageType.Error);
				return;
			}

			if (singleton.NumberOfSingletonsInScene () > 1) {
				string message = String.Format (DUPLICATES_EXIST_WARNING_FORMAT, singleton.GetType ().Name);
				EditorGUILayout.HelpBox (message, MessageType.Warning);
			}

			DrawValidEditorContent ();
		}
	}
}
