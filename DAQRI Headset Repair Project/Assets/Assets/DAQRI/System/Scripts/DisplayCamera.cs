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
 *     File Purpose:        Camera added at runtime.                                                                                    *
 *                                                                                                                                      *
 *     Guide:                                                                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

namespace DAQRI {

    public sealed partial class DisplayCamera : MonoBehaviour {

        private bool stereo;
        private bool clear;
        private bool splitStereoVR;
        private DisplayManager displayManager;
        private ServiceManager serviceManager;
        private IDSHUnityPlugin dshUnityPlugin;
        private Camera rightCamera;

        public Camera Camera { get; private set; }

		/// <summary>
		/// Returns the size needed for an image to fill the screen.
		/// </summary>
		/// <returns>The image size needed to fill the screen.</returns>
		/// <param name="imageAspect">Image aspect ratio.</param>
		/// <param name="screenWidth">Screen width.</param>
		/// <param name="screenHeight">Screen height.</param>
        public static Vector2 AspectFillScreenSize(float imageAspect, int screenWidth, int screenHeight) {
            float screenAspect = (float)screenWidth / (float)screenHeight;

            return imageAspect > screenAspect
                       ? new Vector2(screenHeight * imageAspect, screenHeight)
                       : new Vector2(screenWidth, screenWidth / imageAspect);
        }

		/// <summary>
		/// Returns the size needed for an image to fit the screen.
		/// </summary>
		/// <returns>The image size needed to fit the screen.</returns>
		/// <param name="imageAspect">Image aspect ratio.</param>
		/// <param name="screenWidth">Screen width.</param>
		/// <param name="screenHeight">Screen height.</param>
        public static Vector2 AspectFitScreenSize(float imageAspect, int screenWidth, int screenHeight) {
            float screenAspect = (float)screenWidth / (float)screenHeight;
            return imageAspect > screenAspect
                       ? new Vector2(screenWidth, screenWidth / imageAspect)
                       : new Vector2(screenHeight * imageAspect, screenHeight);
        }

        private Camera RightCamera {
            get {
                if (rightCamera != null) {
                    return rightCamera;
                }

                rightCamera = new GameObject("Right Camera").AddComponent<Camera>();
                rightCamera.transform.SetParent(transform.parent, false);

                return rightCamera;
            }
        }

		/// <summary>
		/// Initializes the camera.
		/// </summary>
		/// <param name="startStereo">If set to <c>true</c> start as a stereo camera.</param>
		/// <param name="startClear">If set to <c>true</c> set clear flags to color.</param>
		/// <param name="useSplitStereoVR">If set to <c>true</c> use split stereo VR.</param>
        public void Initialize(bool startStereo, bool startClear, bool useSplitStereoVR) {
            stereo = startStereo;
            clear = startClear;
            splitStereoVR = useSplitStereoVR;
            InitializeGizmos();

            Camera.MakeMain();
            Camera.gameObject.AddMissingComponent<PhysicsRaycaster>();
            // We are setting the stereoscopic values within the projection
            // matrices themselves so we do not want to use Unity's defaults
            // until we are taking account for this from Gemini when it 
            // sends the projection matrices.
            Camera.stereoConvergence = 0f;
            Camera.stereoSeparation = 0f;

            Resize();

            serviceManager.VisionParametersAvailable += Resize;
            displayManager.ScreenSizeChanged += Resize;
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();

            serviceManager = ServiceManager.Instance;
            displayManager = DisplayManager.Instance;
            dshUnityPlugin = DSHUnityPlugin.Instance;
        }

        private void OnPreCull() {
            if (!Stereo) {
                return;
            }

            if (!splitStereoVR) {
                return;
            }

            Camera.SetStereoViewMatrix(
                Camera.StereoscopicEye.Left,
                dshUnityPlugin.GetViewMatrix(ProjectionMatrixEye.Left) *
                transform.worldToLocalMatrix);
            Camera.SetStereoViewMatrix(
                Camera.StereoscopicEye.Right,
                dshUnityPlugin.GetViewMatrix(ProjectionMatrixEye.Right) *
                transform.worldToLocalMatrix);
        }

