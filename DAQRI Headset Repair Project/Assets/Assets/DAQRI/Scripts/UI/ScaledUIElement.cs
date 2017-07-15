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
 *     File Purpose:        This script handles setting the transform scale for UI elements.                                            *
 *                          For nested objects that each use this script, the scale is set on the highest level parent,                 *
 *                          and the rest use a scale of one.                                                                            *
 *                                                                                                                                      *
 *     Guide:               Add this as a component of an object, the rest is done automatically.                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DAQRI {

	[ExecuteInEditMode]
	public class ScaledUIElement : MonoBehaviour {

		private bool isFirstScaleSet = true;
        private Vector3 defaultScale = new Vector3 (0.000942f, 0.000942f, 1.0f);
		private Vector3 unscaledScale = new Vector3 (1, 1, 1);

		void Update () {
			if (isFirstScaleSet) {
				ScaledUIElement[] scaledElements = GetComponentsInParent<ScaledUIElement> ();

				if (scaledElements.Length > 1) {
					// Parent has a scaled element component which will handle the scaling
					SetScale (unscaledScale);

				} else {
					// This is the only scaled element component, so set scale here
					SetScale (defaultScale);
				}
			}
		}

		/// <summary>
		/// Sets the scale on a rect transform if applicable,
		/// sets the scale on the transform otherwise.
		/// </summary>
		/// <param name="scale">Scale to set.</param>
		private void SetScale (Vector3 scale) {
			RectTransform rectTransfrom = GetComponent<RectTransform> ();
			isFirstScaleSet = false;

			if (rectTransfrom != null) {
				rectTransfrom.localScale = scale;

			} else {
				transform.localScale = scale;
			}
		}
	}
}
