// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Reign;

using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace Reign.EditorTools
{
	public static class Tools
	{
		#region GeneralTools
		[MenuItem("Edit/Reign/Tools/Print New Guid")]
		static void PrintNewGuid()
		{
			Debug.Log(Guid.NewGuid());
		}

		[MenuItem("Edit/Reign/Tools/Merge Folders")]
		static void MergeFolders()
		{
			string src = EditorUtility.OpenFolderPanel("Src Folder", "", "");
			if (string.IsNullOrEmpty(src)) return;
			string dst = EditorUtility.OpenFolderPanel("Dst Folder", "", "");
			if (string.IsNullOrEmpty(dst)) return;

			Debug.Log("Src Folder: " + src);
			Debug.Log("Dst Folder: " + dst);
			var files = new List<string>();
			gatherFilePaths(src, files);
			foreach (var file in files)
			{
				string newDst = dst + file.Substring(src.Length);
				Directory.CreateDirectory(Path.GetDirectoryName(newDst));
				File.Copy(file, newDst, true);
			}

			AssetDatabase.Refresh();
			Debug.Log("Merge Folders Done!");
		}

		static void gatherFilePaths(string path, List<string> files)
		{
			// add files in path
			var dir = new DirectoryInfo(path);
			foreach (var file in dir.GetFiles())
			{
				if ((file.Attributes & FileAttributes.Hidden) == 0 && (file.Attributes & FileAttributes.Directory) == 0) files.Add(file.FullName);
			}

			// add sub paths
			foreach (var subPath in Directory.GetDirectories(path))
			{
				gatherFilePaths(subPath, files);
			}
		}

		[MenuItem("Edit/Reign/Tools/Clear All PlayerPrefs")]
		static void InitClearAll()
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs Cleared!");
		}
	
		[MenuItem("Edit/Reign/Tools/Reset Editor InApps Prefs (While game is running)")]
		static void InitClearInApps()
		{
			if (InAppPurchaseManager.InAppAPIs == null)
			{
				Debug.LogError("The app must be running with the IAP system initialized!");
				return;
			}
	
			foreach (var api in InAppPurchaseManager.InAppAPIs)
			{
				api.ClearPlayerPrefData();
			}

			Debug.Log("PlayerPrefs for IAP Only Cleared!");
		}

		[MenuItem("Edit/Reign/Clean (For troubleshooting only!)")]
		static void Clean()
		{
			if (!EditorUtility.DisplayDialog("Warning!", "This will remove all Reign plugin files.", "OK", "Cancel")) return;

			using (var stream = new FileStream(Application.dataPath+"/Editor/Reign/CleanSettings", FileMode.Open, FileAccess.Read, FileShare.None))
			using (var reader = new StreamReader(stream))
			{
				string file = reader.ReadLine();
				while (!string.IsNullOrEmpty(file))
				{
					file = Application.dataPath + file;
					try
					{
						if (File.Exists(file)) File.Delete(file);
					}
					catch
					{
						Debug.LogError("Failed to delete file: " + file);
					}

					file = reader.ReadLine();
				}
			}

			AssetDatabase.Refresh();
			Debug.Log("Clean Done!");
		}
		#endregion

		#region PostBuildTools
		static void addPostProjectCompilerDirectives(XDocument doc)
		{
			foreach (var element in doc.Root.Elements())
			{
				if (element.Name.LocalName != "PropertyGroup") continue;
				foreach (var subElement in element.Elements())
				{
					if (subElement.Name.LocalName == "DefineConstants")
					{
						// make sure we need to add compiler directive
						bool needToAdd = true;
						foreach (var name in subElement.Value.Split(';', ' '))
						{
							if (name == "REIGN_POSTBUILD")
							{
								needToAdd = false;
								break;
							}
						}

						// add compiler directive
						if (needToAdd) subElement.Value += ";REIGN_POSTBUILD";
					}
				}
			}
		}

		static void addPostProjectReferences(XDocument doc, string pathToBuiltProject, string extraPath, string productName, string extraRefValue)
		{
			XElement sourceElementRoot = null;
			foreach (var element in doc.Root.Elements())
			{
				if (element.Name.LocalName != "ItemGroup") continue;
				foreach (var subElement in element.Elements())
				{
					if (subElement.Name.LocalName == "Compile")
					{
						sourceElementRoot = element;
						break;
					}
				}

				if (sourceElementRoot != null) break;
			}

			if (sourceElementRoot != null)
			{
				var csSources = new string[]
				{
					"Shared/WinRT/EmailPlugin.cs",
					"Shared/WinRT/MarketingPlugin.cs",
					"Shared/WinRT/MessageBoxPlugin.cs",
					"Shared/WinRT/MicrosoftAdvertising_AdPlugin.cs",
					"Shared/WinRT/MicrosoftStore_InAppPurchasePlugin.cs",
					"Shared/WinRT/StreamPlugin.cs",
					"Shared/WinRT/WinRTPlugin.cs",

					#if UNITY_WP8
					"WP8/AdMob_AdPlugin.cs",
					"WP8/AdMob_InterstitialAdPlugin.cs",

					"WP8/CurrentAppSimulator/CurrentApp.cs",
					"WP8/CurrentAppSimulator/LicenseInformation.cs",
					"WP8/CurrentAppSimulator/ListingInformation.cs",
					"WP8/CurrentAppSimulator/MockIAP.cs",
					"WP8/CurrentAppSimulator/MockReceiptState.cs",
					"WP8/CurrentAppSimulator/MockReceiptStore.cs",
					"WP8/CurrentAppSimulator/ProductLicense.cs",
					"WP8/CurrentAppSimulator/ProductListing.cs",
					#endif
				};

				foreach (var source in csSources)
				{
					// copy cs file
					string sourcePath = string.Format("{0}/{1}/{2}", Application.dataPath, "Plugins/Reign", source);
					string sourceFileName = Path.GetFileName(source);
					File.Copy(sourcePath, string.Format("{0}/{1}{2}/{3}", pathToBuiltProject, productName, extraPath, sourceFileName), true);

					// make sure we need to reference the file
					bool needToRefFile = true;
					foreach (var element in sourceElementRoot.Elements())
					{
						if (element.Name.LocalName == "Compile")
						{
							foreach (var a in element.Attributes())
							{
								if (a.Name.LocalName == "Include" && a.Value == sourceFileName)
								{
									needToRefFile = false;
									break;
								}
							}
						}

						if (!needToRefFile) break;
					}

					// add reference to cs proj
					if (needToRefFile)
					{
						var name = XName.Get("Compile", doc.Root.GetDefaultNamespace().NamespaceName);
						var newSource = new XElement(name);
						newSource.SetAttributeValue(XName.Get("Include"), extraRefValue + sourceFileName);
						sourceElementRoot.Add(newSource);
					}
				}
			}
			else
			{
				Debug.LogError("Reign Post Build Error: Failed to find CS source element in proj!");
			}
		}

		[PostProcessBuild]
		static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			#if UNITY_5
			if (target == BuildTarget.WSAPlayer || target == BuildTarget.WP8Player)
			#else
			if (target == BuildTarget.MetroPlayer || target == BuildTarget.WP8Player)
			#endif
			{
				var productName = PlayerSettings.productName.Replace(" ", "").Replace("_", "");
				
				#if UNITY_5
				if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player)
				#else
				if (EditorUserBuildSettings.metroSDK == MetroSDK.UniversalSDK81 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player)
				#endif
				{
					var projPath = string.Format("{0}/{1}/{1}.Shared/{1}.Shared.projItems", pathToBuiltProject, productName);
					Debug.Log("Modifing Proj: " + projPath);
					var doc = XDocument.Load(projPath);
					addPostProjectReferences(doc, pathToBuiltProject, string.Format("/{0}.Shared", productName), productName, "$(MSBuildThisFileDirectory)");
					doc.Save(projPath);

					projPath = string.Format("{0}/{1}/{1}.Windows/{1}.Windows.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifing Proj: " + projPath);
					doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					doc.Save(projPath);

					projPath = string.Format("{0}/{1}/{1}.WindowsPhone/{1}.WindowsPhone.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifing Proj: " + projPath);
					doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					doc.Save(projPath);
				}
				else
				{
					var projPath = string.Format("{0}/{1}/{1}.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifing Proj: " + projPath);

					var doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					addPostProjectReferences(doc, pathToBuiltProject, "", productName, "");
					doc.Save(projPath);
				}
			}
    	}
		#endregion

		#region PlatformTools
		private static void applyCompilerDirectives(bool append, params string[] directives)
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			string newValue = "";
			if (string.IsNullOrEmpty(valueBlock))
			{
				foreach (var directive in directives)
				{
					newValue += directive;
					if (directive != directives[directives.Length-1]) newValue += ';';
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}
			else
			{
				var values = valueBlock.Split(';', ' ');
				if (append)
				{
					foreach (var value in values)
					{
						newValue += value + ';';
					}
				}

				foreach (var directive in directives)
				{
					bool exists = false;
					foreach (var value in values)
					{
						if (value == directive)
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						newValue += directive;
						if (directive != directives[directives.Length-1]) newValue += ';';
					}
				}
				
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("Compiler Directives set to: " + newValue);
		}

		private static void removeCompilerDirectives(params string[] directives)
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			string newValue = "";
			if (!string.IsNullOrEmpty(valueBlock))
			{
				var values = valueBlock.Split(';', ' ');
				foreach (var value in values)
				{
					bool exists = false;
					foreach (var directive in directives)
					{
						if (value == directive)
						{
							exists = true;
							break;
						}
					}

					if (!exists) newValue += value + ';';
				}
				
				if (newValue.Length != 0 && newValue[newValue.Length-1] == ';') newValue = newValue.Substring(0, newValue.Length-1);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("Compiler Directives set to: " + newValue);
		}

		private static void clearCompilerDirectives()
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, "");
			Debug.Log("Compiler Directives cleard");
		}

		[MenuItem("Edit/Reign/Platform Tools/Disable Reign")]
		private static void DisableReignForPlatform()
		{
			applyCompilerDirectives(true, "DISABLE_REIGN");
		}
		
		[MenuItem("Edit/Reign/Platform Tools/Enable Reign")]
		private static void EnableReignForPlatform()
		{
			removeCompilerDirectives("DISABLE_REIGN");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_GooglePlay")]
		private static void SetPlatformDefaults_Android_GooglePlay()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "GOOGLEPLAY");
			InputTools.ApplyDefaultInputLayout();
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_GooglePlay.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_GooglePlay_TV")]
		private static void SetPlatformDefaults_Android_GooglePlay_TV()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "GOOGLEPLAY", "TV");
			InputTools.ApplyDefaultInputLayout();
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_AndroidTV.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_Amazon")]
		private static void SetPlatformDefaults_Android_Amazon()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "AMAZON");
			InputTools.ApplyDefaultInputLayout();
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_Amazon.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_Amazon_TV")]
		private static void SetPlatformDefaults_Android_Amazon_TV()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "AMAZON", "TV");
			InputTools.ApplyDefaultInputLayout();
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_AmazonFireTV.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_Samsung")]
		private static void SetPlatformDefaults_Android_Samsung()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "SAMSUNG");
			InputTools.ApplyDefaultInputLayout();
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_Samsung.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_Ouya_TV")]
		private static void SetPlatformDefaults_Android_Ouya_TV()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "OUYA", "TV");
			InputTools.applyDefaultLayout("Ouya");
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_Ouya.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Android_GameStick_TV")]
		private static void SetPlatformDefaults_Android_GameStick_TV()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			applyCompilerDirectives(false, "GAMESTICK", "TV");
			InputTools.applyDefaultLayout("GameStick");
			ManifestTools.loadAndroidManifiest(Application.dataPath + "/Editor/Reign/ManifestTools/DefaultAndroidManifests/AndroidManifest_GameStick.xml");
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for iOS")]
		private static void SetPlatformDefaults_iOS()
		{
			#if UNITY_5
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.iOS;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
			#else
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.iPhone;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
			#endif
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for WP8")]
		private static void SetPlatformDefaults_WP8()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WP8;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WP8Player);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for WinRT")]
		private static void SetPlatformDefaults_WinRT()
		{
			#if UNITY_5
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WSA;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WSAPlayer);
			#else
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Metro;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.MetroPlayer);
			#endif
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for BlackBerry")]
		private static void SetPlatformDefaults_BlackBerry()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.BlackBerry;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.BlackBerry);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Win32")]
		private static void SetPlatformDefaults_Win32()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for _OSX")]
		private static void SetPlatformDefaults_OSX()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXIntel);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for Linux")]
		private static void SetPlatformDefaults_Linux()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for PSVita")]
		private static void SetPlatformDefaults_PSVita()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.PSM;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.PSM);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		[MenuItem("Edit/Reign/Platform Tools/Set defaults for WebPlayer")]
		private static void SetPlatformDefaults_WebPlayer()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WebPlayer;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebPlayer);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}

		#if UNITY_5
		[MenuItem("Edit/Reign/Platform Tools/Set defaults for WebGL")]
		private static void SetPlatformDefaults_WebGL()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WebGL;
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebGL);
			clearCompilerDirectives();
			InputTools.ApplyDefaultInputLayout();
		}
		#endif
		#endregion
	}
}