using UnityEngine;
using UnityEngine.EventSystems;

namespace DAQRI {

	public class ButtonPointerAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		private const string ANIMATION_NAME = "pointer_enter";

		/// <summary>
		/// Triggers the pointer enter animation.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter (PointerEventData eventData) {
			Animator animator = GetComponent<Animator> ();
			if (animator) {
				animator.SetBool (ANIMATION_NAME, true);
			}
		}
		/// <summary>
		/// Triggers the pointer exit animation.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit (PointerEventData eventData) {
			Animator animator = GetComponent<Animator> ();
			if (animator) {
				animator.SetBool (ANIMATION_NAME, false);
			}
		}
	}
}
