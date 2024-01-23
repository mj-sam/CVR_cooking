#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Manus.Editor
{
	/// <summary>
	/// This class sets and manages the defines for SteamVR and other possible future support
	/// </summary>
	[InitializeOnLoad]
	public class ManusEditor
	{
		// Automatically add the required defines to the project.
		static bool s_AutoManageDefines = true;

		static string s_IconVersionKey = "ManusEditorIconVersion";
		static int s_IconVersion = 1;
	
		const string s_DefineSteamVR = "MANUS_STEAMVR";

		static ManusEditor()
		{
			EditorApplication.playModeStateChanged += OnPlayStateChanged;
			GizmoIcons();
		}

		private static void OnPlayStateChanged(PlayModeStateChange p_State)
		{
			if (s_AutoManageDefines)
			{
				TrySettingSteamVRDefine();
			}
		}

		/// <summary>
		/// Try to add the SteamVR defines to the project. It will only add it if it is not there yet.
		/// </summary>
		public static void TrySettingSteamVRDefine()
		{
			if (!ProjectContainsDefine(s_DefineSteamVR) && IsSteamVRImported())
			{
				SetScriptingDefine(s_DefineSteamVR);
			}
		}

		/// <summary>
		/// Does the project already contain a certain define?
		/// </summary>
		/// <param name="p_Define"></param>
		/// <returns></returns>
		protected static bool ProjectContainsDefine(string p_Define)
		{
			return GetScriptingDefineSymbols().Contains(p_Define);
		}

		/// <summary>
		/// Adds a define to the project.
		/// </summary>
		/// <param name="p_Define"></param>
		protected static void SetScriptingDefine(string p_Define)
		{
			string symbols = GetScriptingDefineSymbols();
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
					BuildTargetGroup.Standalone, symbols + ";" + p_Define);
		}

		protected static string GetScriptingDefineSymbols()
		{
			return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		}

		protected static bool IsSteamVRImported()
		{
			foreach (var t_Assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var t_Type in t_Assembly.GetTypes())
				{
					if (t_Type.FullName == "Valve.VR.SteamVR")
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void GizmoIcons()
		{
			if (EditorPrefs.HasKey(s_IconVersionKey))
			{
				if (s_IconVersion == EditorPrefs.GetInt(s_IconVersionKey)) return;
			}

			if (EditorApplication.isPlayingOrWillChangePlaymode) return; //Don't do anything if going to playmode.
			UnityEngine.Debug.Log("Updating Manus Gizmo Icons");

			string t_LocalPath = "/Scripts/ManusEditor.cs";
			string t_FullPath = GetMonoScriptPathFor(typeof(ManusEditor));
			string t_PluginPath = t_FullPath.Replace(t_LocalPath, string.Empty);

			string[] t_GUIDs = AssetDatabase.FindAssets("t:texture2d", new[] { t_PluginPath + "/Gizmos" });

			List<string> t_FolderList = new List<string>();
			List<string> t_AssetList = new List<string>();

			foreach (string t_GUID in t_GUIDs)
			{
				string t_SrcFile = AssetDatabase.GUIDToAssetPath(t_GUID);
				string t_DstFile = "Assets" + t_SrcFile.Replace(t_PluginPath, string.Empty);

				string[] t_PathElements = t_DstFile.Split('/');
				string t_CPP = t_PathElements[0];
				string t_CP = t_PathElements[0];
				for (int i = 1; i < t_PathElements.Length - 1; i++)
				{
					t_CP += '/' + t_PathElements[i];
					if (!AssetDatabase.IsValidFolder(t_CP) && !t_FolderList.Contains(t_CP))
					{
						t_FolderList.Add(t_CP);
						AssetDatabase.CreateFolder(t_CPP, t_PathElements[i]);
					}
					t_CPP = t_CP;
				}
				t_AssetList.Add(t_SrcFile);
				t_AssetList.Add(t_DstFile);
			}

			for (int i = 0; i < t_AssetList.Count; i += 2)
			{
				string t_Dst = t_AssetList[i + 1];
				UnityEngine.Debug.Log(t_Dst + ": " + AssetDatabase.AssetPathToGUID(t_Dst));
				if (AssetDatabase.AssetPathToGUID(t_Dst) == "")
				{
					AssetDatabase.CopyAsset(t_AssetList[i], t_AssetList[i + 1]);
				}
			}

			EditorPrefs.SetInt(s_IconVersionKey, s_IconVersion);
		}

		private static string GetMonoScriptPathFor(Type p_Type)
		{
			var asset = "";
			var guids = AssetDatabase.FindAssets(string.Format("{0} t:script", p_Type.Name));

			if (guids.Length > 1)
			{
				foreach (var guid in guids)
				{
					var assetPath = AssetDatabase.GUIDToAssetPath(guid);
					var filename = System.IO.Path.GetFileNameWithoutExtension(assetPath);
					if (filename == p_Type.Name)
					{
						asset = guid;
						break;
					}
				}
			}
			else if (guids.Length == 1)
			{
				asset = guids[0];
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("Unable to locate {0}", p_Type.Name);
				return null;
			}

			return AssetDatabase.GUIDToAssetPath(asset);
		}
	}
}
#endif
