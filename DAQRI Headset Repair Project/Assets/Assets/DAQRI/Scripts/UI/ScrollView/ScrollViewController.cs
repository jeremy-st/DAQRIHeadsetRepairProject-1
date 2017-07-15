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
 *     File Purpose:        Use this class to provide row information to a scroll view.                                                 *
 *                                                                                                                                      *
 *     Guide:               Implement this class and assign the object to the controller property of a ScrollView.                      *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAQRI {

    /// <summary>
    /// Implement this class and assign the object to the controller property of a ScrollView.
    /// </summary>
    public abstract class ScrollViewController : MonoBehaviour {

        /// <summary>
        /// Return the total number of rows to display in the scroll view.
        /// </summary>
        /// <returns>The number of rows.</returns>
        abstract public int NumberOfRows ();

        /// <summary>
        /// Return the text for a given row.
        /// Note that the first row is considered row number zero.
        /// </summary>
        /// <returns>The text for a row.</returns>
        /// <param name="row">The row number.</param>
        abstract public string TextForRow (int row);

        /// <summary>
        /// Implement this to respond to the reticle dwell on a row's button.
        /// Note that the first row is considered row number zero.
        /// </summary>
        /// <param name="row">The row number.</param>
        abstract public void DidSelectRow (int row);

        /// <summary>
        /// Override this method to do any custom setup of a cell.
        /// This will be called just before the cell appears in the scroll view.
        /// Note that the first row is considered row number zero.
        /// </summary>
        /// <param name="cell">The cell game object.</param>
        /// <param name="row">The row number.</param>
        virtual public void ConfigureCellForRow (GameObject cell, int row) {
            // noop
        }
    }
}
