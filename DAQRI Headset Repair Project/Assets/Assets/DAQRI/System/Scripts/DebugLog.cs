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

using UnityEngine;
using System.Collections;
using System.IO;

namespace DAQRI {
	public static class DebugLog{

		//static StreamWriter sw = null;
			
		public enum LABELS{
			ERROR = 0,
			WARNING = 1,
			DEBUG = 2,
		}

		public static void Init()
		{
			//sw = new StreamWriter("TestFile.txt");
		}

		public static void close ()
		{
			//sw.Close ();
		}

		public static void LogErrorTofile(string message)
		{
			//sw.WriteLine(string.Format("{0}:{1}:{2}", System.DateTime.UtcNow.ToString("HH:mm:ss") ,LABELS.ERROR.ToString(),message));
		}

		public static void LogWarningToFile(string message)
		{
			//sw.WriteLine(string.Format("{0}:{1}:{2}", System.DateTime.UtcNow.ToString("HH:mm:ss") , LABELS.WARNING.ToString(),message));
		}

		public static void LogDebugToFile(string message)
		{
			//sw.WriteLine(string.Format("{0}:{1}:{2}", System.DateTime.UtcNow.ToString("HH:mm:ss") , LABELS.DEBUG.ToString(),message));
		}

		public static void LogToConsole(string message)
		{
			//Debug.Log(string.Format("{0}:{1}:{2}", System.DateTime.UtcNow.ToString("HH:mm:ss"),message));
		}
	}
}