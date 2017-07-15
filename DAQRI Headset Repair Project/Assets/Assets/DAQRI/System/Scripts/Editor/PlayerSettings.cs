using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Autorun
{
	static bool bOnce = false;
	static Autorun()
	{
		if (!bOnce) {
			EditorApplication.update += Update;
			bOnce = true;
		}
	}

	static bool bOnLoad = true;
	static void Update()
	{
		if (PlayerSettings.displayResolutionDialog != ResolutionDialogSetting.Disabled) {
			if (!bOnLoad) {
				Debug.LogWarning ("Display ResolutionDialog in Player setting is changed to disabled, Building to DAQRI smart device requires displayResolutionDialog to be disabled");
			}
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
		}
		if (!PlayerSettings.defaultIsFullScreen) {
			if (!bOnLoad) {
				Debug.LogWarning ("Full screen is set to true in Player setting, Building to DAQRI smart device requires fullscreen");
			}
			PlayerSettings.defaultIsFullScreen = true;
		}
		bOnLoad = false;
		/*#if UNITY_EDITOR_LINUX
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone).Equals ("DAQRI_SMART_HELMET")) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, "DAQRI_SMART_HELMET");//setting custom define symbols to empty
			}
		#else 
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone).Equals ("")) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, "");//setting custom define symbols to empty
			}
		#endif*/
	}
}
