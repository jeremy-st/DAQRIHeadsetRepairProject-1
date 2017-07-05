// C# example
using UnityEditor;
class DaqriBuild
{
	static void PerformBuild ()
	{
		string[] scenes = { "Assets/Daqri/_Example Scenes/UIPrefabs.unity","Assets/Daqri/_Example Scenes/VIO Example.unity","Assets/Daqri/_Example Scenes/Reticle Interaction Example.unity","Assets/Daqri/_Example Scenes/Simple Tracking Example.unity","Assets/Daqri/_Example Scenes/Thermal Camera Example.unity" };
		for (int i = 0; i < scenes.Length; i++) {
			string[] path = { scenes [i] };
			// /Users/administrator/jenkins/workspace/
			///Users/administrator/SpringboardSDKUnity/Release/Samples/
			BuildPipeline.BuildPlayer (path, "/Users/administrator/jenkins/workspace/SpringboardSDKUnity/Release/Samples/" + System.IO.Path.GetFileNameWithoutExtension(path[0]), BuildTarget.StandaloneLinux64, BuildOptions.None);
		}
	}
}