using UnityEngine;
using System.Collections;

public class MainDemo : MonoBehaviour
{
	GUIStyle uiStyle;

	void Start ()
	{
		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white}
		};

		const string warning = "NOTE: Make sure to add all the Reign Demo projects you wish to test in the 'Build Settings' window!\nThis makes for easy device testing.";
		Debug.Log(warning);
		Debug.LogWarning(warning);
	}

	void OnGUI()
	{
		float offset = 0;
		if (GUI.Button(new Rect(Screen.width-128, offset, 128, 32), "Clear PlayerPrefs"))
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs Cleared!");
		}
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Reign Plugin Demos >>", uiStyle);

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Message Boxes")) Application.LoadLevel("MessageBoxDemo");

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Send Email")) Application.LoadLevel("EmailDemo");

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Data Streams")) Application.LoadLevel("StreamsDemo");

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Marketing")) Application.LoadLevel("MarketingDemo");

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Ads"))
		{
			if (AdsDemo.Singleton != null) AdsDemo.Singleton.gameObject.SetActive(true);
			Application.LoadLevel("AdsDemo");
		}

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "Interstitial Ads"))
		{
			if (InterstitialAdDemo.Singleton != null) InterstitialAdDemo.Singleton.gameObject.SetActive(true);
			Application.LoadLevel("InterstitialAdDemo");
		}

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 148, 32), "IAP"))
		{
			if (InAppPurchaseDemo.Singleton != null) InAppPurchaseDemo.Singleton.gameObject.SetActive(true);
			Application.LoadLevel("InAppPurchaseDemo");
		}

		offset += 34;
		if (GUI.Button(new Rect(20, offset, 200, 32), "Leaderboards & Achievements"))
		{
			if (LeaderboardsAchievementsDemo.Singleton != null) LeaderboardsAchievementsDemo.Singleton.gameObject.SetActive(true);
			Application.LoadLevel("LeaderboardsAchievementsDemo");
		}
	}
}
