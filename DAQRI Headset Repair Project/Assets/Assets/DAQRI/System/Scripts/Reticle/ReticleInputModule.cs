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
 *     File Purpose:        Makes the reticle simulate mouse events to work with the Unity event system.                                *
 *                                                                                                                                      *
 *     Guide:               Place this script on the EventSystem object. Deactivate Standalone Input Module.                            *
 *                          Each frame, the reticle position is transformed to screen space and used to simulate a pointer.             *
 *                          Currently PointerEnter, PointerExit, and PointerClick events are triggered.                                 *
 *                          To work with 3D objects, add a Physics Raycaster to the main camera.                                        *
 *                          To enable mouse, disable this script on the EventSystem object and enable Standalone Input Module.          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace DAQRI {
	
	public class ReticleInputModule : BaseInputModule {
		
		public Reticle reticle;

		private PointerEventData pointerEventData;
		private bool justClicked = false;
		private GameObject justClickedHandler;
		//private GameObject nextClickHandler;
        private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
        private static Vector3 viewPointMiddle = new Vector3(0.5f, 0.5f, DisplayManager.DefaultReticlePosition);

		protected override void Start()
		{
			pointerEventData = new PointerEventData (eventSystem);
			reticle.OnDwellComplete += ProcessClick;
		}

        public override void Process()
		{
		    viewPointMiddle.z = Camera.main.nearClipPlane;
            pointerEventData.position = Camera.main.ViewportToScreenPoint (viewPointMiddle);
			pointerEventData.delta = Vector2.zero;
			pointerEventData.dragging = false;

            raycastResults.Clear();
			eventSystem.RaycastAll (pointerEventData, raycastResults);
			pointerEventData.pointerCurrentRaycast = FindFirstRaycast (raycastResults);

			GameObject target = pointerEventData.pointerCurrentRaycast.gameObject;
            viewPointMiddle.z =
                target != null
                    ? Mathf.Clamp (pointerEventData.pointerCurrentRaycast.distance, DisplayManager.EnforcedReticleNearPlane,
                        Camera.main.farClipPlane)
                    : DisplayManager.DefaultReticlePosition;
            reticle.TargetWorldPosition = Camera.main.ViewportToWorldPoint (viewPointMiddle);

			HandlePointerExitAndEnter (pointerEventData, target);

			GameObject enterHandler = GetValidEnterHandler (target);
			reticle.UpdateHoverTarget (enterHandler);

			GameObject selectHandler = GetValidSelectHandler (target);
			if (selectHandler != null) {
				if (selectHandler != eventSystem.currentSelectedGameObject) {
					eventSystem.SetSelectedGameObject (selectHandler, pointerEventData);
				} else {
					// This needs to be called each frame for InputFields to work
					ExecuteEvents.Execute (selectHandler, pointerEventData, ExecuteEvents.updateSelectedHandler);
				}
			} else {
				if (eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject.GetComponent<InputField> ()) {
					// We want to keep focus on the InputField even after the user looks away from it
					ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, pointerEventData, ExecuteEvents.updateSelectedHandler);
				} else {
					eventSystem.SetSelectedGameObject (null, pointerEventData);
				}
			}

			GameObject clickHandler = GetValidClickHandler (target);
			if (justClicked)
			{
				UpdateJustClicked (clickHandler);
			}
			else
			{
				reticle.UpdateDwellTarget (clickHandler);
			}
		}

		public GameObject GetValidEnterHandler(GameObject root)
		{
			GameObject enterHandler = ExecuteEvents.GetEventHandler<IPointerEnterHandler> (root);

			if (enterHandler == null)
			{
				return null;
			}

			// Only allow EventTriggers with valid PointerEnter entries
			EventTrigger eventTrigger = enterHandler.GetComponent<EventTrigger>();
			if (eventTrigger != null)
			{
				foreach (EventTrigger.Entry entry in eventTrigger.triggers)
				{
					if (entry.eventID == EventTriggerType.PointerEnter && entry.callback != null)
					{
						return enterHandler;
					}
				}

				return null;
			}

			return enterHandler;
		}

		public GameObject GetValidSelectHandler(GameObject root)
		{
			GameObject selectHandler = ExecuteEvents.GetEventHandler<ISelectHandler> (root);

			if (selectHandler == null)
			{
				return null;
			}

			// Only allow EventTriggers with valid Select entries
			EventTrigger eventTrigger = selectHandler.GetComponent<EventTrigger>();
			if (eventTrigger != null)
			{
				foreach (EventTrigger.Entry entry in eventTrigger.triggers)
				{
					if (entry.eventID == EventTriggerType.Select && entry.callback != null)
					{
						return selectHandler;
					}
				}

				return null;
			}

			return selectHandler;
		}

		public GameObject GetValidClickHandler(GameObject root)
		{
			GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler> (root);

			if (clickHandler == null)
			{
				return null;
			}

			// Only allow EventTriggers with valid PointerClick entries
			EventTrigger eventTrigger = clickHandler.GetComponent<EventTrigger>();
			if (eventTrigger != null)
			{
				foreach (EventTrigger.Entry entry in eventTrigger.triggers)
				{
					if (entry.eventID == EventTriggerType.PointerClick && entry.callback != null)
					{
						return clickHandler;
					}
				}

				return null;
			}

			// Only allow interactable Buttons
			Button button = clickHandler.GetComponent<Button> ();
			if (button != null)
			{
				if (button.IsInteractable ())
				{
					return clickHandler;
				}
				else
				{
					return null;
				}
			}

			return clickHandler;
		}

		public void ProcessClick(GameObject target)
		{
			ExecuteEvents.ExecuteHierarchy (target, pointerEventData, ExecuteEvents.pointerClickHandler);

			justClicked = true;
			justClickedHandler = target;
			//nextClickHandler = null;
		}

		public void UpdateJustClicked(GameObject clickHandler)
		{
			if (justClicked)
			{
				if (clickHandler == null || justClickedHandler != clickHandler)
				{
					justClicked = false;
				}
			}

			// Use the following if you need fallthrough protection
			// Note that this can cause problems if buttons are close together
			/*
			if (justClicked)
			{
				if (clickHandler == null)
				{
					justClicked = false;
				}
				else if (nextClickHandler == null)
				{
					if (justClickedHandler != clickHandler)
					{
						// Prevent fallthrough clicks
						nextClickHandler = clickHandler;
					}
				}
				else if (nextClickHandler != clickHandler)
				{
					justClicked = false;
				}
			}
			*/
		}
	}
}
