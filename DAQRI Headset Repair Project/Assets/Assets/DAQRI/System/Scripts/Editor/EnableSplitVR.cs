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
 *     File Purpose:        Enables VR support                                                                                          *
 *                                                                                                                                      *
 *     Guide:               If you comment this out (if you are using multiple VR Devices in one project), make sure you include split. *
 *                          You would then need to run VRSettings.LoadDeviceByName("split") before using DAQRI VR.                      *
 *                          See: https://docs.unity3d.com/ScriptReference/VR.VRSettings.LoadDeviceByName.html                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
using UnityEditor;
using UnityEditorInternal.VR;

namespace DAQRI {

    [InitializeOnLoad]
    internal static class EnableSplitVR {
        static EnableSplitVR() {
            PlayerSettings.virtualRealitySupported = true;

#if UNITY_5_5_OR_NEWER
            VREditor.SetVREnabledDevicesOnTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup, new[] {"split"});
#else
            VREditor.SetVREnabledDevices(EditorUserBuildSettings.selectedBuildTargetGroup, new[] {"split"});
#endif
        }
    }
}