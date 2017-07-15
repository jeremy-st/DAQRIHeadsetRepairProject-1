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
 *     File Purpose:        Keeps track of the current clickable target the reticle is focused on. Animates the reticle and             *
 *                          calls the public OnDwellComplete action when the dwell time has elapsed.                                    *
 *                                                                                                                                      *
 *     Guide:               Call UpdateDwellTarget each frame with the current IPointerClickHandler target (or null).                   *
 *                          Assign a handler to the OnDwellComplete action. Currently works with ReticleInputModule.                    *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DAQRI {

    public class Reticle : MonoBehaviour {
        public enum State {
            Idle,
            Hover,
            Dwell
        }

        public float dwellAnimationLength = 1.0f;

        public float dwellClickTime = 0.7f;
        [SerializeField] private Image dwellImage;
        [SerializeField] private Image hoverImage;
        [SerializeField] private Image idleImage;
        [SerializeField] private GameObject dwellTarget;
        [SerializeField] private bool triggered;

        private IPointerDwellHandler dwellHandler;

        private IEnumerator currentDwellAnimationCoroutine;
        private Sprite[] dwellSprites;
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private float scaleFactor;
        private Vector3 targetLocalPosition;

		/// <summary>
		/// Event to execute when reticle dwell has completed.
		/// </summary>
        public event Action<GameObject> OnDwellComplete = gameObject => { };

		/// <summary>
		/// Sets the target world position.
		/// </summary>
		/// <value>The target world position.</value>
        public Vector3 TargetWorldPosition {
            set {
                targetLocalPosition = transform.parent.InverseTransformPoint(value);
            }
        }

		/// <summary>
		/// Updates the hover state based on the target.
		/// </summary>
		/// <param name="target">Target.</param>
        public void UpdateHoverTarget(GameObject target) {
            SetState(target == null ? State.Idle : State.Hover);
        }

		/// <summary>
		/// Updates the dwell target and triggers animations & state changes if needed.
		/// </summary>
		/// <param name="target">Target.</param>
        public void UpdateDwellTarget(GameObject target) {
            if (target == null) {
                dwellTarget = null;
                dwellHandler = null;

                if (currentDwellAnimationCoroutine != null) {
                    StopCoroutine(currentDwellAnimationCoroutine);
                    currentDwellAnimationCoroutine = null;
                }

            } else if (dwellTarget != target) {
                triggered = false;
                dwellTarget = target;
                dwellHandler = dwellTarget.GetComponent<IPointerDwellHandler> ();

                if (currentDwellAnimationCoroutine != null) {
                    StopCoroutine(currentDwellAnimationCoroutine);
                }

                currentDwellAnimationCoroutine = AnimateDwell();
                StartCoroutine(currentDwellAnimationCoroutine);
            }
        }

        private void Start() {
            dwellSprites = Resources.LoadAll<Sprite>("Texture/reticle-dwell");

            transform.localPosition =
                targetLocalPosition =
                    originalPosition = new Vector3(
                        transform.localPosition.x,
                        transform.localPosition.y,
                        DisplayManager.DefaultReticlePosition);
            originalScale = transform.localScale;
            scaleFactor = Math.Abs(originalPosition.z) > float.Epsilon ? 1f / originalPosition.z : 0f;
        }

        private void Update() {
            transform.localPosition =
                new Vector3(
                    targetLocalPosition.x,
                    targetLocalPosition.y,
                    Mathf.SmoothStep(transform.localPosition.z, targetLocalPosition.z, 0.25f));

            transform.localScale = originalScale * transform.localPosition.z * scaleFactor;
        }

        private void SetState(State state) {
            switch (state) {
                case State.Idle:
                    idleImage.gameObject.SetActive(true);
                    hoverImage.gameObject.SetActive(false);
                    dwellImage.gameObject.SetActive(false);
                    break;
                case State.Hover:
                    idleImage.gameObject.SetActive(false);
                    hoverImage.gameObject.SetActive(true);
                    dwellImage.gameObject.SetActive(false);
                    break;
                case State.Dwell:
                    idleImage.gameObject.SetActive(false);
                    hoverImage.gameObject.SetActive(false);
                    dwellImage.gameObject.SetActive(true);
                    break;
            }
        }

        private IEnumerator AnimateDwell() {
            float timeElapsed = 0.0f;

            while (timeElapsed < dwellClickTime) {
                SetState(State.Dwell);

                int frame = (int)(timeElapsed / dwellAnimationLength * dwellSprites.Length);
                frame = Mathf.Min(frame, dwellSprites.Length - 1);
                dwellImage.sprite = dwellSprites[frame];

                timeElapsed += Time.deltaTime;

                if (dwellHandler != null) {
                    float fractionFilled = (float)frame / ((float)dwellSprites.Length - 1);
                    fractionFilled = fractionFilled.GetClampedValue (0, 1);
                    dwellHandler.OnPointerDwellProgress (fractionFilled);
                }

                yield return 0;
            }

            if (!triggered && (timeElapsed >= dwellClickTime)) {
                triggered = true;
                OnDwellComplete(dwellTarget);
            }
        }
    }
}
