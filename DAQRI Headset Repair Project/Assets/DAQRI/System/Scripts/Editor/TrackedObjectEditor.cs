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
 *     File Purpose:       Editor Script to draw custom inspector for TrackedObject showing useful information about the target	such as *
 *                         file preview, width and height in meters etc                                                                 *
 *     Guide:              <todo>                                                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

namespace DAQRI
{
	[CustomEditor (typeof(TrackedObject))]
	public class TrackedObjectEditor : Editor
	{
		//for folding out UnityEvent properties
		public static bool showUnityEvents = true;

		private static string MATERIAL_PATH = "Assets/DAQRI/System/Materials/TrackedObject.mat";
		private static string TEXTURE_PATH = "Assets/Editor Default Resources/";
		private static string STREAMING_ASSET_PATH = "Assets/StreamingAssets/";

		//to exclude script name from the inspector
		private static readonly string[] _dontIncludeMe = new string[] { "m_Script", "OnTrackerFound", "OnTrackerLost"};//, "OnMarkerlessTrackingGained", "OnMarkerlessTrackingLost" };

		public void Awake() {
			TrackedObject to = target as TrackedObject;
			if (File.Exists(Path.Combine(TEXTURE_PATH, to.targetName))) {
				TrackedObjectEditor.UpdateMesh (to);
				TrackedObjectEditor.UpdateMaterial (to);
				TrackedObjectEditor.UpdateScale (to);
			}
		}

