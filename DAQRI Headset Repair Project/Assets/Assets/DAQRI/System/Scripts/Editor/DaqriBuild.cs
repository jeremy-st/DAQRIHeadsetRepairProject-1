// C# example
using UnityEditor;
class DaqriBuild
{
	static void BuildExtendedSamples()
	{
		string[] scenes = { "Assets/Daqri/_ExtendedExamples/WorkInstructions/Scenes/WorkInstructions.unity", "Assets/Daqri/_ExtendedExamples/WorkOrder/Scenes/WorkOrder.unity"};
		for (int i = 0; i < scenes.Length; i++)
		{
			string[] path = { scenes[i] };
			//Arg: "/Users/administrator/jenkins/workspace/daqri-unity-extended-samples/SpringboardSDKUnity/Release/ExtendedSamples/"

			PlayerSettings.allowFullscreenSwitch = true;
			PlayerSettings.defaultIsFullScreen = true;
			PlayerSettings.defaultIsNativeResolution = true;
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

			BuildPipeline.BuildPlayer(path, System.Environment.GetCommandLineArgs()[7] + System.IO.Path.GetFileNameWithoutExtension(path[0]), BuildTarget.StandaloneLinux64, BuildOptions.None);

		}
	}

	static void BuildBasicSamples()
	{
		string[] scenes = { "Assets/Daqri/_Example Scenes/UIPrefabs.unity", "Assets/Daqri/_Example Scenes/VIO Example.unity", "Assets/Daqri/_Example Scenes/Reticle Interaction Example.unity", "Assets/Daqri/_Example Scenes/Simple Tracking Example.unity", "Assets/Daqri/_Example Scenes/SensorAccessExample.unity" };
		for (int i = 0; i < scenes.Length; i++)
		{
			string[] path = { scenes[i] };
			//Args: "/Users/administrator/jenkins/workspace/daqri-unity-sampleapps-basic/SpringboardSDKUnity/Release/Samples/"
			UnityEngine.Debug.Log("COMMAND LINE ARGUMENTS: " + System.Environment.GetCommandLineArgs()[7]);

			PlayerSettings.allowFullscreenSwitch = true;
			PlayerSettings.defaultIsFullScreen = true;
			PlayerSettings.defaultIsNativeResolution = true;
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;


			BuildPipeline.BuildPlayer(path, System.Environment.GetCommandLineArgs()[7] + System.IO.Path.GetFileNameWithoutExtension(path[0]), BuildTarget.StandaloneLinux64, BuildOptions.None);
		}
	}

}