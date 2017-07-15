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

    public static class CameraExtentions {

        public static void MakeMain(this Camera camera) {
            if ((Camera.main != null) && (Camera.main != camera)) {
                Debug.LogWarningFormat("{0} is replacing an existing camera as the MainCamera", camera.name);
                Camera.main.tag = "Untagged";
            }

            camera.tag = "MainCamera";
        }
    }
}