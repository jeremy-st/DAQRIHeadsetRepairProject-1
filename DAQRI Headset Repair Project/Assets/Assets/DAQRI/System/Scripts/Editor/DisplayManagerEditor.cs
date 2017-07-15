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
using UnityEditor;
using System.Collections;

namespace DAQRI {

	[CustomEditor (typeof (DisplayManager))]
	public class DisplayManagerEditor : SceneSingletonEditor {
		
		// Constants
		private const string DISPLAY_HELP_TEXT_FORMAT = "The DisplayManager creates the appropriate camera setup at runtime. Typically this is mono in the editor and stereo on the {0}.";
		private const string PREVIEW_AS_LABEL_TEXT = "Preview as";
		private const string ADVANCED_FOLDOUT_TEXT = "Advanced";
		private const string OVERRIDE_PLANES_TOGGLE_TEXT = "Override Near/Far Planes";
		private const string NEAR_PLANE_TEXT = "Near Plane (m)";
		private const string FAR_PLANE_TEXT = "Far Plane (m)";
		private const string CLIP_PLANE_HELP_TEXT = "This optimizes how close or far the camera will render digital content. Only override this if you need to optimize for large polygon scenes or models.";
		private const float EMPTY_FLOAT_VALUE = -1;
		private const string DEVICE_PREVIEW_TYPE_EDITOR_PREF_KEY = "EditorPreviewDeviceType";

		public SerializedProperty previewDeviceTypeProperty;
		public SerializedProperty nearClipProperty;
		public SerializedProperty farClipProperty;
		public SerializedProperty overrideClipPlanes;

		bool isAdvancedFoldoutOpen = false;
		string previousFocusedControl = "";

		private float nearPlaneFieldValue = EMPTY_FLOAT_VALUE;
		private float farPlaneFieldValue = EMPTY_FLOAT_VALUE;

		void OnEnable () {
			previewDeviceTypeProperty = serializedObject.FindProperty ("previewDeviceType");
			nearClipProperty = serializedObject.FindProperty ("nearClipPlane");
			farClipProperty = serializedObject.FindProperty ("farClipPlane");
			overrideClipPlanes = serializedObject.FindProperty ("overrideClipPlanes");
			
			nearPlaneFieldValue = nearClipProperty.floatValue;
			farPlaneFieldValue = farClipProperty.floatValue;

			// Show the advanced section by default if clip values are being overridden
			if (overrideClipPlanes.boolValue) {
				isAdvancedFoldoutOpen = true;
			}
		}


		#region SceneSingletonEditor Override

		internal override void DrawValidEditorContent () {
			serializedObject.Update ();
			DrawDefaultInspector ();

			previewDeviceTypeProperty.intValue = (int)(DeviceType)EditorGUILayout.EnumPopup (PREVIEW_AS_LABEL_TEXT, (DeviceType)previewDeviceTypeProperty.intValue);

			string helpBoxText = string.Format (DISPLAY_HELP_TEXT_FORMAT, ((DeviceType)previewDeviceTypeProperty.intValue).ToDisplayString ());
			EditorGUILayout.HelpBox (helpBoxText, MessageType.Info);

			// Advanced content
			isAdvancedFoldoutOpen = EditorGUILayout.Foldout (isAdvancedFoldoutOpen, ADVANCED_FOLDOUT_TEXT);
			if (isAdvancedFoldoutOpen) {
				ShowAdvancedContent ();
			}

			serializedObject.ApplyModifiedProperties ();
		}

		#endregion

		/// <summary>
		/// Draws all content from the 'Advanced' foldout
		/// </summary>
		private void ShowAdvancedContent () {
			overrideClipPlanes.boolValue = EditorGUILayout.ToggleLeft (OVERRIDE_PLANES_TOGGLE_TEXT, overrideClipPlanes.boolValue, EditorStyles.boldLabel);

			// Start greying out content if overriding planes is disabled
			if (!overrideClipPlanes.boolValue) {
				GUI.enabled = false;
			}

			GUI.SetNextControlName (NEAR_PLANE_TEXT);
			nearPlaneFieldValue = EditorGUILayout.FloatField(NEAR_PLANE_TEXT, nearPlaneFieldValue);

			GUI.SetNextControlName (FAR_PLANE_TEXT);
			farPlaneFieldValue = EditorGUILayout.FloatField(FAR_PLANE_TEXT, farPlaneFieldValue);

			// Finish greying out content
			GUI.enabled = true;

			if (overrideClipPlanes.boolValue) {
				EditorGUILayout.HelpBox (CLIP_PLANE_HELP_TEXT, MessageType.Info);
			}

			ValidateAndUpdateClipPlanesIfNeeded ();
		}

		/// <summary>
		/// If any clip plane field just finished being edited, validation is run.
		/// If the validation passes, the serialized object is updated with the field value.
		/// </summary>
		private void ValidateAndUpdateClipPlanesIfNeeded () {
			string focusedControl = GUI.GetNameOfFocusedControl ();
			bool didFinishEditingNearPlane = (previousFocusedControl == NEAR_PLANE_TEXT && focusedControl != NEAR_PLANE_TEXT);
			bool didFinishEditingFarPlane = (previousFocusedControl == FAR_PLANE_TEXT && focusedControl != FAR_PLANE_TEXT);

			if (didFinishEditingNearPlane) {
				ValidateAndUpdateClipPlane (ref nearClipProperty, ref nearPlaneFieldValue);
			}

			if (didFinishEditingFarPlane) {
				ValidateAndUpdateClipPlane (ref farClipProperty, ref farPlaneFieldValue);
			}

			previousFocusedControl = focusedControl;
		}

		/// <summary>
		/// Validates the clip planes.
		/// If all planes are valid, the serialized property is updated with the field value.
		/// If all planes are not valid, it is assumed that the one passed to this method caused the issue, 
		/// and the field value is reset to the value of the serialized property.
		/// </summary>
		/// <param name="clipPlaneProperty">Clip plane serialized property.</param>
		/// <param name="clipPlaneValue">Clip plane field value.</param>
		private void ValidateAndUpdateClipPlane(ref SerializedProperty clipPlaneProperty, ref float clipPlaneFieldValue) {
			bool areClipPlanesValid = true;

			if (nearPlaneFieldValue <= 0) {
				areClipPlanesValid = false;
				Debug.LogWarning ("Near clip plane must be greater than zero");
			}

			if (nearPlaneFieldValue >= DisplayManager.EnforcedReticleNearPlane) {
				areClipPlanesValid = false;
				Debug.LogWarningFormat ("Near clip plane must be less than {0} due to rendering constraints", DisplayManager.EnforcedReticleNearPlane);
			}

			if (farPlaneFieldValue <= DisplayManager.DefaultReticlePosition) {
				areClipPlanesValid = false;
				Debug.LogWarningFormat ("Far clip plane must be greater than {0} due to rendering constraints", DisplayManager.DefaultReticlePosition);
			}

			if (areClipPlanesValid) {
				clipPlaneProperty.floatValue = clipPlaneFieldValue;

			} else {
				clipPlaneFieldValue = clipPlaneProperty.floatValue;
			}
		}
	}
}
