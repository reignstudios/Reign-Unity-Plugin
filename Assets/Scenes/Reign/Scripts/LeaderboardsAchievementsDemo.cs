// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using Reign;

public class LeaderboardsAchievementsDemo : MonoBehaviour
{
	public static LeaderboardsAchievementsDemo Singleton;
	public GameObject ReignScores_Renderer;

	private bool disableUI;
	GUIStyle uiStyle;

	void Start()
	{
		if (Singleton != null)
		{
			Destroy(gameObject);
			return;
		}
		Singleton = this;
		DontDestroyOnLoad(gameObject);// Make sure the start method never gets called more then once. So we don't init the same API twice.

		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white},
		};

		// Leaderboards ---------------------------
		var leaderboards = new LeaderboardDesc[1];
		var leaderboard = new LeaderboardDesc();
		leaderboards[0] = leaderboard;
		var leaderboardID = new System.Guid("f55e3800-eacd-4728-ae4f-31b00aaa63bf");
		leaderboard.ReignScores_SortOrder = LeaderboardSortOrders.Ascending;
		
		// Global
		leaderboard.ID = "Level1";// Any unique ID value you want
		leaderboard.Desc = "Level1 Desc...";// Any desc you want

		// Editor
		leaderboard.Editor_ReignScores_ID = leaderboardID;// Any unique value

		// WinRT
		leaderboard.WinRT_ReignScores_ID = leaderboardID;// Any unique value

		// WP8
		leaderboard.WP8_ReignScores_ID = leaderboardID;// Any unique value

		// BB10
		leaderboard.BB10_ReignScores_ID = leaderboardID;// Any unique value

		// iOS
		leaderboard.iOS_ReignScores_ID = leaderboardID;// Any unique value
		leaderboard.iOS_GameCenter_ID = "";// Set to your GameCenter leaderboard ID

		// Android
		leaderboard.Android_ReignScores_ID = leaderboardID;// Any unique value
		leaderboard.Android_GooglePlay_ID = "";// Set to your GooglePlay leaderboard ID (Not Name)
		leaderboard.Android_GameCircle_ID = "";// Set to your GameCircle leaderboard ID (Not Name)

		// Win32
		leaderboard.Win32_ReignScores_ID = leaderboardID;// Any unique value

		// OSX
		leaderboard.OSX_ReignScores_ID = leaderboardID;// Any unique value

		// Linux
		leaderboard.Linux_ReignScores_ID = leaderboardID;// Any unique value


		// Achievements ---------------------------
		var achievements = new AchievementDesc[5];
		var achievement = new AchievementDesc();
		achievements[0] = achievement;
		var achievementID = new System.Guid("352ce53d-142f-4a10-a4fb-804ad38be879");

		// Global
		achievement.ID = "Achievement1";// Any unique ID value you want
		achievement.Name = "Achievement1";// Any name you want
		achievement.Desc = "Achievement1 Desc...";// Any desc you want

		// When you report an achievement you pass a PercentComplete value.
		// Example: This allows you to change that ratio to something like (0-1000) before the achievement is unlocked.
		achievement.PercentCompletedAtValue = 100;// NOTE: For GooglePlay you must match this value in the developer dashboard under "How many steps are needed?" option.

		// Mark if you want Achievement to use PercentCompleted value or not.
		// Marking this true will make the "PercentComplete" value irrelevant.
		achievement.IsIncremental = true;

		// Editor
		achievement.Editor_ReignScores_ID = achievementID;// Any unique value

		// WinRT
		achievement.WinRT_ReignScores_ID = achievementID;// Any unique value

		// WP8
		achievement.WP8_ReignScores_ID = achievementID;// Any unique value

		// BB10
		achievement.BB10_ReignScores_ID = achievementID;// Any unique value

		// iOS
		achievement.iOS_ReignScores_ID = achievementID;// Any unique index value
		achievement.iOS_GameCenter_ID = "";// Set to your GameCenter achievement ID

		// Android
		achievement.Android_ReignScores_ID = achievementID;// Any unique value
		achievement.Android_GooglePlay_ID = "";// Set to your GooglePlay achievement ID (Not Name)
		achievement.Android_GameCircle_ID = "";// Set to your GameCircle achievement ID (Not Name)

		// Win32
		achievement.Win32_ReignScores_ID = achievementID;// Any unique value

		// OSX
		achievement.OSX_ReignScores_ID = achievementID;// Any unique value

		// Linux
		achievement.Linux_ReignScores_ID = achievementID;// Any unique value

		// Add other achievements...
		for (int i = 1; i != achievements.Length; ++i)
		{
			achievement = new AchievementDesc();
			achievements[i] = achievement;
			achievement.ID = "Achievement" + (i+1);
			achievement.Name = "Achievement_TODO" + (i+1);
			achievement.Desc = "Achievement_TODO Desc" + (i+1);

			achievementID = System.Guid.Empty;
			achievement.Editor_ReignScores_ID = achievementID;
			achievement.WinRT_ReignScores_ID = achievementID;
			achievement.WP8_ReignScores_ID = achievementID;
			achievement.BB10_ReignScores_ID = achievementID;
			achievement.iOS_ReignScores_ID = achievementID;
			achievement.Android_ReignScores_ID = achievementID;
			achievement.Win32_ReignScores_ID = achievementID;
			achievement.OSX_ReignScores_ID = achievementID;
			achievement.Linux_ReignScores_ID = achievementID;
		}

		// Desc ---------------------------
		const string reignScores_gameID = "B2A24047-0487-41C4-B151-0F175BB54D0E";// Get this ID from the Reign-Scores Console.
		var desc = new ScoreDesc();
		desc.ReignScores_UI = ReignScores_Renderer.GetComponent<MonoBehaviour>() as IScores_UI;
		desc.ReignScores_UI.ScoreFormatCallback += scoreFormatCallback;
		desc.ReignScores_ServicesURL = "http://localhost:5537/Services/";// Set to your server!
		desc.ReignScores_GameKey = "04E0676D-AAF8-4836-A584-DE0C1D618D84";// Set to your servers game_api_key!
		desc.ReignScores_UserKey = "CE8E55E1-F383-4F05-9388-5C89F27B7FF2";// Set to your servers user_api_key!
		desc.LeaderboardDescs = leaderboards;
		desc.AchievementDescs = achievements;

		// Editor
		desc.Editor_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Editor_ReignScores_GameID = reignScores_gameID;

		// WinRT
		desc.WinRT_ScoreAPI = ScoreAPIs.ReignScores;
		desc.WinRT_ReignScores_GameID = reignScores_gameID;

		// WP8
		desc.WP8_ScoreAPI = ScoreAPIs.ReignScores;
		desc.WP8_ReignScores_GameID = reignScores_gameID;

		// BB10
		desc.BB10_ScoreAPI = ScoreAPIs.ReignScores;
		desc.BB10_ReignScores_GameID = reignScores_gameID;

		// iOS
		desc.iOS_ScoreAPI = ScoreAPIs.GameCenter;
		desc.iOS_ReignScores_GameID = reignScores_gameID;

		// Android
		#if GOOGLEPLAY
		desc.Android_ScoreAPI = ScoreAPIs.GooglePlay;
		#elif AMAZON
		desc.Android_ScoreAPI = ScoreAPIs.GameCircle;
		#else
		desc.Android_ScoreAPI = ScoreAPIs.ReignScores;
		#endif
		desc.Android_ReignScores_GameID = reignScores_gameID;

		// Win32
		desc.Win32_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Win32_ReignScores_GameID = reignScores_gameID;

		// OSX
		desc.OSX_ScoreAPI = ScoreAPIs.ReignScores;
		desc.OSX_ReignScores_GameID = reignScores_gameID;

		// Linux
		desc.Linux_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Linux_ReignScores_GameID = reignScores_gameID;

		// init
		ScoreManager.Init(desc, createdCallback);

		// <<< ReignScores manual methods >>>
		//ScoreManager.RequestScores(...);
		//ScoreManager.RequestAchievements(...);
		//ScoreManager.ManualLogin(...);
		//ScoreManager.ManualCreateUser(...);
	}

	private void createdCallback(bool success, string errorMessage)
	{
		if (!success) Debug.LogError(errorMessage);
		else ScoreManager.Authenticate(authenticateCallback);
	}

	private void scoreFormatCallback(int score, out string scoreValue)
	{
		scoreValue = System.TimeSpan.FromSeconds(score).ToString();
	}

	private void authenticateCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Authenticated: " + succeeded);
		if (!succeeded && errorMessage != null) Debug.LogError(errorMessage);
		if (succeeded) ScoreManager.RequestAchievements(requestAchievementsCallback);
	}

	private void requestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
	{
		if (succeeded)
		{
			Debug.Log("Got Achievement count: " + achievements.Length);
			foreach (var achievement in achievements)
			{
				Debug.Log(string.Format("Achievement {0} PercentCompleted {1}", achievement.ID, achievement.PercentComplete));
			}
		}
		else
		{
			Debug.LogError("Request Achievements Error: " + errorMessage);
		}
	}

	void OnGUI()
	{
		if (ScoreManager.IsAuthenticated && !disableUI)
		{
			float offset = 0;
			GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Leaderboards & Achievements Demo >>", uiStyle);
			if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
			{
				gameObject.SetActive(false);
				Application.LoadLevel("MainDemo");
				return;
			}
			offset += 34;

			GUI.Label(new Rect(0, offset, Screen.width, Screen.height/8), "Authenticated Username: " + ScoreManager.Username);

			// Show Leaderboards
			if (GUI.Button(new Rect(0, Screen.height-64, 256, 64), "Show Leaderboard Scores") || Input.GetKeyUp(KeyCode.L))
			{
				disableUI = true;
				ScoreManager.ShowNativeScoresPage("Level1", showNativePageCallback);
			}

			// Show Achievements
			if (GUI.Button(new Rect(256, Screen.height-64, 256, 64), "Show Achievements") || Input.GetKeyUp(KeyCode.A))
			{
				disableUI = true;
				ScoreManager.ShowNativeAchievementsPage(showNativePageCallback);
			}

			// Report Scores
			if (GUI.Button(new Rect(Screen.width-256, offset, 256, 64), "Report Random Score") || Input.GetKeyUp(KeyCode.S))
			{
				ScoreManager.ReportScore("Level1", Random.Range(0, 500), reportScoreCallback);
			}

			// Report Achievements
			if (GUI.Button(new Rect(Screen.width-256, 64+offset, 256, 64), "Report Random Achievement") || Input.GetKeyUp(KeyCode.R))
			{
				string value = "Achievement" + 1;
				ScoreManager.ReportAchievement(value, 100, reportAchievementCallback);
			}

			// Logout (NOTE: Some platforms do not support this!)
			if (GUI.Button(new Rect((Screen.width/2)-(256/2), (Screen.height/2)-(64/2)+offset, 256, 64), "Logout") || Input.GetKeyUp(KeyCode.O))
			{
				ScoreManager.Logout();
			}
		}
		else
		{
			GUI.Label(new Rect(0, 0, 256, 64), "Not Authenticated!");
		}
	}

	void showNativePageCallback(bool succeeded, string errorMessage)
	{
		disableUI = false;
		Debug.Log("Show Native Page: " + succeeded);
		if (!succeeded) Debug.LogError(errorMessage);
	}

	void reportScoreCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Report Score Done: " + succeeded);
		if (!succeeded) Debug.LogError(errorMessage);
	}

	void reportAchievementCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Report Achievement Done: " + succeeded);
		if (!succeeded) Debug.LogError(errorMessage);
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
