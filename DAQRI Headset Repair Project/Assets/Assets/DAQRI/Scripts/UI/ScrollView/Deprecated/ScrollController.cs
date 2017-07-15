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
 *     File Purpose:        Allows programmatic scrolling of a ScrollRect.                                                              *
 *                                                                                                                                      *
 *     Guide:               Requires a Selectable component. Works best with a UI Image component. Scrolls up or down when pointer      *
 *                          enters the UI component. Automatically changes the interactable status when top or bottom is reached.       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace DAQRI {
	
	[RequireComponent (typeof (Selectable))]
	public class ScrollController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		
	    public enum ScrollDirection
	    {
	        SCROLL_UP,
	        SCROLL_DOWN
	    };

	    public ScrollRect scrollRect;
	    public ScrollDirection scrollDirection = ScrollDirection.SCROLL_UP;
	    public float speed = 4.0f;

	    private bool isScrolling = false;
	    private Selectable selectable;
		
	    void Start ()
	    {
	        selectable = GetComponent<Selectable> ();
	    }

		void Update ()
	    {
	        if (isScrolling)
	        {
	            if (scrollDirection == ScrollDirection.SCROLL_UP)
	            {
	                ScrollUp ();
	            }
	            else
	            {
	                ScrollDown ();
	            }
	        }

	        float contentHeight = scrollRect.content.rect.height;
	        float scrollRectHeight = scrollRect.GetComponent<RectTransform> ().rect.height;

	        if (contentHeight < scrollRectHeight)
	        {
	            selectable.interactable = false;
	        }
	        else if (scrollDirection == ScrollDirection.SCROLL_UP)
	        {
				bool reachedTop = Mathf.Abs (1f - scrollRect.verticalNormalizedPosition) < 1e-4f;
	            selectable.interactable = !reachedTop;
	        }
	        else
	        {
				bool reachedBottom = Mathf.Abs (scrollRect.verticalNormalizedPosition) < 1e-4f;
	            selectable.interactable = !reachedBottom;
	        }
		}

	    public void ScrollUp ()
	    {
	        float contentHeight = scrollRect.content.rect.height;
	        float scrollRectHeight = scrollRect.GetComponent<RectTransform> ().rect.height;
	        float newPosition = scrollRect.verticalNormalizedPosition + speed / (contentHeight - scrollRectHeight);
	        newPosition = Mathf.Min (newPosition, 1);
	        scrollRect.verticalNormalizedPosition = newPosition;
	    }

	    public void ScrollDown ()
	    {
	        float contentHeight = scrollRect.content.rect.height;
	        float scrollRectHeight = scrollRect.GetComponent<RectTransform> ().rect.height;
	        float newPosition = scrollRect.verticalNormalizedPosition - speed / (contentHeight - scrollRectHeight);
	        newPosition = Mathf.Max (newPosition, 0);
	        scrollRect.verticalNormalizedPosition = newPosition;
	    }

	    public void StartScrolling ()
	    {
	        isScrolling = true;
	    }

	    public void StopScrolling ()
	    {
	        isScrolling = false;
	    }

	    public void OnPointerEnter(PointerEventData eventData)
	    {
	        if (selectable.IsInteractable ())
	        {
	            StartScrolling ();
	        }
	    }

	    public void OnPointerExit(PointerEventData eventData)
	    {
	        StopScrolling ();
	    }
	}
}
