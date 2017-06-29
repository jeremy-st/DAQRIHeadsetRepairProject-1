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
 *     File Purpose:       Reads Smart Helmet Settings from Menu/DAQRI/Smart Helmet Settings and automatically deploys into the         *
 * 						   target device  																					            *
 *                         as part of the postprocess build                                                                             *
 *     Guide:              <todo>                                                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/


using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[Serializable]
public class SmartHelmetPreferences
{
	public string ip_address;
}

public class SmartHelmetPreferencesWindow : EditorWindow
{
	private const string THOR_PREFERENCES = "Assets/DAQRI/System/Scripts/Editor/SmartHelmetPreferences.json";
	private static SmartHelmetPreferences thorPref = new SmartHelmetPreferences();

	//arguments passed on to the binaries
	private static string buildPath;
	private static string iconPath;
	private static string appName;

	[MenuItem("DAQRI/Smart Helmet Settings")]
	static void Init()
	{
		SmartHelmetPreferencesWindow window = (SmartHelmetPreferencesWindow)EditorWindow.GetWindow(typeof(SmartHelmetPreferencesWindow));
		window.name = "Settings";
		ReadSmartHelmetPreferences();
		window.Show();
	}

	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		SetPathArguments(path);
		ReadSmartHelmetPreferences();
		DeployToSmartHelmet();
	}

	private static void SetPathArguments(string path)
	{
		buildPath = path;
		int indexBeg = path.LastIndexOf("/", StringComparison.CurrentCulture) + 1;
		int indexEnd = path.IndexOf(".x86_64", StringComparison.CurrentCulture);
		int length = indexEnd - indexBeg;
		//appName = path.Substring(indexBeg, length);
		appName = PlayerSettings.productName;
		iconPath = buildPath.Substring(0, indexEnd);
		iconPath = iconPath + "_Data/Resources/UnityPlayer.png";
	}

	void OnGUI()
	{
		GUILayout.Label("Deploy to Smart Helmet", EditorStyles.boldLabel);
		EditorStyles.label.wordWrap = true;

		EditorGUILayout.BeginHorizontal();
		string message = "App Icon will be picked up from the Build Settings.\nApp Name is the product name you provide in the Project Settings.\n";
		EditorGUILayout.LabelField(message, new GUILayoutOption[0]);
		EditorGUILayout.Separator();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Target Helmet IP Address: ", new GUILayoutOption[] { GUILayout.Width(150) });

		thorPref.ip_address = EditorGUILayout.TextField(thorPref.ip_address);

		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Save Settings", new GUILayoutOption[0]))
		{
			SaveSmartHelmetPreferences();
			SmartHelmetPreferencesWindow window = (SmartHelmetPreferencesWindow)EditorWindow.GetWindow(typeof(SmartHelmetPreferencesWindow));
			window.Close();
		}


	}

	private static void ReadSmartHelmetPreferences()
	{
		string jsonString;

		if (File.Exists(THOR_PREFERENCES))
		{
			jsonString = File.ReadAllText(THOR_PREFERENCES);
			string thorPreferences = MyJsonHelper.GetJsonObject(jsonString, "SmartHelmetPreferences");
			thorPref = JsonUtility.FromJson<SmartHelmetPreferences>(thorPreferences);
		}
		else
		{
			UnityEngine.Debug.LogError("Invalid path or filename");
		}
	}

	private void SaveSmartHelmetPreferences()
	{
		if (File.Exists(THOR_PREFERENCES))
		{
			string updatedString = JsonUtility.ToJson(thorPref);
			updatedString = "{ \"SmartHelmetPreferences\":" + updatedString;
			File.WriteAllText(THOR_PREFERENCES, updatedString);
		}
		else
		{
			UnityEngine.Debug.LogError("Invalid path or filename");
		}
	}

	//TODO: Need to check for the platform before picking the appropriate binaries
	private static void DeployToSmartHelmet()
	{
		string filename = "";

		#if UNITY_EDITOR_OSX

		filename = Application.dataPath + "/DAQRI/System/Deploy/OSX/unity_deploy_osx";

#elif UNITY_EDITOR_WIN

		filename = Application.dataPath + "/DAQRI/System/Deploy/WIN/unity_deploy_win.exe";

#else //LINUX

		filename = Application.dataPath + "/DAQRI/System/Deploy/LINUX/unity_deploy_linux";

#endif

		string arg1 = "\"" + appName + "\"";
		string arg2 = thorPref.ip_address;
		string arg3 = "\"" + buildPath + "\"";
		string arg4 = "\"" + iconPath + "\"";

		//UnityEngine.Debug.Log("APP Name: " + arg1);
		//UnityEngine.Debug.Log("IP Address: " + arg2);
		//UnityEngine.Debug.Log("Build Path: " + arg3);
		//UnityEngine.Debug.Log("Icon Path: " + arg4);

		Process process = new Process();
		process.StartInfo.FileName = filename;
		process.StartInfo.Arguments = arg1 + " " + arg2 + " " + arg3 + " " + arg4;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.FileName = filename;

		// Redirects the standard input so that commands can be sent to the shell.
		process.StartInfo.RedirectStandardInput = true;

		process.OutputDataReceived += ProcessOutputDataHandler;
		process.ErrorDataReceived += ProcessErrorDataHandler;

		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		process.WaitForExit();

	}

	public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
	{
		if (outLine.Data.Contains("ERROR")){
			int index = outLine.Data.IndexOf(":", StringComparison.CurrentCulture);
			string errorMsg = outLine.Data.Substring(index + 1);
			UnityEngine.Debug.LogError("Deployment to Smart Helmet failed : " + errorMsg);
		}
	}

	public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
	{
		if (outLine.Data.Contains("ERROR"))
		{
			int index = outLine.Data.IndexOf(":", StringComparison.CurrentCulture);
			string errorMsg = outLine.Data.Substring(index + 1);
			UnityEngine.Debug.LogError("Deployment to Smart Helmet failed : " + errorMsg);
		}
	}
}