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
 *     File Purpose:        Adds functionality to the UnityEngine.Camera.                                                               *
 *                                                                                                                                      *
 *     Guide:                                                                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;

namespace DAQRI {

	public static class DeviceTypeExtensions {

		/// <summary>
		/// Returns a nice looking, readable string for the DeviceType enum to be displayed to the user.
		/// </summary>
		/// <returns>A naturally readable string.</returns>
		/// <param name="deviceType">Device type.</param>
		public static string ToDisplayString(this DeviceType deviceType) {
			string formattedString = null;

			switch (deviceType) {
			case (DeviceType.DAQRISmartHelmet):
				formattedString = "DAQRI Smart Helmet";
				break;

			case (DeviceType.DAQRISmartGlasses):
				formattedString = "DAQRI Smart Glasses";
				break;
			}

			return formattedString;
		}
	}
}
