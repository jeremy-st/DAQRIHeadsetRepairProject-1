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
	
	public class IMUData {
		
		public const int IMU_DATA_SIZE=18;

		//private float[] imuData = new float[IMU_DATA_SIZE];
		protected Vector3 gyro = new Vector3 (0.0f,0.0f,0.0f);//float.NaN, float.NaN, float.NaN);
		protected Vector3 acc = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN);
		protected Vector3 mag = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN);
		protected Vector3 eul = new Vector3 (0.0f,0.0f,0.0f);//(float.NaN, float.NaN, float.NaN);
		protected Quaternion quat = new Quaternion (0.0f,0.0f,0.0f,1.0f);//(float.NaN, float.NaN, float.NaN, float.NaN);

		public Vector3 Gyro {
			get {
				return gyro;
			}
		}
		public Vector3 Acc {
			get {
				return acc;
			}
		}
		public Vector3 Mag {
			get {
				return mag;
			}
		}
		public Vector3 Eul {
			get {
				return eul;
			}
		}
		public Quaternion Quat {
			get {
				return quat;
			}
		}
		public IMUData ()
		{

		}

		public virtual void SetNativeData(float[] data) {
			if (data.Length < IMU_DATA_SIZE) {
				Debug.LogWarning("IMU size incorrect -- " + data.Length);
				return;
			}
			int index = 0;
			int dim = 3;
			for (int i = 0; i < dim; ++i) {
				gyro[i] = data[index++]; 
			}
			
			for (int i = 0; i < dim; ++i) {
				acc[i] = data[index++]; 
			}

			for (int i = 0; i < dim; ++i) {
				mag[i] = data[index++]; 
			}

			for (int i = 0; i < dim; ++i) {
				eul[i] = data[index++]; 
			}
			dim = 4;
			for (int i = 0; i < dim; ++i) {
				quat[i] = data[index++]; 
			}
		}
	}
}
