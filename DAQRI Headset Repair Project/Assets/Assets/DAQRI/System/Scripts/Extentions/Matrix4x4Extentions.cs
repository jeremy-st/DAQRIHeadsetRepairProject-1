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
 *     File Purpose:        Adds functionality to the UnityEngine.Matrix4x4.                                                            *
 *                                                                                                                                      *
 *     Guide:                                                                                                                           *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System;
using UnityEngine;

namespace DAQRI {

    public static class Matrix4x4Extentions {

        public static float ProjectionFieldOfView(this Matrix4x4 projection) {
            float t = projection.m11;
            return Mathf.Atan (1f / t) * 2f * Mathf.Rad2Deg;
        }

        public static float ProjectionAspectRatio(this Matrix4x4 projection) {
            return projection.m11 / projection.m00;
        }

        public static float ProjectionNearClip(this Matrix4x4 projection) {
            float k = (projection.m22 - 1f) / (projection.m22 + 1f);
            return projection.m23 * (1f - k) / (2f * k);
        }

        public static float ProjectionFarClip(this Matrix4x4 projection) {
            float k = (projection.m22 - 1f) / (projection.m22 + 1f);
            float clipMin = projection.m23 * (1f - k) / (2f * k);
            return k * clipMin;
        }

        public static Quaternion TSRRotation(this Matrix4x4 m) {
            // Trap the case where the matrix passed in has an invalid rotation submatrix.
            if (m.GetColumn(2) == Vector4.zero) {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        public static Vector3 TSRPosition(this Matrix4x4 m) {
            return m.GetColumn(3);
        }

        /// <summary>
        /// Fills the matrix with float values.
        /// The values array MUST have a length of 16.
        /// </summary>
        /// <exception cref="ArgumentException">Expected 16 elements in values array</exception>
        public static Matrix4x4 FillWithFloats(this Matrix4x4 matrix, float[] values) {
            if (values == null || values.Length < 16) {
                throw new ArgumentException("Expected 16 elements in values array", "values");
            }

            for (int i = 0; i < 16; i++) {
                matrix[i] = values[i];
            }

            return matrix;
        }
    }
}