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
 *     File Purpose:        Handles automatic canvas generation for objects that inherit from MonoNeedingParentCanvas.                  *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace DAQRI {

	[CustomEditor (typeof (MonoNeedingParentCanvas), true)] // 'true' flag makes this apply to child classes as well
	public class MonoNeedingParentCanvasEditor : Editor {

		private static Vector2 DEFAULT_CANVAS_SIZE = new Vector2 (1.28f, 0.72f);

		private SerializedProperty didRunCanvasCreationProp;

		private bool DidRunCanvasCreation {
			get {
				return didRunCanvasCreationProp.boolValue;
			} 
			set {
				didRunCanvasCreationProp.boolValue = true;
				serializedObject.ApplyModifiedProperties ();
			}
		}

		/// <summary>
		/// Creates a parent canvas if needed.
		/// </summary>
		void OnEnable () {
			didRunCanvasCreationProp = serializedObject.FindProperty ("didRunCanvasCreation"); // Declared in MonoNeedingParentCanvas

			CreateCanvasIfNeeded ();
		}


		#region Canvas Creation

		/// <summary>
		/// Creates a parent canvas if needed.
		/// </summary>
		private void CreateCanvasIfNeeded () {
			MonoBehaviour targetMono = target as MonoBehaviour;
			if (targetMono == null) {
				Debug.LogWarning ("Could not access MonoBehavior object, therefore cannot create canvas");
				return;
			}

			if (NeedsCanvasGeneration (targetMono)) {
				if (IsCorrectTimeToRunCanvasCreation (targetMono)) {
					CreateParentCanvas (targetMono);
				}

			} else if (IsCorrectTimeToRunCanvasCreation (targetMono)) {
				// If parent canvas isn't needed, treat it as if canvas generation process was run
				DidRunCanvasCreation = true;
			}
		}

		/// <summary>
		/// Checks if an object needs a parent canvas generated.
		/// </summary>
		/// <returns><c>true</c>, if canvas generation is needed, <c>false</c> otherwise.</returns>
		/// <param name="targetMono">Object to run the check on.</param>
		private bool NeedsCanvasGeneration (MonoBehaviour targetMono) {
			if (DidRunCanvasCreation) {
				return false;
			}

			Canvas canvas = targetMono.GetComponentInParent<Canvas> ();
			return (canvas == null);
		}

		/// <summary>
		/// Determines whether canvas generation should be run at this moment, given the application and editor states.
		/// </summary>
		/// <returns><c>true</c> if is correct time to run canvas creation; otherwise, <c>false</c>.</returns>
		/// <param name="targetMono">Object to run the check on.</param>
		private bool IsCorrectTimeToRunCanvasCreation (MonoBehaviour targetMono) {
			if (Application.isPlaying) {
				return false;
			}

			// Ensure that an instance of the object is selected, not a prefab
			GameObject gameObject = targetMono.gameObject;
			bool isPrefabSelected = (PrefabUtility.GetPrefabParent(gameObject) == null && PrefabUtility.GetPrefabObject(gameObject) != null);
			if (isPrefabSelected) {
				return false;
			}

			// Ensure the object is selected
			if (Selection.activeGameObject != gameObject) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Creates the parent canvas.
		/// </summary>
		/// <param name="targetMono">Object needing the parent canvas.</param>
		private void CreateParentCanvas (MonoBehaviour targetMono) {
			GameObject canvasGameObject = new GameObject ();
			canvasGameObject.name = "Canvas";

			if (targetMono.transform.parent != null) {
				canvasGameObject.transform.SetParent (targetMono.transform.parent);
			}

			Canvas canvas = canvasGameObject.AddComponent<Canvas> ();
			canvas.gameObject.AddComponent<CanvasScaler> ();
			canvas.gameObject.AddComponent<GraphicRaycaster> ();

			RectTransform rectTransform = canvas.GetComponent<RectTransform> ();
			rectTransform.sizeDelta = DEFAULT_CANVAS_SIZE;

			targetMono.transform.SetParent (canvasGameObject.transform);

			DidRunCanvasCreation = true;
		}

		#endregion
	}
}
