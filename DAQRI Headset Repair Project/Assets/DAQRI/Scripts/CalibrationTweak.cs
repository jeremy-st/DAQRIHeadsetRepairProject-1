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
 *     File Purpose:        Manual Calibration                         																	*
 *                                                                                                                                      *
 *     Guide:               Attach to any gameobject to do manual calibration on left and right display					            	*
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CalibrationTweak : MonoBehaviour {
	
	public string filename = "calib.config";
	#if !UNITY_EDITOR
	struct Calibration
	{
		public Vector3 calibrationLeftOffset;
		public Vector3 calibrationRightOffset;
		Calibration(Vector3 leftOffset,Vector3 rightOffset)
		{
			calibrationLeftOffset = leftOffset;
			calibrationRightOffset = rightOffset;
		}
	}
	float offsetDelta;
	bool calibrateLeftDisplay = true;
	bool displayOnScreen = false;
	GameObject leftDisplay;
	GameObject rightDisplay;
	Calibration calibration;
	// Use this for initialization
	void Start () {
		Screen.SetResolution (2560, 720, true, 100);
		calibration = new Calibration ();
		offsetDelta  = 0.001f;
		leftDisplay = GameObject.Find ("Left Camera");
		rightDisplay = GameObject.Find ("Right Camera");
		if (leftDisplay != null && rightDisplay != null) {
			Invoke ("LateLoadConfig", 0.01f);
		}
	}

	enum STEPS {IDLE, IPD_CALCULATION, DISPLAY_CALIBRATION }
	STEPS currStep;

	IEnumerator CalibrationSteps()
	{
		bool bStepCompleted = false;
		while (!bStepCompleted) {
			if (Input.GetKeyDown (KeyCode.N)) {
				bStepCompleted = true;
			}
			yield return null;
		}
	}

	void LateLoadConfig()
	{
		LoadConfig (filename);
	}

	void OnGUI()
	{
		if (displayOnScreen)
		{
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(0, h * 12 / 100, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 5 / 100;
			style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			string lefttext = "Left : " + calibration.calibrationLeftOffset.ToString("F5");
			string righttext = "right : " + calibration.calibrationRightOffset.ToString("F5");
			GUI.Label(rect, lefttext, style);
			rect = new Rect(0, h * 18 / 100, w, h * 2 / 100);
			GUI.Label(rect, righttext, style);
		}
	}

	// Update is called once per frame
	void Update () {
		Vector3 pos = Vector3.zero;

		if (Input.GetKeyDown (KeyCode.Space)) {
			calibrateLeftDisplay = !calibrateLeftDisplay;
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			displayOnScreen = !displayOnScreen;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			SaveToFile (filename);
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.x += offsetDelta;
			else
				calibration.calibrationRightOffset.x += offsetDelta;
			pos.x += offsetDelta;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.x -= offsetDelta;
			else
				calibration.calibrationRightOffset.x -= offsetDelta;
			pos.x -= offsetDelta;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.y += offsetDelta;
			else
				calibration.calibrationRightOffset.y += offsetDelta;
			pos.y += offsetDelta;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.y -= offsetDelta;
			else
				calibration.calibrationRightOffset.y -= offsetDelta;
			pos.y -= offsetDelta;
		}
		if (Input.GetKeyDown (KeyCode.PageDown)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.z += offsetDelta;
			else
				calibration.calibrationRightOffset.z += offsetDelta;
			pos.z += offsetDelta;
		}
		if (Input.GetKeyDown (KeyCode.PageUp)) {
			if(calibrateLeftDisplay)
				calibration.calibrationLeftOffset.z -= offsetDelta;
			else
				calibration.calibrationRightOffset.z -= offsetDelta;
			pos.z -= offsetDelta;
		}

		if(calibrateLeftDisplay)
			leftDisplay.transform.localPosition += pos;
		else
			rightDisplay.transform.localPosition += pos;
	}

	void SaveToFile(string filename)
	{
		string config = null;
		string data_path = Application.dataPath;
		string fullPath = Path.GetFullPath(data_path + "/../" + filename);
		Debug.Log ("Saved to path : " + fullPath);
		config = JsonUtility.ToJson(calibration);
		System.IO.File.WriteAllText(fullPath,config);
	}

	void LoadConfig(string filename)
	{
		string data_path = Application.dataPath;
		string fullPath = Path.GetFullPath(data_path + "/../" + filename);
		//string config;
		if (File.Exists (fullPath))
			calibration = JsonUtility.FromJson<Calibration> (File.ReadAllText (fullPath));
		else {
			Debug.LogWarning ("Config file does not exist, please press s if you want to save a config");
			return;
		}
		leftDisplay.transform.localPosition += calibration.calibrationLeftOffset;
		rightDisplay.transform.localPosition += calibration.calibrationRightOffset;
	}
	#endif
}