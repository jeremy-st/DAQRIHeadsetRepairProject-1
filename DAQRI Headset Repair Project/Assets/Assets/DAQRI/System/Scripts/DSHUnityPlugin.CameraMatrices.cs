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
 *     File Purpose:        Access to either native or editor-runtime values for the matrices used by the cameras.                      *
 *                                                                                                                                      *
 *     Guide:                                                                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System;
using UnityEngine;

namespace DAQRI {

    public partial class DSHUnityPlugin {

        private const bool DaqriSmartDevice =
#if DAQRI_SMART_HELMET
            true;
#else
            false;
#endif

        private const int TotalProjectionMatrices = (int)ProjectionMatrixEye.Mono + 1;

        private const string DllNotFoundErrorFormat =
            "{0} failed, Please install the correct libraries, its not recommanded to use the Extension without proper libraries installed.\n{1}";

        private const string EntryPointNotFoundErrorFormat =
            "{0} failed, Signature missing in the library, please update the library.\n{1}";

        private const string GeneralErrorFormat = "{0} failed.\n{1}";

        private static readonly Action ActionNone = () => {};
        private static Action retrievedProjectionMatrices = GetProjectionMatrices;
        private static Action retrievedViewMatrices = GetViewMatrices;
        private static Action retrievedViewsAndTransforms = GetViewsAndTransforms;
        private static Matrix4x4[] projectionMatrices = new Matrix4x4[TotalProjectionMatrices];
        private static Matrix4x4[] viewMatrices = new Matrix4x4[TotalProjectionMatrices];
        private static Vector3[] viewPositions = new Vector3[TotalProjectionMatrices];
        private static Vector3[] transformPositions = new Vector3[2];
        private static Quaternion[] transformRotations = new Quaternion[2];
        private static Quaternion[] viewRotations = new Quaternion[TotalProjectionMatrices];

        private static void GetProjectionMatrices() {
            retrievedProjectionMatrices = ActionNone;

			switch (Instance.runEnvironment.CurrentEnvironment ()) {
			case (RunEnvironmentType.OnDevice):
				const string MethodName = "Get Projection Matrices";
				try {
					DSHUnityAbstraction.L7_GetDisplayProjectionMatrices(
						ServiceManager.Instance.GetNearClipPlane(),
						ServiceManager.Instance.GetFarClipPlane(),
						out projectionMatrices[(int)ProjectionMatrixEye.Left],
						out projectionMatrices[(int)ProjectionMatrixEye.Right]);
					VisionUnityAbstraction.L7_TrackerGetProjectionMatrix(out projectionMatrices[(int)ProjectionMatrixEye.Mono]);
					return;
				}
				catch (DllNotFoundException e) {
					Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
					Instance.bCorrectDLLloaded = false;
				}
				catch (EntryPointNotFoundException e) {
					Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
					Instance.bCorrectDLLloaded = false;
				}
				catch (Exception e) {
					Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
				}
				break;

			default:
				break;
			}

            // These values where what was being returned at the time of this code was written from a running thor.
            // They are used as defaults or editor builds.
            projectionMatrices[(int)ProjectionMatrixEye.Right] = new Matrix4x4 {
                m00 = 3.07199f,
                m01 = 0f,
                m02 = 0f,
                m03 = 0f,
                m10 = 0f,
                m11 = 5.46199f,
                m12 = 0f,
                m13 = 0f,
                m20 = 0f,
                m21 = 0f,
                m22 = -1.00004f,
                m23 = -0.04f,
                m30 = 0f,
                m31 = 0f,
                m32 = -1f,
                m33 = 0
            };

            projectionMatrices[(int)ProjectionMatrixEye.Left] = new Matrix4x4 {
                m00 = 3.07199f,
                m01 = 0f,
                m02 = 0f,
                m03 = 0f,
                m10 = 0f,
                m11 = 5.46199f,
                m12 = 0f,
                m13 = 0f,
                m20 = 0f,
                m21 = 0f,
                m22 = -1.00004f,
                m23 = -0.04f,
                m30 = 0f,
                m31 = 0f,
                m32 = -1f,
                m33 = 0
            };

            projectionMatrices[(int)ProjectionMatrixEye.Mono] =
                Matrix4x4.Perspective(
                    ServiceManager.Instance.GetVisionFieldOfView(),
                    ServiceManager.Instance.GetVisionAspectRatio(),
                    ServiceManager.Instance.GetNearClipPlane(),
                    ServiceManager.Instance.GetFarClipPlane());
        }

