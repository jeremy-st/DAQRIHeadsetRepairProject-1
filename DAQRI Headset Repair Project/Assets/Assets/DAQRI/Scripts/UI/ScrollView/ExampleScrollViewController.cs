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
 *     File Purpose:        Provides an example of a scroll view controller, and is used by the example scenes.                         *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DAQRI;

public class ExampleScrollViewController : ScrollViewController {

    public override int NumberOfRows () {
        return 14;
    }

    public override string TextForRow (int row) {
        return string.Format ("Row {0}", row.ToString ());
    }

    public override void DidSelectRow (int row) {
        Debug.LogFormat ("Selected row {0}!", row.ToString ());
    }
}
