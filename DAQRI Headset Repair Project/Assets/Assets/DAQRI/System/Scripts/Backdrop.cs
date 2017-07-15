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
 *     File Purpose:        Abstract class for a camera hierarchy created at runtime.                                                   *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DAQRI {

	abstract public class Backdrop : MonoBehaviour,IBackdrop {

		private static IRunEnvironmentInfo _runEnvironmentInfo = new RunEnvironmentInfo ();

		protected Material sharedMaterial;
		protected float[] posRaw = new float[3]; 
		protected float[] rotRaw = new float[4];

		void Start() {
			InitMaterial ();
			Setup ();
		}

		virtual public void Setup (){
		}

		/// <summary>
		/// Loads a default material.
		/// </summary>
		public void InitMaterial()
		{
			GetComponent<MeshRenderer> ().material = (Material)Resources.Load("Materials/Backdrop", typeof(Material));
		}

		/// <summary>
		/// Sets a texture on the mesh renderer.
		/// </summary>
		/// <param name="texture">Texture.</param>
		public void SetTexture(Texture2D texture)
		{
			GetComponent<MeshRenderer> ().material.SetTexture("_MainTex", texture);
		}

		/// <summary>
		/// Sets the run environment info.
		/// This can be useful when mocking behavior in tests.
		/// </summary>
		/// <param name="info">Run evironment info.</param>
		public void SetRunEnvironmentInfo(IRunEnvironmentInfo info){
			_runEnvironmentInfo = info;
		}

		/// <summary>
		/// Calculates the local position.
		/// </summary>
		/// <returns>The local position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="farClipPlane">Far clip plane.</param>
		public Vector3 CalculateLocalPosition (Vector3 position, Quaternion rotation, float farClipPlane) {
			return position + rotation * (new Vector3 (0.0f, 0.0f, farClipPlane * 0.95f));
		}

		/// <summary>
		/// Calculates the local scale.
		/// </summary>
		/// <returns>The local scale.</returns>
		/// <param name="thermalAspect">Thermal aspect ratio.</param>
		/// <param name="thermalFieldOfView">Thermal field of view.</param>
		/// <param name="farClipPlane">Far clip plane.</param>
		public Vector3 CalculateLocalScale (float thermalAspect, float thermalFieldOfView, float farClipPlane) {
			float dist = farClipPlane * 0.95f;
			float halfFov = Mathf.Deg2Rad * (thermalFieldOfView / 2.0f);
			float height = Mathf.Tan (halfFov) * dist * 2.0f;
			float width = thermalAspect * height;

			Vector3 localScale;

			switch (_runEnvironmentInfo.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				localScale = new Vector3 (width, -height, 1.0f);//proper mapping has to happen (opengl vs unity texture coordinate system)
				break;

			default:
				localScale = new Vector3 (width, height, 1.0f);
				break;
			}

			return localScale;
		}
}
}