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
 *     File Purpose:        Use as a parent class for objects that should auto-generate a canvas when dragged into the scene.           *
 *                                                                                                                                      *
 *     Guide:               Simply declare a class as inheriting from this one.                                                         *
 *                          Canvas generation is automatically handled (by the editor script).                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

/// This disables the 'private field assigned but not used' warning.
/// The field is used in the editor script, 
/// but is accessed through a string passed to SerializedObject.FindProperty().
/// Therefore the comiler doesn't catch the usage, and logs a warning that doesn't apply.
#pragma warning disable 0414

namespace DAQRI {

	public class MonoNeedingParentCanvas : MonoBehaviour {

		[SerializeField]
		[HideInInspector]
		private bool didRunCanvasCreation = false; // Used by the editor script
	}
}
