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
using UnityEngine;

namespace DAQRI {

	public class VIOData : IMUData {

		public const int VIO_DATA_SIZE=34;

		private Vector3 worldposition = new Vector3 (0.0f,0.0f,0.0f);
		private Vector3 worldvelocity = new Vector3 (0.0f,0.0f,0.0f);

		/// <summary>
		/// Gets the world position.
		/// </summary>
		/// <value>The world position.</value>
		public Vector3 WorldPosition {
			get {
				return worldposition;
			} 
		}

		/// <summary>
		/// Gets the world velocity.
		/// </summary>
		/// <value>The world velocity.</value>
		public Vector3 WorldVelocity {
			get {
				return worldvelocity;
			}
		}

		public VIOData ()
		{
		}

		/// <summary>
		/// Sets the VIO data from float arrays.
		/// </summary>
		/// <param name="pos">Position array.</param>
		/// <param name="rot">Rotation array.</param>
		public void SetNativeData(float[] pos, float[] rot) {
			int dim = 3;
			for (int i = 0; i < dim; ++i) {
				worldposition[i] = pos[i];
			}
			dim = 4;
			for (int i = 0; i < dim; ++i) {
				quat[i] = rot[i]; 
			}
		}
	}
}
