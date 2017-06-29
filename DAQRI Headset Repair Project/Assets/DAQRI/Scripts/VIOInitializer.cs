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
 *     File Purpose:        Used by VIN Example. Initializes the gameobject it's attached to, based on the pose from the VIN IMU        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI 
{

	public class VIOInitializer : MonoBehaviour 
	{

		private bool initialized = false;
		private Vector3 offset;

		// Use this for initialization
		void Start () 
		{
			offset = new Vector3 (0.0f, 0.15f, 0.0f) + Camera.main.gameObject.transform.forward * 1.5f;
			transform.position = ServiceManager.Instance.GetPosition () + offset;
		}

		void Update () 
		{
			if (!initialized) 
			{
				LateInitialization ();
			}
		}

		private void LateInitialization () 
		{
			if (ServiceManager.Instance.HasIMUData ()) 
			{
				transform.position = ServiceManager.Instance.GetPosition () + offset;
				initialized = true;
			}
		}
	}

}
