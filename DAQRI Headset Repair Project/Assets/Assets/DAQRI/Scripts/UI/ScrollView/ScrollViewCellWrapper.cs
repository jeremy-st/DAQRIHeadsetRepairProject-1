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
 *     File Purpose:        Stores useful information about a scroll view cell.                                                         *
 *                                                                                                                                      *
 *     Guide:               Add a listener to the cell click event to respond to a reticle dwell on a 'Button' component in the cell.   *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DAQRI.Internal {

    public class ScrollViewCellWrapper {
        
        public readonly GameObject gameObject;
        public readonly Text cellText;

        public int currentRow = 0;
        public CellClickEvent OnCellClick;

        /// <summary>
        /// The cell wrapper check the game object for a 'Button' component.
        /// If it finds one, it adds a listener that will trigger the cell click event.
        /// </summary>
        /// <param name="instantiatedCell">Instantiated cell game object.</param>
        public ScrollViewCellWrapper (GameObject instantiatedCell) {
            gameObject = instantiatedCell;

            cellText = gameObject.GetComponentInChildren<Text> ();
            OnCellClick = new CellClickEvent ();

            gameObject.GetComponentInChildren<Button> ().onClick.AddListener (OnPointerClick);
        }

        private void OnPointerClick () {
            OnCellClick.Invoke (currentRow);
        }
    }
}
