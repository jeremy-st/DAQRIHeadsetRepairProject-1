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
using UnityEngine.UI;
using System.Collections;

namespace DAQRI {

	[ExecuteInEditMode]
	/// <summary>
	/// If no canvas is present as a parent component, this class automatically creates one.
	/// </summary>
	public class CreateParentCanvas : MonoBehaviour {

		private static Vector2 DEFAULT_CANVAS_SIZE = new Vector2 (1.28f, 0.72f);

		void Update () {
			Canvas canvas = GetComponentInParent<Canvas> ();

			if (canvas == null) {
				GameObject parent = new GameObject ();
				parent.name = "Canvas";

				canvas = parent.AddComponent<Canvas> ();
				canvas.gameObject.AddComponent<CanvasScaler> ();

				RectTransform rectTransform = canvas.GetComponent<RectTransform> ();
				rectTransform.sizeDelta = DEFAULT_CANVAS_SIZE;

				this.transform.parent = parent.transform;
			}
		}
	}
}
