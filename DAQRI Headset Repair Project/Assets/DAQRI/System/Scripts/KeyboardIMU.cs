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
 *     File Purpose:        Simulates IMU movement using Horizontal and Vertical inputs (e.g. WASD or arrow keys).                      *
 *                          Hold ALT key to simulate roll.                                                                              *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI {
	
	public class KeyboardIMU : MonoBehaviour {
		
	    public float speed = 0.5f;

	    private Quaternion quat = Quaternion.identity;

	    void Awake()
	    {
	        if (!Application.isEditor)
	        {
	            enabled = false;
	        }
	    }

	    void Update()
	    {
	        float yAxisRotation = 0.0f;
	        float xAxisRotation = 0.0f;
	        float zAxisRotation = 0.0f;

	        if (Input.GetKey (KeyCode.LeftAlt) ||
	            Input.GetKey (KeyCode.RightAlt))
	        {
	            zAxisRotation = Input.GetAxis ("Horizontal") * -speed;
	        }
	        else
	        {
	            yAxisRotation = Input.GetAxis ("Horizontal") * speed;
	            xAxisRotation = Input.GetAxis ("Vertical") * -speed;
	        }

	        Vector3 xAxis = quat * Vector3.right;
	        Vector3 zAxis = quat * Vector3.forward;
	        quat = Quaternion.AngleAxis (xAxisRotation, xAxis) * quat;
	        quat = Quaternion.AngleAxis (yAxisRotation, Vector3.up) * quat;
	        quat = Quaternion.AngleAxis (zAxisRotation, zAxis) * quat;

			Vector3 deltas = new Vector3 (xAxisRotation, yAxisRotation, zAxisRotation);
			ServiceManager.Instance.SimulateIMU (deltas, quat);
	    }
	}
}
