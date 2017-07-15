// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaceDisplay.cs" company="Daqri LLC">
//  Copyright © 2016 DAQRI. DAQRI is a registered trademark of DAQRI LLC. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CES.WorldUI
{
    using DAQRI;
    using UnityEngine;
    public class FaceDisplay : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = 2f;
        private Vector3 startingScale;
        private float startingDistance;

        private void Start()
        {
            startingScale = transform.localScale;
            startingDistance = Vector3.Distance(DisplayManager.Instance.transform.position,transform.position);
        }

        private void Update()
        {
            if (DisplayManager.Instance == null)
            {
                return;
            }

            UpdateRotation();

            UpdateScale();
        }

        private void UpdateRotation()
        {
            Vector3 targetPos = DisplayManager.Instance.transform.position;
            Vector3 currentPos = transform.position;
            currentPos.y = targetPos.y = 0f;
            Vector3 newForward = currentPos - targetPos;
            transform.forward = Vector3.Lerp(transform.forward, newForward.normalized, Time.deltaTime * rotationSpeed);
        }

        private void UpdateScale()
        {
            float currentDistance = Vector3.Distance(DisplayManager.Instance.transform.position, transform.position) - startingDistance;
            Vector3 newScale = startingScale * currentDistance;
            transform.localScale = new Vector3(Mathf.Clamp(newScale.x, 1,currentDistance), Mathf.Clamp(newScale.y, 1,currentDistance), Mathf.Clamp(newScale.z,1, currentDistance));
        }
    }
}