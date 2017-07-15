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
 *     File Purpose:        <todo>                                                                                                      *
 *                                                                                                                                      *
 *     Guide:               <todo>                                                                                                      *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;
using System;

namespace DAQRI
{
	public class MenuItems : EditorWindow
	{
		private static string version;
		private static string linkToDocs;

		[MenuItem("DAQRI/Version")]
		static void Init()
		{
			MenuItems window = (MenuItems)EditorWindow.GetWindow(typeof(MenuItems));
			window.name = "About";
			window.Show();
		}

		[MenuItem("DAQRI/Documentation")]
		static void BrowseToDocumentation()
		{
			linkToDocs = GetDocumentationLink ();
			// open a browser and redirect to this link
			try{
				Application.OpenURL(linkToDocs);
			}
			catch (Exception e){
				Debug.LogError (e);
			}
		}

		void OnGUI()
		{
			GUILayout.Label("Daqri Unity Extension", EditorStyles.boldLabel);
			version = GetVersion();
			GUILayout.Label("Version " + version, EditorStyles.boldLabel);//version number
		}

		private static string GetVersion()
		{
			if (File.Exists("Assets/DAQRI/System/DaqriVersion.txt"))
			{
				version = File.ReadAllText("Assets/DAQRI/System/DaqriVersion.txt");
			}
			return version;
		}

		private static string GetDocumentationLink()
		{
			if (File.Exists("Assets/DAQRI/System/DaqriDocsLink.txt"))
			{
				linkToDocs = (File.ReadAllText("Assets/DAQRI/System/DaqriDocsLink.txt")).TrimEnd('\n');
			}
			return linkToDocs;
		}
	}


}
