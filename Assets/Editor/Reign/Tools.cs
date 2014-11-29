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

namespace Reign.EditorTools
{
	public static class Tools
	{
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
		private static void DisableReignForPlatform(BuildTargetGroup platform)
		{
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
		}
		
		[MenuItem("Edit/Reign/Enable Reign")]
		private static void EnableReignForPlatform(BuildTargetGroup platform)
		{
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
		}

		[PostProcessBuild]
		static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			if (target == BuildTarget.MetroPlayer || target == BuildTarget.WP8Player)
			{
				var productName = PlayerSettings.productName.Replace(" ", "").Replace("_", "");
				string projPath = string.Format("{0}/{1}/{1}.csproj", pathToBuiltProject, productName);
				Debug.Log("Modifing Win8 Proj: " + projPath);

				var csProj = XDocument.Load(projPath);

				// add compiler directives
				foreach (var element in csProj.Root.Elements())
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

				// find cs source ItemGroup
				XElement sourceElementRoot = null;
				foreach (var element in csProj.Root.Elements())
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
						File.Copy(sourcePath, string.Format("{0}/{1}/{2}", pathToBuiltProject, productName, sourceFileName), true);

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
							var name = XName.Get("Compile", csProj.Root.GetDefaultNamespace().NamespaceName);
							var newSource = new XElement(name);
							newSource.SetAttributeValue(XName.Get("Include"), sourceFileName);
							sourceElementRoot.Add(newSource);
						}
					}
				}
				else
				{
					Debug.LogError("Reign Post Build Error: Failed to find CS source element in proj!");
				}

				csProj.Save(projPath);
			}
    	}
	}
}