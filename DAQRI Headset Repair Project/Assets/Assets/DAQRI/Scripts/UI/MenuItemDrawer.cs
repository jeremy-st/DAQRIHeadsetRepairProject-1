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
 *     File Purpose:        Used by MenuItem UI Prefab.                                                                                 *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemDrawer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	/// <summary>
	/// Triggers the pointer enter animation.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		Animator animator = GetComponent<Animator>();
		if (animator)
		{
			animator.SetBool("entered", true);
		}
	}

	/// <summary>
	/// Triggers the pointer exit animation.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit(PointerEventData eventData)
	{
		Animator animator = GetComponent<Animator>();
		if (animator)
		{
			animator.SetBool("entered", false);
		}
	}

}
