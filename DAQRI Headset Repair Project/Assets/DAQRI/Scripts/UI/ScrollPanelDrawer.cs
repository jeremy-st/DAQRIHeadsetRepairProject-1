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
 *     File Purpose:        Used by ScrollPanel UI Prefab.                                                                              *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPanelDrawer : MonoBehaviour, IPointerClickHandler
{

	public Sprite minusIcon;
	public Sprite plusIcon;
	public Animator scrollPanel;
	private bool isMinimized;


	public void OnPointerClick(PointerEventData eventData)
	{
		if (scrollPanel)
		{
			scrollPanel.SetBool("minimize", isMinimized);
			isMinimized = !isMinimized;
		}

		if (isMinimized)
		{
			Image image = GetComponent<Image>();
			image.sprite = minusIcon;
		}
		else
		{
			Image image = GetComponent<Image>();
			image.sprite = plusIcon;
		}
	}

}
