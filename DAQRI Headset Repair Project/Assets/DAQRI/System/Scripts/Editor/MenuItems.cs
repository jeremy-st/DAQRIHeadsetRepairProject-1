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

namespace DAQRI
{
	public class MenuItems : EditorWindow
	{
		private static string version;

		[MenuItem("DAQRI/Version")]
		static void Init()
		{
			MenuItems window = (MenuItems)EditorWindow.GetWindow(typeof(MenuItems));
			window.name = "About";
			window.Show();
		}

		void OnGUI()
		{
			GUILayout.Label("Daqri Unity Extension", EditorStyles.boldLabel);
			version = GetVersion();
			GUILayout.Label("Version " + version, EditorStyles.boldLabel);//version number
		}

		private static string GetVersion()
		{
			if (File.Exists("Assets/Daqri/DaqriVersion.txt"))
			{
				version = File.ReadAllText("Assets/Daqri/DaqriVersion.txt");
			}
			return version;
		}
	}


}
