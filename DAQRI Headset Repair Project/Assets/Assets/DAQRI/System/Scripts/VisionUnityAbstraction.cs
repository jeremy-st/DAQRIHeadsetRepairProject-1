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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DAQRI {
	
	public class VisionUnityAbstraction {
	//#if !UNITY_EDITOR || DAQRI_SMART_HELMET_OSX
		// The name of the external library containing the native functions
		#if UNITY_EDITOR
		private const string LIBRARY_NAME = "libunitywrapperspringboard";
		#else
		private const string LIBRARY_NAME = "unitywrapperspringboard";
		#endif

		[DllImport(LIBRARY_NAME, CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool L7_TrackerStart(string[] markerPath, int numMarkers, float[] width, float[] height, int[] markerId, int mode);

		[DllImport(LIBRARY_NAME, CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool L7_TrackerUpdate();

		[DllImport(LIBRARY_NAME, CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool L7_TrackerGetTrackablePose([MarshalAs(UnmanagedType.LPArray, SizeConst=3)] float[] pos, [MarshalAs(UnmanagedType.LPArray, SizeConst=4)] float[] rot, int markerId);

		[DllImport(LIBRARY_NAME, CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool L7_TrackerStop();

		[DllImport(LIBRARY_NAME, CallingConvention=CallingConvention.Cdecl)]
        public static extern void L7_TrackerGetProjectionMatrix([MarshalAs(UnmanagedType.LPArray, SizeConst=16)] out Matrix4x4 matrix);
	//#endif
	}
}
