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
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    internal static class ProcessProgressWindow {
        private class ProgressData {
            private const string DateText = "([A-z]+ [A-z]+ [0-9]+ [0-9]+:[0-9]+:[0-9]+ [0-9]+)";
            private const float ApproxMaxLengthOfOutput = 84f;
            internal readonly string title;
            private readonly string filePath;
            private readonly List<string> lines = new List<string>();

            private FileStream fileStream;
            private StreamReader streamReader;
            internal string Info { get; private set; }
            internal float Progress { get; private set; }

            internal ProgressData(string title, string filePath) {
                this.title = title;
                this.filePath = filePath;
                Info = string.Empty;
            }

            internal void Update() {
                if (fileStream == null) {
                    if (!File.Exists(filePath)) {
                        Debug.LogWarning("null");
                        return;
                    }

                    fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    if (fileStream == null) {
                        return;
                    }

                    streamReader = new StreamReader(fileStream);
                }

                while (streamReader.EndOfStream == false) {
                    string nextLine = streamReader.ReadLine();
                    AddLine(nextLine);
                }

                if (lines.Count > 0) {
                    Info = lines[lines.Count - 1];
                }
            }

            internal void Clear() {
                if (fileStream != null) {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (streamReader != null) {
                    streamReader.Dispose();
                    streamReader = null;
                }
            }

            private void AddLine(string nextLine) {
                if (nextLine == null) {
                    return;
                }

                if (Regex.Match(nextLine, DateText).Success) {
                    nextLine = Regex.Replace(nextLine, DateText, string.Empty, RegexOptions.None);
                }

                nextLine = nextLine.Trim(' ', '-');

                if (nextLine.Length <= 1) {
                    return;
                }
                lines.Add(nextLine);
                Progress = Mathf.Min(1f, lines.Count / ApproxMaxLengthOfOutput);
            }

            ~ProgressData() {
                Clear();
            }
        }

        private static ProgressData progressData;

        internal static void ShowProgressBar(string progressBarTitle, string filePath) {
            progressData = new ProgressData(progressBarTitle, filePath);
            EditorApplication.LockReloadAssemblies();
            EditorApplication.update += Update;
        }

        private static void Update() {
            if ((progressData == null) || !SmartDeviceProcess.Installing) {
                Close();
                return;
            }

            progressData.Update();

            EditorUtility.DisplayProgressBar(
                progressData.title,
                progressData.Info,
                progressData.Progress);
        }

        private static void Close() {
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= Update;
            EditorApplication.UnlockReloadAssemblies();
            EditorUtility.ClearProgressBar();
            if (progressData == null) {
                return;
            }

            progressData.Clear();
            progressData = null;
        }
    }
}
