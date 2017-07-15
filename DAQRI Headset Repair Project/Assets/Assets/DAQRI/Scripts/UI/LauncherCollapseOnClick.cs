/****************************************************************************************************************************************
 * © 2017 Daqri International. All Rights Reserved.                                                                                     *
 *                                                                                                                                      *
 *     NOTICE:  All software code and related information contained herein is, and remains the property of DAQRI INTERNATIONAL and its  *
 * suppliers, if any.  The intellectual and technical concepts contained herein are proprietary to DAQRI INTERNATIONAL and its          *
 * suppliers and may be covered by U.S. and Foreign Patents, patents in process, and/or trade secret law, and the expression of         *
 * those concepts is protected by copyright law. Dissemination, reproduction, modification, public display, reverse engineering, or     *
 * decompiling of this material is strictly forbidden unless prior written permission is obtained from DAQRI INTERNATIONAL.             *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *     File Purpose:        Triggers the launcher collapse animation on a pointer click event.                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace DAQRI {

	public class LauncherCollapseOnClick : LauncherBaseAnimation, IPointerClickHandler {

		private Animator launcherAnimator;

		/// <summary>
		/// Triggers the launcher collapse animation.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick (PointerEventData eventData) {
			if (launcherAnimator == null) {
				launcherAnimator = FindLauncherAnimator ();
			}

			if (launcherAnimator) {
				launcherAnimator.SetBool (LAUNCHER_ANIMATION_NAME, false);

			} else {
				Debug.LogWarning ("Couldn't find launcher animator in the scene");
			}
		}

		/// <summary>
		/// Finds the launcher animator in the current scene.
		/// </summary>
		/// <returns>The launcher animator.</returns>
		private Animator FindLauncherAnimator () {
			Animator[] animators = GetComponentsInParent<Animator> ();

			foreach (Animator animator in animators) {
				if (animator.name == LAUNCHER_ANIMATOR_NAME) {
					return animator;
				}
			}

			return null;
		}
	}
}
