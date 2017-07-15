using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace DAQRI {

	public class Launcher : LauncherBaseAnimation, IPointerEnterHandler {
		
		private const float LAUNCHER_COLLAPSE_DISTANCE_FROM_RETICLE = 0.5f;

		/// <summary>
		/// The buttons that appear when the reticle hovers over the launcher.
		/// </summary>
		public List<GameObject> contentButtons;

		private Reticle reticle;
		private Animator animator;

		void Start () {
			reticle = FindObjectOfType<Reticle> ();
			animator = GetComponent<Animator> ();
		}

		void Update () {
//			CollapseIfFarFromReticle ();
		}

		void OnEnable () {
			if (animator != null) {
				animator.SetBool (LAUNCHER_ANIMATION_NAME, false);
			}

			foreach (GameObject button in contentButtons) {
				button.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
			}
		}

		/// <summary>
		/// Triggers the pointer enter animation.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter (PointerEventData eventData) {
			Animator animator = GetComponent<Animator> ();
			if (animator != null) {
				animator.SetBool (LAUNCHER_ANIMATION_NAME, true);
			}
		}

		/// <summary>
		/// Triggers the collapse animation if the reticle has moved too far away.
		/// </summary>
		private void CollapseIfFarFromReticle () {
			if (animator == null || reticle == null) {
				return;
			}

			Vector3 positionDifference = this.gameObject.transform.position - reticle.gameObject.transform.position;

			float xDifference = Mathf.Abs (positionDifference.x);
			float yDifference = Mathf.Abs (positionDifference.y);

			float distance = Mathf.Sqrt (Mathf.Pow (xDifference, 2) + Mathf.Pow (yDifference, 2));

			if (distance > LAUNCHER_COLLAPSE_DISTANCE_FROM_RETICLE) {
				animator.SetBool (LAUNCHER_ANIMATION_NAME, false);
			}
		}
	}
}
