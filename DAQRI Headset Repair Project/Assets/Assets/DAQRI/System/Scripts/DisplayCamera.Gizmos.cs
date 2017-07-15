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
 *     File Purpose:        Camera gizmos and settings for editor only.                                                                 *
 *                                                                                                                                      *
 *     Guide:                                                                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DAQRI {

    public sealed partial class DisplayCamera {
#if !UNITY_EDITOR

        private bool Stereo {
            get { return stereo; }
        }

        private bool Clear {
            get { return clear; }
        }

        private void InitializeGizmos() {}

#else //!UNITY_EDITOR

        private const string StereoInEditorPrefKey = "DAQRI.DisplayCamera.StereoInEditor";
        private const string ClearInEditorPrefKey = "DAQRI.DisplayCamera.ClearInEditor";

        [SerializeField] private bool stereoInEditor;
        [SerializeField] private bool clearInEditor;

        private bool previousStereoInEditor;
        private bool previousClearInEditor;

        private bool Stereo {
            get { return stereo || stereoInEditor; }
        }

        private bool Clear {
            get { return clear || clearInEditor; }
        }

        private static bool StereoInEditorPreference {
            get { return EditorPrefs.GetBool(StereoInEditorPrefKey, false); }
            set { EditorPrefs.SetBool(StereoInEditorPrefKey, value); }
        }

        private static bool ClearInEditorPreference {
            get { return EditorPrefs.GetBool(ClearInEditorPrefKey, false); }
            set { EditorPrefs.SetBool(ClearInEditorPrefKey, value); }
        }

        private void InitializeGizmos() {
            previousStereoInEditor = stereoInEditor = StereoInEditorPreference;
            previousClearInEditor = clearInEditor = ClearInEditorPreference;
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void OnDrawGizmosSelected(DisplayCamera displayCamera, GizmoType gizmoType) {
            displayCamera.UpdateGizmos();
            displayCamera.DrawFrustums(0.5f);
        }

        [DrawGizmo(GizmoType.NonSelected)]
        private static void OnDrawGizmosNonSelected(DisplayCamera displayCamera, GizmoType gizmoType) {
            displayCamera.UpdateGizmos();
        }

        private void UpdateGizmos() {
            UpdateStereoMode();
            UpdateClearMode();
        }

        private void UpdateStereoMode()
        {
            if (previousStereoInEditor == stereoInEditor)
            {
                return;
            }

            StereoInEditorPreference =
                previousStereoInEditor = stereoInEditor;
            Resize();
        }

        private void UpdateClearMode()
        {
            if (previousClearInEditor == clearInEditor)
            {
                return;
            }

            ClearInEditorPreference =
                previousClearInEditor = clearInEditor;

            Resize();
        }

        private void DrawFrustums(float alpha) {
            if (Stereo && !splitStereoVR) {
                return;
            }

            Color previousColor = Gizmos.color;
            Matrix4x4 previousMatrix = Gizmos.matrix;

            if (Stereo) {
                DrawFrustum(
                    dshUnityPlugin.GetViewPosition(ProjectionMatrixEye.Left),
                    dshUnityPlugin.GetViewRotation(ProjectionMatrixEye.Left),
                    Camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left),
                    new Color(1f, 1f, 0f, alpha));
                DrawFrustum(
                    dshUnityPlugin.GetViewPosition(ProjectionMatrixEye.Right),
                    dshUnityPlugin.GetViewRotation(ProjectionMatrixEye.Right),
                    Camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right),
                    new Color(0f, 1f, 0f, alpha));
            } else {
                DrawFrustum(
                    dshUnityPlugin.GetViewPosition(ProjectionMatrixEye.Mono),
                    dshUnityPlugin.GetViewRotation(ProjectionMatrixEye.Mono),
                    Camera.projectionMatrix,
                    new Color(0f, 1f, 1f, alpha));
            }

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }

        private void DrawFrustum(Vector3 position, Quaternion rotation, Matrix4x4 projection, Color color) {
            Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.TRS(
                                position,
                                rotation,
                                Vector3.one);
            Gizmos.color = color;
            Gizmos.DrawFrustum(
                Vector3.zero,
                projection.ProjectionFieldOfView(),
                projection.ProjectionFarClip(),
                projection.ProjectionNearClip(),
                projection.ProjectionAspectRatio());
        }

#endif //!UNITY_EDITOR
    }
}
