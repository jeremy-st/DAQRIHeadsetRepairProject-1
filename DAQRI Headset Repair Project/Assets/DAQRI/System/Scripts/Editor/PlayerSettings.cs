using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Autorun
{
	static Autorun()
	{
		EditorApplication.update += Update;
	}

	static void Update()
	{
		if (PlayerSettings.displayResolutionDialog != ResolutionDialogSetting.Disabled) {
			Debug.LogWarning ("Building to DAQRI smart helmet requires displayResolutionDialog to be disabled");
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
		}
		if (!PlayerSettings.defaultIsFullScreen) {
			Debug.LogWarning ("Building to DAQRI smart helmet requires fullscreen");
			PlayerSettings.defaultIsFullScreen = true;
		}
	}
}
