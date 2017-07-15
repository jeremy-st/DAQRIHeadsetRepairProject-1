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
 *     File Purpose:       Encapsulates objects such as the SmartDeviceProcess.InstalledApplications and SmartDeviceProcess.ErrorList   *
 *                         so that it can ensure only one thread is accessing these items for they are touched from threads by the      *
 *                         process callback and will result in not releasing the memory and bufferes otherwise.                         *
 *     Guide:              <todo>                                                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
namespace DAQRI.BuildToSmartHelmet {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    internal static class SmartDeviceProcess {
        private const string ProcessNameFormat = "{0}/DAQRI/System/Deploy/" +
#if UNITY_EDITOR_OSX
                                                 "OSX/daqri_bridge";
#elif UNITY_EDITOR_WIN
                                                 "WIN/daqri_bridge.exe";
#else //LINUX
                                                 "LINUX/daqri_bridge";
#endif

        private static readonly List<string> InstalledApplications = new List<string>();
        private static readonly List<string> ErrorList = new List<string>();
        private static bool dirty;
        private static readonly object DirtyMutex = new object();

        private static bool installing;
        private static readonly object InstallingMutex = new object();

        private static bool removed;
        private static readonly object RemovedMutex = new object();

        internal static bool Dirty {
            get {
                lock (DirtyMutex) {
                    return dirty;
                }
            }

            private set {
                lock (DirtyMutex) {
                    dirty = value;
                }
            }
        }

        internal static bool Installing {
            get {
                lock (InstallingMutex) {
                    return installing;
                }
            }

            private set {
                lock (InstallingMutex) {
                    installing = value;
                }
            }
        }

        internal static bool Removed {
            get {
                lock (RemovedMutex) {
                    return removed;
                }
            }

            set {
                lock (RemovedMutex) {
                    removed = value;
                }
            }
        }

        internal static void ClearInstalledApplications() {
            lock (InstalledApplications) {
                lock (DirtyMutex) {
                    InstalledApplications.Clear();
                    Dirty = true;
                }
            }
        }

        private static void ClearBuildErrors() {
            lock (ErrorList) {
                ErrorList.Clear();
            }
        }

        private static void LogAndClearBuildErrors() {
            lock (ErrorList) {
                foreach (string error in ErrorList) {
                    Debug.LogError(error);
                }
                ErrorList.Clear();
            }
        }

        private static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            string newInstalledApplication = outLine.Data.Substring(3);
            if (string.IsNullOrEmpty(newInstalledApplication)) {
                return;
            }

            lock (InstalledApplications) {
                lock (DirtyMutex) {
                    InstalledApplications.Add(newInstalledApplication);
                    Dirty = true;
                }
            }
        }

        private static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            int index = outLine.Data.IndexOf(":", StringComparison.Ordinal);
            string errorMsg = outLine.Data.Substring(index + 1);
            if (string.IsNullOrEmpty(errorMsg)) {
                return;
            }

            lock (ErrorList) {
                ErrorList.Add(string.Format("Deployment to Smart Device failed : {0}", errorMsg));
            }
        }

        private static Thread processThread;
        private static readonly object processThreadMutex = new object();

        internal static void RunProcess(DaqriBridge daqriBridgeType) {
            lock (processThreadMutex) {
                if (processThread != null) {
                    Debug.LogError("Process thread already started.");
                    return;
                }
                ClearBuildErrors();
                processThread = new Thread(Start);
            }

            if (daqriBridgeType is DaqriBridgeInstall) {
                Installing = true;
            }

            processThread.Start(new ProcessArguments(string.Format(ProcessNameFormat, Application.dataPath),
                daqriBridgeType));
        }

        private class ProcessArguments {
            internal readonly DaqriBridge daqriBridge;
            internal readonly string fileName;

            public ProcessArguments(string fileName, DaqriBridge daqriBridge) {
                this.fileName = fileName;
                this.daqriBridge = daqriBridge;
            }
        }

        private static void Start(object o) {
            ProcessArguments arguments = (ProcessArguments)o;
            Process process = new Process {
                StartInfo = {
                    FileName = arguments.fileName,
                    Arguments = arguments.daqriBridge.arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    // Redirects the standard input so that commands can be sent to the shell.
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.OutputDataReceived += ProcessOutputDataHandler;
            process.ErrorDataReceived += ProcessErrorDataHandler;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            Installing = false;
            processThread = null;
            LogAndClearBuildErrors();

            if (arguments.daqriBridge is DaqriBridgeRemove) {
                Removed = true;
            }
        }

        internal static void DisplayInstalledApplications() {
            lock (InstalledApplications) {
                lock (DirtyMutex) {
                    foreach (string installedApp in InstalledApplications) {
                        if (DisplayInstalledApplication(installedApp)) {
                            break;
                        }
                    }
                    Dirty = false;
                }
            }
        }

        private static bool DisplayInstalledApplication(string installedApp) {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(installedApp, EditorStyles.boldLabel);

				bool uninstallClicked = GUILayout.Button ("Uninstall");
				bool logClicked = GUILayout.Button ("Get Log");
				if (!uninstallClicked && !logClicked)
					return false;
				
				if (uninstallClicked) {
					SmartDevicePreferencesWindow.InvokeDaqriBridgeCommand (
						new DaqriBridgeRemove (SmartDevicePreferences.IpAddress, installedApp.TrimEnd('\r')));
				}

				if (logClicked) {
					// Lets open a filebrowser window to allow the user to select the save location and name
					var path = EditorUtility.SaveFilePanel ("Save log file", "", PlayerSettings.productName + ".log", "log");
					if (path.Length != 0) {

						Debug.LogWarning (path);
						SmartDevicePreferencesWindow.InvokeDaqriBridgeCommand (
							new DaqriBridgeApplog (SmartDevicePreferences.IpAddress, installedApp.TrimEnd ('\r'), path));
						return true;
					}
					return false;
				}

				return true;
            }
        }

    }// class
}// namespace
