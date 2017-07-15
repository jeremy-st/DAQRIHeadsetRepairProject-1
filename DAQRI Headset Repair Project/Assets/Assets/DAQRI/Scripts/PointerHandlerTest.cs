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
 *     File Purpose:        Used by Reticle Interaction Example.                                                                        *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DAQRI;

public class PointerHandlerTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Material pointerEnteredMaterial;
	public Material pointerExitMaterial;
	public Material pointerClickMaterial;
	public MeshRenderer meshRenderer;

	void Start ()
	{
		// turn off the video background for these tests
		DisplayManager.Instance.TurnVideoBackgroundOff ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		meshRenderer.material = pointerEnteredMaterial;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		meshRenderer.material = pointerExitMaterial;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		meshRenderer.material = pointerClickMaterial;
	}
}
