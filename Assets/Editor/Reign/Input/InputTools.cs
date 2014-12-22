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

				default: Debug.LogError("Unsuported platform: " + EditorUserBuildSettings.activeBuildTarget); break;
			}
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