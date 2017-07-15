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
using System.Collections;

namespace DAQRI {

	public class RunEnvironmentInfo : IRunEnvironmentInfo {

		private static DeviceType previewDeviceType;

		/// <summary>
		/// Set this from anywhere in the code and it will apply to all instances of this class.
		/// </summary>
		/// <param name="deviceType">Device type.</param>
		public static void SetPreviewDeviceType (DeviceType deviceType) {
			previewDeviceType = deviceType;
		}

		/// <summary>
		/// Get the current run environment.
		/// This is a non-static method to allow mocking & stubbing in tests,
		/// but the values returned will be the same across all instances of this class.
		/// </summary>
		/// <returns>The run environment.</returns>
		public RunEnvironmentType CurrentEnvironment () {
			if (IsRunningOnDevice ()) {
				return RunEnvironmentType.OnDevice;
			}

			RunEnvironmentType environment = RunEnvironmentType.EditorSmartHelmetPreview;

			switch (DisplayManager.Instance.previewDeviceType) {
			case (DeviceType.DAQRISmartHelmet):
				environment = RunEnvironmentType.EditorSmartHelmetPreview;
				break;

			case (DeviceType.DAQRISmartGlasses):
				environment = RunEnvironmentType.EditorSmartGlassesPreview;
				break;
			}

			return environment;
		}

		/// <summary>
		/// Checks if running on a real device.
		/// Labeled as virtual to allow stubbing in a partial mock of this class.
		/// </summary>
		/// <returns><c>true</c> if this instance is running on a real device; otherwise, <c>false</c>.</returns>
		public virtual bool IsRunningOnDevice () {
			bool isOnDevice = false;

			#if !UNITY_EDITOR || DAQRI_SMART_HELMET
			isOnDevice = true;
			#endif

			return isOnDevice;
		}
	}
}
