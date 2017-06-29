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

		private Vector3 worldposition = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);
		private Vector3 worldvelocity = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);
		/*private Vector3 gyrobias = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);
		private Vector3 acclbias = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);
		private Vector4 rawquat = new Vector4 (0.0f,0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);*/


		public Vector3 WorldPosition {
			get {
				return worldposition;
			} 
		}
		public Vector3 WorldVelocity {
			get {
				return worldvelocity;
			}
		}

		public VIOData ()
		{
		}

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
