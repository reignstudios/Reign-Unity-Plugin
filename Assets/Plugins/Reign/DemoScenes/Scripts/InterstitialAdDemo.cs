// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class InterstitialAdDemo : MonoBehaviour
{
	private static InterstitialAdDemo singleton;
	private static bool created, adIsCahced;
	private static InterstitialAd ad;
	public Button CacheAdButton, ShowAdButton, BackButton;
	public Text MessageText;

	void Start()
	{
		singleton = this;

		// bind button events
		CacheAdButton.Select();
		CacheAdButton.onClick.AddListener(cacheAdClicked);
		ShowAdButton.onClick.AddListener(showAdClicked);
		BackButton.onClick.AddListener(backClicked);

		// make sure we don't init the same Ad twice
		if (created) return;
		created = true;

		// create add
		var desc = new InterstitialAdDesc();

		// Global
		desc.Testing = true;
		desc.EventCallback = eventCallback;
		desc.UseClassicGUI = false;
		desc.GUIOverrideEnabled = false;
		desc.UnityUI_SortIndex = 1001;

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

	private void cacheAdClicked()
	{
		if (adIsCahced)
		{
			MessageText.text = "Ad already cached!";
			return;
		}

		// its a good idea to cache ads when the level starts, then show it when the level ends
		MessageText.text = "Caching Ad...";
		ad.Cache();
	}

	private void showAdClicked()
	{
		if (!adIsCahced)
		{
			MessageText.text = "Ad must be cached first!";
			return;
		}

		// NOTE: you must call "ad.Cache()" each time before you call "ad.Show()"
		adIsCahced = false;
		ad.Show();
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	private void createdCallback(bool success)
	{
		Debug.Log(success);
		if (success)
		{
			MessageText.text = "Ad created successfully!";
		}
		else
		{
			Debug.LogError("Failed to create InterstitialAd!");
			MessageText.text = "Failed to create InterstitialAd!";
		}
	}

	private static void eventCallback(InterstitialAdEvents adEvent, string eventMessage)
	{
		Debug.Log(adEvent);
		switch (adEvent)
		{
			 case InterstitialAdEvents.Error:
				Debug.LogError(eventMessage);
				singleton.MessageText.text = eventMessage;
				break;

			case InterstitialAdEvents.Canceled:
				singleton.MessageText.text = "Ad Canceled!";
				break;

			case InterstitialAdEvents.Clicked:
				singleton.MessageText.text = "Ad Clicked!";
				break;

			case InterstitialAdEvents.Cached:
				adIsCahced = true;
				singleton.MessageText.text = "Ad Cached!";
				break;

			case InterstitialAdEvents.Shown:
				singleton.MessageText.text = "Ad Shown!";
				break;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
