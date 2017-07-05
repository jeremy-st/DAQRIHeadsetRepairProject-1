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
 *     File Purpose:        Keeps track of the current clickable target the reticle is focused on. Animates the reticle and             *
 *                          calls the public OnDwellComplete action when the dwell time has elapsed.                                    *
 *                                                                                                                                      *
 *     Guide:               Call UpdateDwellTarget each frame with the current IPointerClickHandler target (or null).                   *
 *                          Assign a handler to the OnDwellComplete action. Currently works with ReticleInputModule.                    *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace DAQRI {
	
	public class Reticle : MonoBehaviour {

		public enum State { Idle, Hover, Dwell };
		public event System.Action<GameObject> OnDwellComplete;

		public float dwellClickTime = 0.7f;
		public float dwellAnimationLength = 1.0f;
		public Image idleImage;
		public Image hoverImage;
		public Image dwellImage;
		private Sprite[] dwellSprites;

		private GameObject dwellTarget;
		private bool triggered;

		private float originalDistance;
		private Vector3 originalScale;
		private float currDistance;
		void Start ()
		{
			originalDistance = transform.localPosition.z;
			originalScale = transform.localScale;
			dwellSprites = Resources.LoadAll<Sprite>("Texture/reticle-dwell");
			UpdateCenter (originalDistance);
		}

		public void Update()
		{
			UpdateDistance (null,originalDistance + ServiceManager.Instance.GetNearClipPlane ());
		}

		public void UpdateDistance (GameObject target, float distance)
		{
			// Note: distance is from target to screen
			// adjust by adding distance from camera to near clip plane
			// otherwise the reticle will float in front of the target
			distance += ServiceManager.Instance.GetNearClipPlane ();
			//Debug.Log ("distance: " + distance);
			RaycastHit hitInfo;
			if (Physics.Raycast (DisplayManager.Instance.transform.position, DisplayManager.Instance.transform.forward, out hitInfo)) {
				distance = hitInfo.distance;
			}
			//currDistance = distance;
			UpdateCenter (distance);
			float scaleFactor = distance / originalDistance;
			transform.localScale = originalScale * scaleFactor;
		}

		public void UpdateHoverTarget(GameObject target)
		{
			if (target == null)
			{
				SetState (State.Idle);
			}
			else
			{
				SetState (State.Hover);
			}
		}

		public void UpdateCenter (float distance) {
			Vector3 center = DisplayManager.Instance.mainDisplayCamera.ScreenCenter;
			center.z = distance;
			//Debug.Log(center.ToString("F5"));
			transform.localPosition = center;
		}

		public void UpdateDwellTarget(GameObject target)
		{
			if (target == null)
			{
				dwellTarget = null;
				StopCoroutine ("AnimateDwell");
			}
			else if (dwellTarget != target)
			{
				triggered = false;
				dwellTarget = target;
				StopCoroutine ("AnimateDwell");
				StartCoroutine ("AnimateDwell");
			}
		}

		private void SetState (State state)
		{
			switch (state)
			{
			case State.Idle:
				idleImage.gameObject.SetActive (true);
				hoverImage.gameObject.SetActive (false);
				dwellImage.gameObject.SetActive (false);
				break;
			case State.Hover:
				idleImage.gameObject.SetActive (false);
				hoverImage.gameObject.SetActive (true);
				dwellImage.gameObject.SetActive (false);
				break;
			case State.Dwell:
				idleImage.gameObject.SetActive (false);
				hoverImage.gameObject.SetActive (false);
				dwellImage.gameObject.SetActive (true);
				break;
			}
		}

		private IEnumerator AnimateDwell ()
		{
			float timeElapsed = 0.0f;

			while (timeElapsed < dwellAnimationLength)
			{
				SetState (State.Dwell);

				int frame = (int) (timeElapsed / dwellAnimationLength * dwellSprites.Length);
				frame = Mathf.Min (frame, dwellSprites.Length - 1);
				dwellImage.sprite = dwellSprites [frame];

				if (!triggered && timeElapsed >= dwellClickTime)
				{
					triggered = true;
					if (OnDwellComplete != null) {
						OnDwellComplete (dwellTarget);
					}
				}

				timeElapsed += Time.deltaTime;

				yield return 0;
			}
		}
	}
}
