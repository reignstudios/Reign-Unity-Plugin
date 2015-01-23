// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections;
using Reign;

public class InterstitialAdDemo : MonoBehaviour
{
	public static InterstitialAdDemo Singleton;
	private InterstitialAd ad;
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

		DontDestroyOnLoad(gameObject);// Make sure the start method never gets called more then once. So we don't create the same Ad twice.

		var desc = new InterstitialAdDesc();

		// Global
		desc.Testing = true;
		desc.EventCallback = eventCallback;

		// WP8
		desc.WP8_AdAPI = InterstitialAdAPIs.AdMob;
		desc.WP8_AdMob_UnitID = "";// NOTE: Must set event for testing
			
		// iOS
		desc.iOS_AdAPI = InterstitialAdAPIs.AdMob;
		desc.iOS_AdMob_UnitID = "";// NOTE: Must set event for testing
			
		// Android
		#if AMAZON
		desc.Android_AdAPI = InterstitialAdAPIs.Amazon;
		#else
		desc.Android_AdAPI = InterstitialAdAPIs.AdMob;
		#endif
		desc.Android_AdMob_UnitID = "";// NOTE: Must set event for testing
		desc.Android_Amazon_ApplicationKey = "";// NOTE: Must set event for testing

		// create ad
		ad = InterstitialAdManager.CreateAd(desc, createdCallback);
	}

	private void createdCallback(bool success)
	{
		Debug.Log(success);
		if (!success) Debug.LogError("Failed to create InterstitialAd!");
	}

	private void eventCallback(InterstitialAdEvents adEvent, string eventMessage)
	{
		Debug.Log(adEvent);
		if (adEvent == InterstitialAdEvents.Error) Debug.LogError(eventMessage);
		if (adEvent == InterstitialAdEvents.Cached) ad.Show();
	}

	void OnGUI()
	{
		GUI.matrix = Matrix4x4.identity;
		GUI.color = Color.white;

		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Interstitial Ads Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
		{
			gameObject.SetActive(false);
			Application.LoadLevel("MainDemo");
			return;
		}
		offset += 34;

		if (GUI.Button(new Rect(0, offset, 128, 64), "Show Ad"))
		{
			ad.Cache();
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
