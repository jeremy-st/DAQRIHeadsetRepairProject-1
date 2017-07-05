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
 *     File Purpose:       To recieve asset import callbacks to handle display of progress bar                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DAQRI
{
	public class TexturePostprocessor : AssetPostprocessor
	{

		void OnPreprocessTexture()
		{
			EditorUtility.DisplayProgressBar("File Copy In Progress", "Copying to " + assetPath, 0.5f);
		}

		void OnPostprocessTexture(Texture2D texture)
		{
			EditorUtility.ClearProgressBar();
		}
	}
}