        private static void GetViewMatrices() {
            retrievedViewMatrices = ActionNone;

            for (ProjectionMatrixEye eye = 0; eye <= ProjectionMatrixEye.Mono; ++eye) {
                viewMatrices[(int)eye] = GetViewMatrix(Instance.GetViewPosition(eye), Instance.GetViewRotation(eye));
            }
        }

        private static Matrix4x4 GetViewMatrix(Vector3 offset, Quaternion rotation) {
            // The world to camera matrix should be inversed on the position's x and y
            // as well as it's scale fliped on the z.
            // https://docs.unity3d.com/ScriptReference/Camera-worldToCameraMatrix.html
            return Matrix4x4.TRS(
                new Vector3(-offset.x, -offset.y, offset.z),
                rotation,
                new Vector3(1f, 1f, -1f));
        }

        private static void GetViewsAndTransforms() {
            retrievedViewsAndTransforms = ActionNone;

            GetDisplayPoses(
                out viewPositions[(int)ProjectionMatrixEye.Left],
                out viewRotations[(int)ProjectionMatrixEye.Left],
                out viewPositions[(int)ProjectionMatrixEye.Right],
                out viewRotations[(int)ProjectionMatrixEye.Right]);

            transformPositions[0] = viewPositions[(int)ProjectionMatrixEye.Mono] = Vector3.zero;
            transformRotations[0] = viewRotations[(int)ProjectionMatrixEye.Mono] = Quaternion.identity;

            transformPositions[1] =
                Vector3.Lerp(
                    viewPositions[(int)ProjectionMatrixEye.Left],
                    viewPositions[(int)ProjectionMatrixEye.Right],
                    0.5f);
            transformRotations[1] =
                Quaternion.Lerp(
                    viewRotations[(int)ProjectionMatrixEye.Left],
                    viewRotations[(int)ProjectionMatrixEye.Right],
                    0.5f);

            viewPositions[(int)ProjectionMatrixEye.Left] -= transformPositions[1];
            viewPositions[(int)ProjectionMatrixEye.Right] -= transformPositions[1];

            viewRotations[(int)ProjectionMatrixEye.Left] *= Quaternion.Inverse(transformRotations[1]);
            viewRotations[(int)ProjectionMatrixEye.Right] *= Quaternion.Inverse(transformRotations[1]);
        }

        private static void GetDisplayPoses(out Vector3 posLeft, out Quaternion rotLeft,
                                            out Vector3 posRight, out Quaternion rotRight) {

			if (!Application.isEditor || DaqriSmartDevice) {
                const string MethodName = "Get Display Poses";
                try {
                    DSHUnityAbstraction.L7_GetDisplayPoses(out posLeft, out rotLeft, out posRight, out rotRight);
                    return;
                } catch (DllNotFoundException e) {
                    Debug.LogWarningFormat(DllNotFoundErrorFormat, MethodName, e);
                    Instance.bCorrectDLLloaded = false;
                }
                catch (EntryPointNotFoundException e) {
                    Debug.LogWarningFormat(EntryPointNotFoundErrorFormat, MethodName, e);
                    Instance.bCorrectDLLloaded = false;
                }
                catch (Exception e) {
                    Debug.LogWarningFormat(GeneralErrorFormat, MethodName, e);
                }
            }

            // These values where what was being returned at the time of this code was written from a running thor.
            // They are used as defaults or editor builds.
            posLeft = new Vector3(
                -0.015389f,
                -0.03775f,
                -0.067457f);
            posRight = new Vector3(
                0.046458f,
                -0.039182f,
                -0.066511f);

            rotLeft = new Quaternion(0f, 0f, 0f, -1f);
            rotRight = new Quaternion(0f, 0f, 0f, -1f);
        }
    }
}
