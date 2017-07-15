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
 *     File Purpose:        Abstract class for a camera preview.                                                                        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI {

    public abstract class AbstractCameraPreview : MonoNeedingParentCanvas {

		/// <summary>
		/// Calculates the local scale for the camera preview image.
		/// </summary>
		/// <returns>The image local scale.</returns>
		/// <param name="textureWidth">Texture width.</param>
		/// <param name="textureHeight">Texture height.</param>
		public static Vector3 CalculateImageLocalScale (float textureWidth, float textureHeight) {
			float aspectRatio = (float)textureWidth / (float)textureHeight;

			// The flip in the y direction compensates for the different coordinate systems between Unity and the rendered texture
			return new Vector3 (aspectRatio, -1.0f, 1.0f);
		}
	}
}
