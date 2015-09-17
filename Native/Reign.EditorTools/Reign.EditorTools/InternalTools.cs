using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Reign.EditorTools
{
	public static class InternalTools
	{
		[MenuItem("Edit/Reign/Version Info")]
		static void ShowBuildNumber()
		{
			string buildNum = "???";
			try
			{
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					buildNum = reader.ReadLine();
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			EditorUtility.DisplayDialog("Version Info", "Build Number: " + buildNum, "Ok");
		}

		[MenuItem("Edit/Reign/Validate plugin versions are in Sync")]
		static void CheckPluginSync()
		{
			try
			{
				// main
				string mainBuildNumber;
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					mainBuildNumber = reader.ReadLine();
				}

				// Win8 - WP8
				if (File.Exists(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_Windows"))
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_Windows", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					if (reader.ReadLine() != mainBuildNumber)
					{
						EditorUtility.DisplayDialog("Version Check", "Plugins not in sync! Please re-install the plugin!", "Ok");
						return;
					}
				}

				// BB10
				if (File.Exists(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_BB10"))
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_BB10", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					if (reader.ReadLine() != mainBuildNumber)
					{
						EditorUtility.DisplayDialog("Version Check", "Plugins not in sync! Please re-install the plugin!", "Ok");
						return;
					}
				}

				// iOS
				if (File.Exists(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_iOS"))
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_iOS", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					if (reader.ReadLine() != mainBuildNumber)
					{
						EditorUtility.DisplayDialog("Version Check", "Plugins not in sync! Please re-install the plugin!", "Ok");
						return;
					}
				}

				// Android
				if (File.Exists(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_Android"))
				using (var stream = new FileStream(Application.dataPath + "/Plugins/Reign/VersionInfo/ReignVersionCheck_Android", FileMode.Open, FileAccess.Read, FileShare.None))
				using (var reader = new StreamReader(stream))
				{
					if (reader.ReadLine() != mainBuildNumber)
					{
						EditorUtility.DisplayDialog("Version Check", "Plugins not in sync! Please re-install the plugin!", "Ok");
						return;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			EditorUtility.DisplayDialog("Version Check", "All plugin version are in sync!", "Ok");
		}
	}
}