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
 *     File Purpose:        Used by DropDown UI Prefab.                                                                                 *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownAppear : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	MenuItemDrawer[] menuItems;

	void Start()
	{
		menuItems = GetComponentsInChildren<MenuItemDrawer>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		foreach (MenuItemDrawer item in menuItems)
		{
			Animator animator = item.GetComponent<Animator>();
			if (animator)
			{
				animator.SetBool("entered", true);
			}
		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		foreach (MenuItemDrawer item in menuItems)
		{
			Animator animator = item.GetComponent<Animator>();
			if (animator)
			{
				animator.SetBool("entered", false);
			}
		}
	}

}
