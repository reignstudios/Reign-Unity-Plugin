using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Reign.EditorTools
{
	public class InputTools
	{
		[MenuItem("Edit/Reign/Input/Save active platform layout as Default")]
		static void SaveAsDefaultLayout()
		{
			switch (EditorUserBuildSettings.activeBuildTarget)
			{
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					saveActiveLayoutAsDefault("Win32");
					break;

				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXUniversal:
					saveActiveLayoutAsDefault("OSX");
					break;

				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					saveActiveLayoutAsDefault("Linux");
					break;

				case BuildTarget.MetroPlayer:
					saveActiveLayoutAsDefault("WinRT");
					break;

				case BuildTarget.WP8Player:
					saveActiveLayoutAsDefault("WP8");
					break;

				case BuildTarget.iPhone:
					saveActiveLayoutAsDefault("iOS");
					break;

				case BuildTarget.Android:
					saveActiveLayoutAsDefault("Android");
					break;

				case BuildTarget.BlackBerry:
					saveActiveLayoutAsDefault("BlackBerry");
					break;

				case BuildTarget.Tizen:
					saveActiveLayoutAsDefault("Tizen");
					break;

				case BuildTarget.SamsungTV:
					saveActiveLayoutAsDefault("SamsungTV");
					break;

				case BuildTarget.WebPlayer:
				case BuildTarget.WebPlayerStreamed:
					saveActiveLayoutAsDefault("WebPlayer");
					break;

				case BuildTarget.PSM:
				case BuildTarget.PSP2:
					saveActiveLayoutAsDefault("Vita");
					break;

				case BuildTarget.PS3:
					saveActiveLayoutAsDefault("PS3");
					break;

				case BuildTarget.PS4:
					saveActiveLayoutAsDefault("PS4");
					break;

				case BuildTarget.XBOX360:
					saveActiveLayoutAsDefault("Xbox360");
					break;

				case BuildTarget.XboxOne:
					saveActiveLayoutAsDefault("XboxOne");
					break;

				default: Debug.LogError("Unsuported platform: " + EditorUserBuildSettings.activeBuildTarget); break;
			}
		}

		[MenuItem("Edit/Reign/Input/Apply Default layout for active platform")]
		static void ApplyDefaultInputLayout()
		{
			switch (EditorUserBuildSettings.activeBuildTarget)
			{
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					applyDefaultLayout("Win32");
					break;

				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXUniversal:
					applyDefaultLayout("OSX");
					break;

				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					applyDefaultLayout("Linux");
					break;

				case BuildTarget.MetroPlayer:
					applyDefaultLayout("WinRT");
					break;

				case BuildTarget.WP8Player:
					applyDefaultLayout("WP8");
					break;

				case BuildTarget.iPhone:
					applyDefaultLayout("iOS");
					break;

				case BuildTarget.Android:
					applyDefaultLayout("Android");
					break;

				case BuildTarget.BlackBerry:
					applyDefaultLayout("BlackBerry");
					break;

				case BuildTarget.Tizen:
					applyDefaultLayout("Tizen");
					break;

				case BuildTarget.SamsungTV:
					applyDefaultLayout("SamsungTV");
					break;

				case BuildTarget.WebPlayer:
				case BuildTarget.WebPlayerStreamed:
					applyDefaultLayout("WebPlayer");
					break;

				case BuildTarget.PSM:
				case BuildTarget.PSP2:
					applyDefaultLayout("Vita");
					break;

				case BuildTarget.PS3:
					applyDefaultLayout("PS3");
					break;

				case BuildTarget.PS4:
					applyDefaultLayout("PS4");
					break;

				case BuildTarget.XBOX360:
					applyDefaultLayout("Xbox360");
					break;

				case BuildTarget.XboxOne:
					applyDefaultLayout("XboxOne");
					break;

				default: Debug.LogError("Unsuported platform: " + EditorUserBuildSettings.activeBuildTarget); break;
			}
		}

		[MenuItem("Edit/Reign/Input/Save custom platform layout")]
		static void SaveCustomPlatformLayout()
		{
			string filename = EditorUtility.SaveFilePanel("Input Layout", Application.dataPath+"", "InputManager_Custom", "");
			if (string.IsNullOrEmpty(filename)) return;
			
			EditorApplication.SaveAssets();
			string root = Application.dataPath.Replace("Assets", "ProjectSettings");
			File.Copy(root + "/InputManager.asset", filename, true);
			AssetDatabase.Refresh();
		}

		[MenuItem("Edit/Reign/Input/Load custom platform layout")]
		static void LoadCustomPlatformLayout()
		{
			string filename = EditorUtility.OpenFilePanel("Input Layout", Application.dataPath+"", "");
			if (string.IsNullOrEmpty(filename)) return;
			
			EditorApplication.SaveAssets();
			string root = Application.dataPath.Replace("Assets", "ProjectSettings");
			File.Copy(filename, root + "/InputManager.asset", true);
			AssetDatabase.Refresh();
		}

		static void saveActiveLayoutAsDefault(string platform)
		{
			EditorApplication.SaveAssets();
			string root = Application.dataPath.Replace("Assets", "ProjectSettings");
			File.Copy(root + "/InputManager.asset", Application.dataPath+"/Editor/Reign/Input/DefaultLayouts/InputManager_" + platform, true);
			AssetDatabase.Refresh();
		}

		static void applyDefaultLayout(string platform)
		{
			EditorApplication.SaveAssets();
			string root = Application.dataPath.Replace("Assets", "ProjectSettings");
			File.Copy(Application.dataPath+"/Editor/Reign/Input/DefaultLayouts/InputManager_" + platform, root + "/InputManager.asset", true);
			AssetDatabase.Refresh();
		}
	}
}