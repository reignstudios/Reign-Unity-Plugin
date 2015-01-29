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

		[MenuItem("Edit/Reign/Tools/Merge Reign Android Manifest")]
		static void MergeAndroidManifest()
		{
			if (!EditorUtility.DisplayDialog("Android Manifest Merge Tool", "Are you sure? You can also reference ReignAndroidManifest.xml and manually merge.\n\nThis tool will merge ReignAndroidManifest.xml with AndroidManifest.xml", "Ok", "Cancel")) return;
	
			XDocument mainDoc = XDocument.Load(Application.dataPath+"/Plugins/Android/AndroidManifest.xml");
			XDocument reignDoc = XDocument.Load(Application.dataPath+"/Plugins/Android/ReignAndroidManifest.xml");
			var context = reignDoc.Descendants("Types").Descendants("Type").Except(mainDoc.Descendants("Types").Descendants("Type")).ToArray();
			mainDoc.Root.Add(context);
		
			var settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			using (var writer = XmlWriter.Create(Application.dataPath+"/Plugins/Android/AndroidManifest.xml", settings))
			{
				mainDoc.WriteTo(writer);
			}
		
			EditorUtility.DisplayDialog("Android Manifest Merge Tool", "Successful!", "Ok");
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
		
		[MenuItem("Edit/Reign/Disable Reign")]
		private static void DisableReignForPlatform()
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			if (string.IsNullOrEmpty(valueBlock))
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, "DISABLE_REIGN");
			}
			else
			{
				string newValue = "";
				var values = valueBlock.Split(';', ' ');
				foreach (var value in values)
				{
					if (value == "DISABLE_REIGN") return;
					newValue += value + ';';
				}
				
				newValue += "DISABLE_REIGN";
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("DISABLE_REIGN added to your Player Settings");
		}
		
		[MenuItem("Edit/Reign/Enable Reign")]
		private static void EnableReignForPlatform()
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			if (!string.IsNullOrEmpty(valueBlock))
			{
				string newValue = "";
				var values = valueBlock.Split(';', ' ');
				foreach (var value in values)
				{
					if (value != "DISABLE_REIGN") newValue += value + ';';
				}
				
				if (newValue.Length != 0 && newValue[newValue.Length-1] == ';') newValue = newValue.Substring(0, newValue.Length-1);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("DISABLE_REIGN removed from your Player Settings");
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
			if (target == BuildTarget.MetroPlayer || target == BuildTarget.WP8Player)
			{
				var productName = PlayerSettings.productName.Replace(" ", "").Replace("_", "");
				
				if (EditorUserBuildSettings.metroSDK == MetroSDK.UniversalSDK81 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player)
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
	}
}