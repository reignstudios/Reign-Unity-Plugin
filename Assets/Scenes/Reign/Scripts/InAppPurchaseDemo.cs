// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using Reign;

public class InAppPurchaseDemo : MonoBehaviour
{
	public static bool created;
	private bool waiting;
	private string[] restoreInAppStatusText;
	private string formatedPriceText;

	#if SAMSUNG
	private string item1 = "xxxxxxxxxxx1";
	private string item2 = "xxxxxxxxxxx2";
	private string item3 = "xxxxxxxxxxx3";
	#else
	private string item1 = "com.reignstudios.test_app1";
	private string item2 = "com.reignstudios.test_app2";
	private string item3 = "com.reignstudios.test_app3";
	#endif

	GUIStyle uiStyle;

	void Start()
	{
		if (created)
		{
			Destroy(gameObject);
			return;
		}
		created = true;
		DontDestroyOnLoad(gameObject);// Make sure the start method never gets called more then once. So we don't init the same API twice.

		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white},
		};

		// InApp-Purchases - NOTE: you can set different "In App IDs" for each platform.
		var inAppIDs = new InAppPurchaseID[3];
		inAppIDs[0] = new InAppPurchaseID(item1, 1.99m, "$", InAppPurchaseTypes.NonConsumable);
		inAppIDs[1] = new InAppPurchaseID(item2, 0.99m, "$", InAppPurchaseTypes.NonConsumable);
		inAppIDs[2] = new InAppPurchaseID(item3, 2.49m, "$", InAppPurchaseTypes.Consumable);
		
		restoreInAppStatusText = new string[inAppIDs.Length];
		var desc = new InAppPurchaseDesc();

		// Global
		desc.Testing = true;
			
		// Editor
		desc.Editor_InAppIDs = inAppIDs;
			
		// WinRT
		desc.WinRT_InAppPurchaseAPI = InAppPurchaseAPIs.MicrosoftStore;
		desc.WinRT_MicrosoftStore_InAppIDs = inAppIDs;
			
		// WP8
		desc.WP8_InAppPurchaseAPI = InAppPurchaseAPIs.MicrosoftStore;
		desc.WP8_MicrosoftStore_InAppIDs = inAppIDs;
			
		// BB10
		desc.BB10_InAppPurchaseAPI = InAppPurchaseAPIs.BlackBerryWorld;
		desc.BB10_BlackBerryWorld_InAppIDs = inAppIDs;
			
		// iOS
		desc.iOS_InAppPurchaseAPI = InAppPurchaseAPIs.AppleStore;
		desc.iOS_AppleStore_InAppIDs = inAppIDs;
		desc.iOS_AppleStore_SharedSecretKey = "";// NOTE: Must set SharedSecretKey, even for Testing!
			
		// Android
		// Choose for either GooglePlay or Amazon.
		// NOTE: Use "player settings" to define compiler directives.
		#if AMAZON
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.Amazon;
		#elif SAMSUNG
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.Samsung;
		#else
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.GooglePlay;
		#endif

		desc.Android_GooglePlay_InAppIDs = inAppIDs;
		desc.Android_GooglePlay_Base64Key = "";// NOTE: Must set Base64Key for GooglePlay in Apps to work, even for Testing!
		desc.Android_Amazon_InAppIDs = inAppIDs;
		desc.Android_Samsung_InAppIDs = inAppIDs;
		desc.Android_Samsung_ItemGroupID = "";

		// init
		InAppPurchaseManager.Init(desc, createdCallback);
	}
	
	private void createdCallback(bool succeeded)
	{
		Debug.Log("InAppPurchaseManager: " + succeeded);
		InAppPurchaseManager.MainInAppAPI.AwardInterruptedPurchases(awardInterruptedPurchases);
	}

	private void awardInterruptedPurchases(string inAppID, bool succeeded)
	{
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			restoreInAppStatusText[appIndex] = "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			Debug.Log(restoreInAppStatusText[appIndex]);
		}
	}

	void OnGUI()
	{
		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< IAP Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
		{
			gameObject.SetActive(false);
			Application.LoadLevel("MainDemo");
			return;
		}
		offset += 34;

		float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

		// Buy
		if (!waiting && GUI.Button(new Rect(0, offset, 148, 64*scale), "Buy NonConsumable"))
		{
			waiting = true;
			// NOTE: You can pass in a "InAppID string value" or an "index" value.
			InAppPurchaseManager.MainInAppAPI.Buy(item1, buyAppCallback);
		}
		
		if (!waiting && GUI.Button(new Rect(0, (64*scale)+offset, 148, 64*scale), "Buy Consumable"))
		{
			waiting = true;
			// NOTE: You can pass in a "InAppID string value" or an "index" value.
			InAppPurchaseManager.MainInAppAPI.Buy(item3, buyAppCallback);
		}

		// Restore
		if (!waiting && GUI.Button(new Rect(0, (128*scale)+offset, 148, 64*scale), "Restore Apps"))
		{
			waiting = true;
			InAppPurchaseManager.MainInAppAPI.Restore(restoreAppsCallback);
		}
		else
		{
			for (int i = 0; i != restoreInAppStatusText.Length; ++i)
			{
				GUI.Label(new Rect(Screen.width-256, (64*i)+offset, 256, 64), restoreInAppStatusText[i]);
			}
		}

		// Get price information
		if (!waiting && GUI.Button(new Rect(148+16, offset, 148, 64*scale), "Get Price Info"))
		{
			waiting = true;
			InAppPurchaseManager.MainInAppAPI.GetProductInfo(productInfoCallback);
		}
		else if (formatedPriceText != null)
		{
			GUI.Label(new Rect(148*2+16+8, offset, 128, 32), formatedPriceText);
		}
	}

	private void productInfoCallback(InAppPurchaseInfo[] priceInfos, bool succeeded)
	{
		waiting = false;
		if (succeeded)
		{
			foreach (var info in priceInfos)
			{
				if (info.ID == item1) formatedPriceText = info.FormattedPrice;
				Debug.Log(string.Format("ID: {0} Price: {1}", info.ID, info.FormattedPrice));
			}
		}
	}

	void buyAppCallback(string inAppID, bool succeeded)
	{
		waiting = false;
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		MessageBoxManager.Show("App Buy Status", inAppID + " Success: " + succeeded + " Index: " + appIndex);
		if (appIndex != -1) restoreInAppStatusText[appIndex] = "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
	}

	void restoreAppsCallback(string inAppID, bool succeeded)
	{
		waiting = false;
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			restoreInAppStatusText[appIndex] = "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			Debug.Log(restoreInAppStatusText[appIndex]);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
