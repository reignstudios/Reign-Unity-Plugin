// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using Reign;

public class MarketingDemo : MonoBehaviour
{
	GUIStyle uiStyle;

	void Start ()
	{
		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white},
		};
	}

	void OnGUI()
	{
		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Marketing Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back")) Application.LoadLevel("MainDemo");
		offset += 34;

		float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;
		if (GUI.Button(new Rect((Screen.width/2)-(64), (Screen.height/2)-(32*scale)+(64*scale)+offset, 128, 64*scale), "Review App"))
		{
			var desc = new MarketingDesc();
			desc.Editor_URL = "http://reign-studios.net/";// Editor: Any full URL
			desc.WinRT_PackageFamilyName = "41631Reign-Studios.CosmicPong_2wv2wvs0mpzqp";// WinRT: This is the "Package family name" that can be found in your "Package.appxmanifest".
			// WP8: Do nothing...
			desc.iOS_AppID = "547246359";// iOS: Pass in your AppID "xxxxxxxxx"
			desc.BB10_AppID = "49146902";// BB10: You pass in your AppID "xxxxxxxx".

			desc.Android_MarketingStore = MarketingStores.Amazon;
			desc.Android_GooglePlay_BundleID = "com.ReignStudios.CosmicPong";// Android GooglePlay: Pass in your bundle ID "com.Company.AppName"
			desc.Android_Amazon_BundleID = "com.reignstudios.cosmicpong";// Android Amazon: Pass in your bundle ID "com.Company.AppName"
			
			MarketingManager.OpenStoreForReview(desc);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