        private void Resize() {
            if (Clear) {
                Camera.clearFlags = CameraClearFlags.Color;
                Camera.backgroundColor = Color.black;
            } else {
                Camera.clearFlags = CameraClearFlags.Skybox;
            }

            Camera.stereoTargetEye = Stereo ? StereoTargetEyeMask.Both : StereoTargetEyeMask.None;
            Camera.nearClipPlane = ServiceManager.Instance.GetNearClipPlane();
            Camera.farClipPlane = ServiceManager.Instance.GetFarClipPlane();
            Camera.rect = new Rect(Vector2.zero, Vector2.one);

            if (Stereo) {
                if (!splitStereoVR) {
                    ResizeDualCamera();
                    return;
                }

                ResizeSplitScreenCamera();
                return;
            }

            transform.localRotation = dshUnityPlugin.GetTransformRotation(Stereo);
            transform.localPosition = dshUnityPlugin.GetTransformPosition(Stereo);

            Matrix4x4 monoMatrix = dshUnityPlugin.GetProjectionMatrix(ProjectionMatrixEye.Mono);
            Vector2 imageSize = AspectFillScreenSize(monoMatrix.ProjectionAspectRatio(), displayManager.ScreenWidth, displayManager.ScreenHeight);
            Vector3 projectionScale = 
                new Vector3(
                    imageSize.x / displayManager.ScreenWidth,
                    imageSize.y / displayManager.ScreenHeight,
                    1.0f);

            Camera.projectionMatrix = 
                monoMatrix *
                Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                    projectionScale);
            Camera.fieldOfView = Camera.projectionMatrix.ProjectionFieldOfView();
            Camera.aspect = Camera.projectionMatrix.ProjectionAspectRatio();
        }

        private void OnDestroy() {
            if (serviceManager != null) {
                serviceManager.VisionParametersAvailable -= Resize;
            }
            if (displayManager != null) {
                displayManager.ScreenSizeChanged -= Resize;
            }

            serviceManager = null;
            displayManager = null;
            dshUnityPlugin = null;
        }

        private void ResizeSplitScreenCamera() {
            transform.localRotation = dshUnityPlugin.GetTransformRotation(Stereo);
            transform.localPosition = dshUnityPlugin.GetTransformPosition(Stereo);

            Camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left,
                dshUnityPlugin.GetProjectionMatrix(ProjectionMatrixEye.Left));
            Camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right,
                dshUnityPlugin.GetProjectionMatrix(ProjectionMatrixEye.Right));
        }

        private void ResizeDualCamera() {
            if (splitStereoVR) {
                if (rightCamera != null) {
                    rightCamera.gameObject.SetActive(false);
                }
                return;
            }

            Camera.stereoTargetEye = StereoTargetEyeMask.None;
            RightCamera.stereoTargetEye = StereoTargetEyeMask.None;
            Camera.ResetStereoProjectionMatrices();
            Camera.ResetStereoViewMatrices();
            Camera.projectionMatrix =
                dshUnityPlugin.GetProjectionMatrix(ProjectionMatrixEye.Left);
            RightCamera.projectionMatrix =
                dshUnityPlugin.GetProjectionMatrix(ProjectionMatrixEye.Right);

            RightCamera.nearClipPlane = ServiceManager.Instance.GetNearClipPlane();
            RightCamera.farClipPlane = ServiceManager.Instance.GetFarClipPlane();
            Camera.rect = new Rect(new Vector2(0f, 0f), new Vector2(0.5f, 1f));
            RightCamera.rect = new Rect(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));

            RightCamera.clearFlags = Camera.clearFlags;
            RightCamera.backgroundColor = Camera.backgroundColor;

            transform.localRotation =
                dshUnityPlugin.GetViewRotation(ProjectionMatrixEye.Left) *
                dshUnityPlugin.GetTransformRotation(Stereo);
            transform.localPosition =
                dshUnityPlugin.GetViewPosition(ProjectionMatrixEye.Left) +
                dshUnityPlugin.GetTransformPosition(Stereo);

            RightCamera.gameObject.SetActive(true);
            RightCamera.transform.localRotation =
                dshUnityPlugin.GetViewRotation(ProjectionMatrixEye.Right) *
                dshUnityPlugin.GetTransformRotation(Stereo);
            RightCamera.transform.localPosition =
                dshUnityPlugin.GetViewPosition(ProjectionMatrixEye.Right) +
                dshUnityPlugin.GetTransformPosition(Stereo);
        }
    }
}
