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
 *     File Purpose:        Custom editor for the scroll view.                                                                          *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DAQRI.Internal {

    [CustomEditor (typeof (ScrollView))]
    public class ScrollViewEditor : Editor {
        
        public SerializedProperty fitHeightToCellsProperty;
        public SerializedProperty controllerProperty;
        public SerializedProperty cellSpacingProperty;
        public SerializedProperty cellPrefabProperty;

        private bool isAdvancedFoldoutOpen = false;

        void OnEnable () {
            fitHeightToCellsProperty = serializedObject.FindProperty ("fitHeightToCells");
            controllerProperty = serializedObject.FindProperty ("controller");
            cellSpacingProperty = serializedObject.FindProperty ("cellSpacing");
            cellPrefabProperty = serializedObject.FindProperty ("cellPrefab");

            if (cellPrefabProperty.objectReferenceValue == null) {
                isAdvancedFoldoutOpen = true;
            }
        }

        public override void OnInspectorGUI () {
            DrawBasicContent ();

            isAdvancedFoldoutOpen = EditorGUILayout.Foldout (isAdvancedFoldoutOpen, "Advanced");

            if (isAdvancedFoldoutOpen) {
                DrawAdvancedContent ();
            }

            serializedObject.ApplyModifiedProperties ();
        }

        private void DrawBasicContent () {
            controllerProperty.objectReferenceValue = EditorGUILayout.ObjectField ("Controller", controllerProperty.objectReferenceValue, typeof (ScrollViewController), true);

            if (controllerProperty.objectReferenceValue != null) {
                EditorGUILayout.HelpBox (
                    "The scroll view creates rows at runtime. " +
                    "Use the controller to provide information about rows and respond to cell click events.", 
                    MessageType.Info
                );
            } else {
                EditorGUILayout.HelpBox (
                    "Please add a controller to provide information about rows and respond to cell click events. " +
                    "The scroll view will use this controller at runtime to populate the view.",
                    MessageType.Error
                );
            }
        }

        private void DrawAdvancedContent () {
            cellSpacingProperty.floatValue = EditorGUILayout.FloatField ("Cell Spacing", cellSpacingProperty.floatValue);

            fitHeightToCellsProperty.boolValue = EditorGUILayout.Toggle ("Trim Height To Cells", fitHeightToCellsProperty.boolValue);

            if (fitHeightToCellsProperty.boolValue) {
                EditorGUILayout.HelpBox (
                    "This will trim the scroll view height at runtime so that a whole number of cells are visible.", 
                    MessageType.Info
                );
            }

            cellPrefabProperty.objectReferenceValue = EditorGUILayout.ObjectField ("Cell prefab", cellPrefabProperty.objectReferenceValue, typeof (GameObject), true);

            if (cellPrefabProperty.objectReferenceValue != null) {
                EditorGUILayout.HelpBox (
                    "This prefab is used to create cells at runtime. " +
                    "To make the cell clickable, add a 'Button' component to it's hierarchy. " +
                    "The scroll view will automatically link the click event with the button. " +
                    "Note that having other buttons on the cell is not supported at this time.", 
                    MessageType.Info
                );

            } else {
                EditorGUILayout.HelpBox (
                    "A cell prefab is needed to generate cells at runtime.",
                    MessageType.Error);
            }
        }
    }
}
