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
 *     File Purpose:        Defines an interface for responding to reticle dwell progression.                                           *
 *                                                                                                                                      *
 *     Guide:               Implement this interface in a class to respond to the reticle dwell progression                             *
 *                          and the final pointer click event.                                                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DAQRI {

    public interface IPointerDwellHandler : IPointerClickHandler {

        /// <summary>
        /// Implement this to respond to reticle dwell progression.
        /// The dwell fraction value is clamped between zero and one,
        /// and indicates the fraction of dwell that has been completed towards triggering a pointer click.
        /// The fraction completed is calculated for each frame, so no value is ever guarenteed (including zero and one).
        /// The only guarentee is that the values will lie between zero and one, inclusive.
        /// You should use IPointerClickHandler and IPointerExitHandler to handle resetting any changes you make to your objects,
        /// since this method is not called on a pointer exit or after a pointer click.
        /// </summary>
        /// <param name="fractionCompleted">A value between zero and one indicating the fraction of dwell that has been completed.</param>
        void OnPointerDwellProgress (float fractionCompleted);
    }
}


