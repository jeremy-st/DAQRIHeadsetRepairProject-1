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
 *     File Purpose:       Reads Smart Device Settings from Menu/DAQRI/Smart Device Settings and automatically deploys into the         *
 *                         target device as part of the postprocess build.                                                              *                                                                             *
 *     Guide:              <todo>                                                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
namespace DAQRI.BuildToSmartHelmet {
    using System;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    public sealed class SmartDevicePreferencesWindow : EditorWindow {
        private static Vector2 scrollPos;
        private static string newIpAddress;

        private static GUIStyle wordWrapped;

        internal static void InvokeDaqriBridgeCommand(DaqriBridge daqriBridgeType) {
            if (daqriBridgeType is DaqriBridgeInstall) {
                SmartDevicePreferencesWindow window = GetWindow<SmartDevicePreferencesWindow>();
                if (window != null) {
                    window.Close();
                }
            }

            if (!IsValidIP(SmartDevicePreferences.IpAddress)) {
                return;
            }

            SmartDeviceProcess.RunProcess(daqriBridgeType);
        }

        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string path) {
            if ((target != BuildTarget.StandaloneLinux64) && (target != BuildTarget.StandaloneLinux) &&
                (target != BuildTarget.StandaloneLinuxUniversal)) {
                return;
            }

            int indexEnd = path.IndexOf(".x86_64", StringComparison.OrdinalIgnoreCase);
            if (indexEnd == -1) {
                Debug.LogError("Invalid file extentions");
                return;
            }

			// Product name and executable name can be different, need to find the last 
			// instance of the forward slash instead
			string folderPath = path.Substring(0, path.LastIndexOf('/') + 1);

			// write a file to the extra directory with copmany name?


            InvokeDaqriBridgeCommand(
                new DaqriBridgeInstall(
                    SmartDevicePreferences.IpAddress,
                    PlayerSettings.productName,
                    path,
					PlayerSettings.companyName));

            ProcessProgressWindow.ShowProgressBar(
                string.Format("Installing {0} to {1}", PlayerSettings.productName, SmartDevicePreferences.IpAddress),
				string.Format("{0}daqri_bridge_log.txt", folderPath));
        }

        [MenuItem("DAQRI/Smart Device Settings")]
        private static void OpenSmartDeviceSettings() {
            SmartDevicePreferencesWindow window = GetWindow<SmartDevicePreferencesWindow>();
            window.name = "Settings";
            window.Show();
        }

        private static bool IsValidIP(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                return false;
            }

            string[] quads = ipAddress.Split('.');

            if (quads.Length != 4) {
                return false;
            }

            foreach (string quad in quads) {
                int q;
                if (!int.TryParse(quad, out q)
                    || !q.ToString().Length.Equals(quad.Length)
                    || (q < 0)
                    || (q > 255)) {
                    return false;
                }
            }

            return true;
        }

        private static void Initialize() {
            if (wordWrapped == null) {
                wordWrapped = new GUIStyle(EditorStyles.label) {wordWrap = true};
            }
            if (newIpAddress != null) {
                return;
            }

            newIpAddress = SmartDevicePreferences.IpAddress;
            SmartDeviceProcess.ClearInstalledApplications();
            InvokeDaqriBridgeCommand(new DaqriBridgeList(SmartDevicePreferences.IpAddress));
        }

        private static void DrawDeviceIPSelector() {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField("Enter Device IP: ", GUILayout.Width(150));

                Color guiColor = GUI.color;
                if (!IsValidIP(newIpAddress)) {
                    GUI.color = Color.red;
                }
                newIpAddress = EditorGUILayout.TextField(newIpAddress);
                GUI.color = guiColor;
            }
        }

        private static void DrawHeader() {
            GUILayout.Label("Connecting to your Smart Device", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope()) {
                const string Message =
                    "App Icon will be picked up from the Build Settings.\nApp Name is the product name you provide in the Project Settings.\n";
                EditorGUILayout.LabelField(Message, wordWrapped);
                EditorGUILayout.Separator();
            }
        }

        private void OnGUI() {
            Initialize();

            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                DrawHeader();
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                DrawDeviceIPSelector();
            }

            DrawApplicationList();
        }

        private void Update() {
            if (SmartDeviceProcess.Dirty) {
                Repaint();
            }

            if (SmartDeviceProcess.Removed) {
                SmartDeviceProcess.Removed = false;
                SmartDeviceProcess.ClearInstalledApplications();
                InvokeDaqriBridgeCommand(new DaqriBridgeList(SmartDevicePreferences.IpAddress));
            }
        }

        private void DrawApplicationList() {
            if (GUILayout.Button(!string.Equals(SmartDevicePreferences.IpAddress, newIpAddress) ? "Save" : "Refresh")) {
                SmartDevicePreferences.IpAddress = newIpAddress;
                SmartDeviceProcess.ClearInstalledApplications();
                InvokeDaqriBridgeCommand(new DaqriBridgeList(SmartDevicePreferences.IpAddress));
            }

            EditorGUILayout.Separator();

            using (new EditorGUILayout.VerticalScope()) {
                if (IsValidIP(SmartDevicePreferences.IpAddress)) {
                    EditorGUILayout.LabelField(string.Format("Applications on {0}", SmartDevicePreferences.IpAddress),
                        EditorStyles.boldLabel);
                    using (
                        EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(
                            scrollPos,
                            GUILayout.Width(position.width),
                            GUILayout.Height(position.height * 0.5f))) {
                        scrollPos = scrollViewScope.scrollPosition;
                        SmartDeviceProcess.DisplayInstalledApplications();
                    }
                }
                else {
                    EditorGUILayout.LabelField("Enter a valid IP!", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }
    }
}