		public override void OnInspectorGUI ()
		{
			TrackedObject to = target as TrackedObject;
			TrackedObjectEditor.ConfigureInspector (to);
			serializedObject.Update ();
			serializedObject.ApplyModifiedProperties();
			DrawPropertiesExcluding (serializedObject, _dontIncludeMe);
			showUnityEvents = EditorGUILayout.Foldout(showUnityEvents, "Tracking Events");
			if (showUnityEvents) 
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrackerFound"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrackerLost"));
				//EditorGUILayout.PropertyField(serializedObject.FindProperty("OnMarkerlessTrackingGained"));
				//EditorGUILayout.PropertyField(serializedObject.FindProperty("OnMarkerlessTrackingLost"));

			}

			//This lets you save the properties that you set for the prefab the first time
			EditorUtility.SetDirty(target);

			if (GUI.changed) {
				//Apparently the above line isn't enough. We need this for subsequent edits to be saved as well. 
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			serializedObject.ApplyModifiedProperties ();
		}

		public void OnSceneGUI()
		{
			Repaint ();
		}

		#region private_methods

		private static void ConfigureInspector (TrackedObject to)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.TextField ("Image File", to.targetName);
			if (GUILayout.Button ("...", new GUILayoutOption[] { GUILayout.Width (25) })) {
				if (TrackedObjectEditor.BrowseAndLoadTexture (to)) {
					TrackedObjectEditor.UpdateMesh (to);
					TrackedObjectEditor.UpdateMaterial (to);
					TrackedObjectEditor.UpdateScale (to);
				}
			}
			EditorGUILayout.EndHorizontal ();
	
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			if (to.previewImage == null) {
				GUILayout.Label ("No Preview\n Available", new GUILayoutOption[] {
					GUILayout.Height (Screen.width / 4),
					GUILayout.Width (Screen.width / 4)
				});
			} else 
			{
				GUILayout.Label (to.previewImage, new GUILayoutOption[] {
					GUILayout.Height (Screen.width / 10),
					GUILayout.Width (Screen.width / 10)
				});
				EditorGUILayout.BeginVertical ();
				float rwWidth = EditorGUILayout.FloatField("Physical Width (m)", to.WidthInMeters, new GUILayoutOption[0]);
				to.HeightInMeters = (rwWidth / to.previewImage.width) * to.previewImage.height;
				float rwHeight = EditorGUILayout.FloatField("Physical Height (m)", to.HeightInMeters, new GUILayoutOption[0]);

				EditorGUILayout.BeginHorizontal ();
				if (Application.isPlaying) {
					string status = to.IsVisible ? "Status: Tracking" : "Status: Not Tracking";
					EditorGUILayout.LabelField (status, new GUILayoutOption[] { GUILayout.Width (115) });
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndVertical ();

				//if width is updated in the inspector, adjust the height and update mesh
				if (rwWidth != to.WidthInMeters) {
					to.WidthInMeters = rwWidth;
					rwHeight = (rwWidth / to.previewImage.width) * to.previewImage.height;
					to.HeightInMeters = rwHeight;
					TrackedObjectEditor.UpdateMesh (to);
					TrackedObjectEditor.UpdateMaterial (to);
					TrackedObjectEditor.UpdateScale (to);

				}

				//if height is updated in the inpector, adjust the width and update mesh
				if (rwHeight != to.HeightInMeters) {
					to.HeightInMeters = rwHeight;
					rwWidth = (rwHeight / to.previewImage.height) * to.previewImage.width;
					to.WidthInMeters = rwWidth;
					TrackedObjectEditor.UpdateMesh (to);
					TrackedObjectEditor.UpdateMaterial (to);
					TrackedObjectEditor.UpdateScale (to);
				}
			}
			EditorGUILayout.EndHorizontal ();
		}

		private static bool BrowseAndLoadTexture(TrackedObject to)
		{
			int index;
			string[] validFileExtensions = { ".png", ".jpeg", ".jpg" };
			string externalPath = "";
			externalPath = EditorUtility.OpenFilePanel("Select an image ... ", externalPath, "");
			if (externalPath.Length != 0)
			{
				//validate file extension
				index = externalPath.LastIndexOf(".", System.StringComparison.CurrentCulture);
				string extension = externalPath.Substring(index);

				if (!validFileExtensions.Contains(extension))
				{
					Debug.LogError("Invalid File Type. Only files of type png, jpeg and jpg are allowed");
					return false;
				}

				index = externalPath.LastIndexOf("/", System.StringComparison.CurrentCulture) + 1;
				string filename = externalPath.Substring(index);

				string assetPath = TEXTURE_PATH + filename;
				if (!Directory.Exists(TEXTURE_PATH))
				{
					Debug.Log("Creating Streaming Assets");
					Directory.CreateDirectory(TEXTURE_PATH);
				}
				//check if texture exists in TEXTURE_PATH
				if (!File.Exists(assetPath))
				{
					FileUtil.CopyFileOrDirectory(externalPath, assetPath);
					AssetDatabase.Refresh();

					TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

					importer.npotScale = TextureImporterNPOTScale.None;
					importer.mipmapEnabled = true;
					importer.isReadable = true;
					importer.maxTextureSize = 512;
					AssetDatabase.WriteImportSettingsIfDirty(assetPath);

					AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
					to.previewImage = EditorGUIUtility.Load(filename) as Texture;
				}
				else
				{
					to.previewImage = EditorGUIUtility.Load(filename) as Texture;
				}

				//check if texture exists in StreamingAssets
				if (!Directory.Exists(STREAMING_ASSET_PATH))
				{
					Debug.Log("Creating Streaming Assets");
					Directory.CreateDirectory(STREAMING_ASSET_PATH);
				}
				if (!File.Exists(STREAMING_ASSET_PATH + filename))
				{
					FileUtil.CopyFileOrDirectory(externalPath, STREAMING_ASSET_PATH + filename);
				}
				to.gameObject.name = "TO_" + filename.Remove(filename.Length - 4);
				to.targetName = filename;//.Remove(filename.Length - 4);
				string dir = Application.streamingAssetsPath;
				to.targetPath = System.IO.Path.Combine(dir, filename);
				to.WidthInMeters = 1.0f;

				//Unity apparently doesn't see updating the texture in the inspector as a GUI change.
				//So explicitly marking the scene dirty. 
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				return true;

			}

			return false;
		}

		private static void UpdateMesh (TrackedObject to)
		{
			GameObject gameObject = to.gameObject;
			MeshFilter mf = gameObject.GetComponent<MeshFilter> ();
			if (!mf) {
				mf = gameObject.AddComponent<MeshFilter> ();
			}

			Mesh mesh = new Mesh ();
			mf.mesh = mesh;

			//Vertices
			Vector3[] vertices = new Vector3[4];

			float X = to.WidthInMeters;
			float Y = X / to.Aspect;

			vertices [0] = new Vector3 (-0.5f * X, -0.5f * Y, 0);
			vertices [1] = new Vector3 (0.5f * X, -0.5f * Y, 0);
			vertices [2] = new Vector3 (-0.5f * X, 0.5f * Y, 0);
			vertices [3] = new Vector3 (0.5f * X, 0.5f * Y, 0);

			//Triangles

			int[] tri = new int[6];
			tri [0] = 0;
			tri [1] = 2;
			tri [2] = 1;
			tri [3] = 2;
			tri [4] = 3;
			tri [5] = 1;

			//Normals (only if  you want to display object in the game)

			Vector3[] normals = new Vector3[4];
			normals [0] = -Vector3.forward;
			normals [1] = -Vector3.forward;
			normals [2] = -Vector3.forward;
			normals [3] = -Vector3.forward;

			//UVs (How textures are displayed)

			Vector2[] uv = new Vector2[4];
			uv [0] = new Vector2 (0, 0);
			uv [1] = new Vector2 (1, 0);
			uv [2] = new Vector2 (0, 1);
			uv [3] = new Vector2 (1, 1);

			//Assign Arrays 

			mesh.vertices = vertices;
			mesh.triangles = tri;
			mesh.normals = normals;
			mesh.uv = uv;

			//To Prevent Backside Culling
			var verticestemp = mesh.vertices;
			var szV = verticestemp.Length;
			var newVerts = new Vector3[szV*2];
			var newUv = new Vector2[szV*2];
			var newNorms = new Vector3[szV*2];
			for (var j=0; j< szV; j++){
				newVerts[j] = newVerts[j+szV] = vertices[j];
				newUv [j] = uv[j];
				newUv[j+szV] = new Vector2(0.0f,0.0f);//uv[j];
				newNorms[j] = normals[j];
				newNorms[j+szV] = -normals[j];
			}
			var triangles = mesh.triangles;
			var szT = triangles.Length;
			var newTris = new int[szT*2]; // double the triangles
			for (var i=0; i< szT; i+=3){
				// copy the original triangle
				newTris[i] = triangles[i];
				newTris[i+1] = triangles[i+1];
				newTris[i+2] = triangles[i+2];
				// save the new reversed triangle
				var j = i+szT; 
				newTris[j] = triangles[i]+szV;
				newTris[j+2] = triangles[i+1]+szV;
				newTris[j+1] = triangles[i+2]+szV;
			}
			mesh.vertices = newVerts;
			mesh.uv = newUv;
			mesh.normals = newNorms;
			mesh.triangles = newTris; // assign triangles last!

		}

		private static void UpdateMaterial (TrackedObject to)
		{
			Material material = (Material)AssetDatabase.LoadAssetAtPath (MATERIAL_PATH, typeof(Material));

			if (material == null) {
				Debug.LogError ("Count not find reference material at " + MATERIAL_PATH);
				return;
			}

			Material material2 = new Material (material);

			material2.SetTexture ("_MainTex", to.previewImage);
			material2.name = to.previewImage.name;

			MeshRenderer renderer = to.gameObject.GetComponent<MeshRenderer> ();
			if (!renderer) {
				renderer = to.gameObject.AddComponent<MeshRenderer> ();
			}
			renderer.sharedMaterial = material2;

		}

		private static void UpdateScale (TrackedObject to)
		{
			to.transform.localScale = new Vector3 (1, 1, 1);
		}

		#endregion private_methods

	}
}
