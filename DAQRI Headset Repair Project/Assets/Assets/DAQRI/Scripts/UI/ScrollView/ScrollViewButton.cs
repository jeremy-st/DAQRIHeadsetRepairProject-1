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
 *     File Purpose:        Defines a button that scrolls the content of a scroll view.                                                 *
 *                                                                                                                                      *
 *     Guide:               Assign a scroll view and a scroll direction.                                                                *
 *                          Hover over the button with the reticle to trigger scrolling.                                                *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DAQRI {

    [RequireComponent (typeof (Selectable))]
    public class ScrollViewButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public enum ScrollDirection {
            Up,
            Down
        };

        public float speed = 4.0f;
        public ScrollDirection scrollDirection = ScrollDirection.Up;
        public ScrollView scrollView;

        private Selectable selectable;
        private bool isScrolling = false;

        void Start () {
            selectable = GetComponent<Selectable> ();
        }

        void Update () {
            
            if (isScrolling) {
                
                switch (scrollDirection) {
                case (ScrollDirection.Up):
                    ScrollUp ();
                    break;

                case (ScrollDirection.Down):
                    ScrollDown ();
                    break;
                }
            }

            if (!scrollView.HasScrollableContent ()) {
                selectable.interactable = false;

            } else if (scrollDirection == ScrollDirection.Up) {
                selectable.interactable = scrollView.CanScrollUp ();

            } else {
                selectable.interactable = scrollView.CanScrollDown ();
            }
        }

        private void ScrollUp () {
            scrollView.ContentOffset -= speed;
        }

        private void ScrollDown () {
            scrollView.ContentOffset += speed;
        }


        #region Pointer Handlers

        public void OnPointerEnter(PointerEventData eventData) {
            if (selectable.IsInteractable ()) {
                isScrolling = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            isScrolling = false;
        }

        #endregion
    }
}
