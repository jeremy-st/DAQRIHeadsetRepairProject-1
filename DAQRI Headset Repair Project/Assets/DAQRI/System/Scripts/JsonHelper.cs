﻿/****************************************************************************************************************************************
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
 *     Reference:        https://github.com/Bekwnn/UnityJsonHelper
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using System.Text.RegularExpressions;
using System.Collections.Generic;

public class MyJsonHelper
{

	public static string GetJsonObject(string jsonString, string handle)
	{
		string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

		Regex regx = new Regex(pattern);

		Match match = regx.Match(jsonString);

		if (match.Success)
		{
			int bracketCount = 1;
			int i;
			int startOfObj = match.Index + match.Length;
			for (i = startOfObj; bracketCount > 0; i++)
			{
				if (jsonString[i] == '{') bracketCount++;
				else if (jsonString[i] == '}') bracketCount--;
			}
			return "{" + jsonString.Substring(startOfObj, i - startOfObj);
		}

		//no match, return null
		return null;
	}

	public static string[] GetJsonObjects(string jsonString, string handle)
	{
		string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

		Regex regx = new Regex(pattern);

		//check if there's a match at all, return null if not
		if (!regx.IsMatch(jsonString)) return null;

		List<string> jsonObjList = new List<string>();

		//find each regex match
		foreach (Match match in regx.Matches(jsonString))
		{
			int bracketCount = 1;
			int i;
			int startOfObj = match.Index + match.Length;
			for (i = startOfObj; bracketCount > 0; i++)
			{
				if (jsonString[i] == '{') bracketCount++;
				else if (jsonString[i] == '}') bracketCount--;
			}
			jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));
		}

		return jsonObjList.ToArray();
	}

	public static string[] GetJsonObjectArray(string jsonString, string handle)
	{
		string pattern = "\"" + handle + "\"\\s*:\\s*\\[\\s*{";

		Regex regx = new Regex(pattern);

		List<string> jsonObjList = new List<string>();

		Match match = regx.Match(jsonString);

		if (match.Success)
		{
			int squareBracketCount = 1;
			int curlyBracketCount = 1;
			int startOfObjArray = match.Index + match.Length;
			int i = startOfObjArray;
			while (true)
			{
				if (jsonString[i] == '[') squareBracketCount++;
				else if (jsonString[i] == ']') squareBracketCount--;

				int startOfObj = i;
				for (i = startOfObj; curlyBracketCount > 0; i++)
				{
					if (jsonString[i] == '{') curlyBracketCount++;
					else if (jsonString[i] == '}') curlyBracketCount--;
				}
				jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));

				// continue with the next array element or return object array if there is no more left
				while (jsonString[i] != '{')
				{
					if (jsonString[i] == ']' && squareBracketCount == 1)
					{
						return jsonObjList.ToArray();
					}
					i++;
				}
				curlyBracketCount = 1;
				i++;
			}
		}

		//no match, return null
		return null;
	}
}