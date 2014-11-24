// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections;
using Reign;

public class AdsDemo : MonoBehaviour
{
	public static AdsDemo Singleton;
	private Ad ad;
	private string adStatus;
	GUIStyle uiStyle;

	void Start()
	{
		if (Singleton != null)
		{
			Destroy(gameObject);
			return;
		}
		Singleton = this;

		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white},
		};

		// NOTE: use 'ad.Visible = true/false' instead of delete and re-creating Ads!
		DontDestroyOnLoad(gameObject);// Make sure the start method never gets called more then once. So we don't create the same Ad twice.
		adStatus = "none";

		// Ads - NOTE: You can pass in multiple "AdDesc" objects if you want more then one ad.
		var desc = new AdDesc();
		// global settings
		desc.Testing = true;// NOTE: To test ads on iOS, you must enable them in iTunes Connect.
		desc.Visible = true;
		desc.EventCallback = eventCallback;

		// Editor
		desc.Editor_AdAPI = AdAPIs.EditorTestAd;
		desc.Editor_AdGravity = AdGravity.BottomCenter;
		desc.Editor_AdWidth = 256;
		desc.Editor_AdHeight = 32;
		//desc.Editor_AdGUIOverrideEnabled = true;

		desc.Editor_MillennialMediaAdvertising_APID = "";
		desc.Editor_MillennialMediaAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.Editor_GuiAdScale = 1;
		//desc.Editor_MillennialMediaAdvertising_RefreshRate = 120,

		// Win8 settings
		desc.Win8_AdAPI = AdAPIs.MicrosoftAdvertising;
		desc.Win8_MicrosoftAdvertising_ApplicationID = "";
		desc.Win8_MicrosoftAdvertising_UnitID = "";
		desc.Win8_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.Win8_MicrosoftAdvertising_AdSize = Win8_MicrosoftAdvertising_AdSize.Wide_728x90;
		//desc.Win8_MicrosoftAdvertising_UseBuiltInRefresh = false;
		//desc.Win8_MicrosoftAdvertising_RefreshRate = 120;
			
		// WP8 settings
		desc.WP8_AdAPI = AdAPIs.MicrosoftAdvertising;
		desc.WP8_MicrosoftAdvertising_ApplicationID = "";
		desc.WP8_MicrosoftAdvertising_UnitID = "";
		desc.WP8_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.WP8_MicrosoftAdvertising_AdSize = WP8_MicrosoftAdvertising_AdSize.Wide_480x80;
		//desc.WP8_MicrosoftAdvertising_UseBuiltInRefresh = false;
		//desc.WP8_MicrosoftAdvertising_RefreshRate = 120;
			
		desc.WP8_AdMob_UnitID = "";// NOTE: You can use legacy (PublisherID) too, You MUST have this even for Testing!
		desc.WP8_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.WP8_AdMob_AdSize = Reign.WP8_AdMob_AdSize.Banner;
			
		// BB10 settings
		desc.BB10_AdAPI = AdAPIs.MillennialMediaAdvertising;
		desc.BB10_BlackBerryAdvertising_ZoneID = "";
		desc.BB10_BlackBerryAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.BB10_BlackBerryAdvertising_AdSize = BB10_BlackBerryAdvertising_AdSize.Wide_320x53;

		desc.BB10_MillennialMediaAdvertising_APID = "";
		desc.BB10_MillennialMediaAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.BB10_GuiAdScale = 2;
		//desc.BB10_AdGUIOverrideEnabled = true;
		//desc.BB10_MillennialMediaAdvertising_RefreshRate = 120;
			
		// iOS settings
		desc.iOS_AdAPI = AdAPIs.iAd;
		desc.iOS_iAd_AdGravity = AdGravity.BottomCenter;
			
		desc.iOS_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.iOS_AdMob_UnitID = "";// NOTE: You can use legacy (PublisherID) too, You MUST have this even for Testing!
		desc.iOS_AdMob_AdSize = iOS_AdMob_AdSize.Banner_320x50;
			
		// Android settings
		#if AMAZON
		desc.Android_AdAPI = AdAPIs.AmazonAds;// Choose between AdMob or AmazonAds
		#else
		desc.Android_AdAPI = AdAPIs.AdMob;// Choose between AdMob or AmazonAds
		#endif
			
		desc.Android_AdMob_UnitID = "";// NOTE: You can use legacy (PublisherID) too, You MUST have this even for Testing!
		desc.Android_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.Android_AdMob_AdSize = Android_AdMob_AdSize.Banner_320x50;
			
		desc.Android_AmazonAds_ApplicationKey = "";
		desc.Android_AmazonAds_AdSize = Android_AmazonAds_AdSize.Wide_320x50;
		desc.Android_AmazonAds_AdGravity = AdGravity.BottomCenter;
		//desc.Android_AmazonAds_RefreshRate = 120;

		// create ad
		ad = AdManager.CreateAd(desc, adCreatedCallback);
	}

	private void adCreatedCallback(bool succeeded)
	{
		adStatus = succeeded ? "Ads Succeded" : "Ads Failed";
	}

	private void eventCallback(AdEvents adEvent, string eventMessage)
	{
		// NOTE: On BB10 these events never get called!
		switch (adEvent)
		{
			case AdEvents.Refreshed: adStatus = "Refreshed"; break;
			case AdEvents.Clicked: adStatus = "Clicked"; break;
			case AdEvents.Error: adStatus = "Error: " + eventMessage; break;
		}
	}

	void OnGUI()
	{
		GUI.matrix = Matrix4x4.identity;
		GUI.color = Color.white;

		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Banner Ads Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
		{
			gameObject.SetActive(false);
			Application.LoadLevel("MainDemo");
			return;
		}
		offset += 34;

		GUI.Label(new Rect(0, Screen.height/2, 256, 64), "Ad status: " + adStatus);
				
		// Manual Refresh does not work on (Apple iAd) or (BB10 Ads).
		if (GUI.Button(new Rect(0, offset, 128, 64), "Manual Refresh")) ad.Refresh();
		offset += 68;

		// Show / Hide Ad
		if (GUI.Button(new Rect(0, offset, 128, 64), "Show / Hide")) ad.Visible = !ad.Visible;

		// You can also manually draw GUI based Ads if you want to control GUI sort order
		//ad.Draw();
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
