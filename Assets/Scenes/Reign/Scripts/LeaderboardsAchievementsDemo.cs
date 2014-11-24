// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using Reign;

public class LeaderboardsAchievementsDemo : MonoBehaviour
{
	public static LeaderboardsAchievementsDemo Singleton;
	public Texture BackgroundTexture, ScoreBoardTexture, AchievementBoardTexture;
	public Texture CloseBoxNormal, CloseBoxHover;
	public Texture PrevNormal, PrevHover, NextNormal, NextHover;
	public Rect CloseBoxFrame, PrefFrame, NextFrame;
	public Rect TopScoreUsernameFrame, TopScoreFrame, AchievementNameFrame, AchievementDescFrame;
	public AudioClip ButtonClick;

	private bool modeSelected, loginMode;
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

		DontDestroyOnLoad(gameObject);// Make sure the start method never gets called more then once. So we don't init the same API twice.

		// Leaderboards ---------------------------
		var leaderboards = new LeaderboardDesc[1];
		var leaderboard = new LeaderboardDesc();
		leaderboards[0] = leaderboard;
		
		// Global
		leaderboard.ID = "Level1";// Any unique ID value you want
		leaderboard.Desc = "Level1 Desc...";// Any desc you want (NOTE: this only applies to APIs that don't have a native UI)

		// Editor
		leaderboard.Editor_Scoreoid_ID = 0;// Any unique index value

		// Win8
		leaderboard.Win8_Scoreoid_ID = 0;// Any unique index value

		// WP8
		leaderboard.WP8_Scoreoid_ID = 0;// Any unique index value

		// BB10
		leaderboard.BB10_Scoreoid_ID = 0;// Any unique index value
		leaderboard.BB10_Scoreloop_Mode = 0;// Each "mode" value can also be thought of as an ID

		// iOS
		leaderboard.iOS_Scoreoid_ID = 0;// Any unique index value
		leaderboard.iOS_GameCenter_ID = "";// Set to your GameCenter leaderboard ID

		// Android
		leaderboard.Android_Scoreoid_ID = 0;// Any unique index value
		leaderboard.Android_GooglePlay_ID = "";// Set to your GooglePlay leaderboard ID (Not Name)
		leaderboard.Android_GameCircle_ID = "";// Set to your GameCircle leaderboard ID (Not Name)


		// Achievements ---------------------------
		var achievements = new AchievementDesc[14];
		for (int i = 0; i != achievements.Length; ++i)
		{
			string value = "Achievement" + (i+1);
			var achievement = new AchievementDesc();
			achievements[i] = achievement;

			// Global
			achievement.ID = value;// Any unique ID value you want
			achievement.Name = value;// Any name you want (NOTE: this only applies to APIs that don't have a native UI)
			achievement.Desc = value + " Desc...";// Any desc you want (NOTE: this only applies to APIs that don't have a native UI)

			// Editor
			achievement.Editor_Scoreoid_ID = value;// Any unique index value

			// Win8
			achievement.Win8_Scoreoid_ID = value;// Any unique index value

			// WP8
			achievement.WP8_Scoreoid_ID = value;// Any unique index value

			// BB10
			achievement.BB10_Scoreoid_ID = value;// Any unique index value
			achievement.BB10_Scoreloop_ID = value.ToLower();// Must be lower case

			// iOS
			achievement.iOS_Scoreoid_ID = value;// Any unique index value
			achievement.iOS_GameCenter_ID = "";// Set to your GameCenter achievement ID

			// Android
			achievement.Android_Scoreoid_ID = value;// Any unique index value
			achievement.Android_GooglePlay_ID = "";// Set to your GooglePlay achievement ID (Not Name)
			achievement.Android_GameCircle_ID = "";// Set to your GameCircle achievement ID (Not Name)
		}

		// Desc ---------------------------
		string scoreoid_apiKey = "";
		string scoreoid_gameID = "";
		var desc = new ScoreDesc();
		desc.LeaderboardDescs = leaderboards;
		desc.AchievementDescs = achievements;

		// Global (Take note that you can adjust the built in Scoreoid UI below)
		desc.Scoreoid_EnableTestRects = false;
		desc.Scoreoid_AutoTriggerAuthenticateGUI = true;
		desc.Scoreoid_BackgroudTexture = BackgroundTexture;

		desc.Scoreoid_TopScoresToListPerPage = 10;
		desc.Scoreoid_TopScoreBoardTexture = ScoreBoardTexture;
		desc.Scoreoid_TopScoreBoardFrame_Usernames = TopScoreUsernameFrame;
		desc.Scoreoid_TopScoreBoardFrame_Scores = TopScoreFrame;
		desc.Scoreoid_TopScoreBoardFrame_CloseBox = CloseBoxFrame;
		desc.Scoreoid_TopScoreBoardButton_CloseNormal = CloseBoxNormal;
		desc.Scoreoid_TopScoreBoardButton_CloseHover = CloseBoxHover;
		desc.Scoreoid_TopScoreBoardFrame_PrevButton = PrefFrame;
		desc.Scoreoid_TopScoreBoardButton_PrevNormal = PrevNormal;
		desc.Scoreoid_TopScoreBoardButton_PrevHover = PrevHover;
		desc.Scoreoid_TopScoreBoardFrame_NextButton = NextFrame;
		desc.Scoreoid_TopScoreBoardButton_NextNormal = NextNormal;
		desc.Scoreoid_TopScoreBoardButton_NextHover = NextHover;
		desc.Scoreoid_TopScoreBoardFont_Size = 18;
		desc.Scoreoid_TopScoreBoardFont_Color = Color.white;

		desc.Scoreoid_AchievementsToListPerPage = 10;
		desc.Scoreoid_AchievementBoardTexture = AchievementBoardTexture;
		desc.Scoreoid_AchievementBoardFrame_Names = AchievementNameFrame;
		desc.Scoreoid_AchievementBoardFrame_Descs = AchievementDescFrame;
		desc.Scoreoid_AchievementBoardFrame_CloseBox = CloseBoxFrame;
		desc.Scoreoid_AchievementBoardButton_CloseNormal = CloseBoxNormal;
		desc.Scoreoid_AchievementBoardButton_CloseHover = CloseBoxHover;
		desc.Scoreoid_AchievementBoardFrame_PrevButton = PrefFrame;
		desc.Scoreoid_AchievementBoardButton_PrevNormal = PrevNormal;
		desc.Scoreoid_AchievementBoardButton_PrevHover = PrevHover;
		desc.Scoreoid_AchievementBoardFrame_NextButton = NextFrame;
		desc.Scoreoid_AchievementBoardButton_NextNormal = NextNormal;
		desc.Scoreoid_AchievementBoardButton_NextHover = NextHover;
		desc.Scoreoid_AchievementBoardFont_Size = 18;
		desc.Scoreoid_AchievementBoardFont_Color = Color.white;

		desc.Scoreoid_LoginTitle = "Login";
		desc.Scoreoid_CreateUserTitle = "Create Account";
		desc.Scoreoid_AudioSource = audio;
		desc.Scoreoid_ButtonClick = ButtonClick;
		desc.Scoreoid_ScoreFormatCallback = scoreFormatCallback;

		// Editor
		desc.Editor_ScoreAPI = ScoreAPIs.Scoreoid;
		desc.Editor_Scoreoid_APIKey = scoreoid_apiKey;
		desc.Editor_Scoreoid_GameID = scoreoid_gameID;

		// Win8
		desc.Win8_ScoreAPI = ScoreAPIs.Scoreoid;
		desc.Win8_Scoreoid_APIKey = scoreoid_apiKey;
		desc.Win8_Scoreoid_GameID = scoreoid_gameID;

		// WP8
		desc.WP8_ScoreAPI = ScoreAPIs.Scoreoid;
		desc.WP8_Scoreoid_APIKey = scoreoid_apiKey;
		desc.WP8_Scoreoid_GameID = scoreoid_gameID;

		// BB10
		desc.BB10_ScoreAPI = ScoreAPIs.Scoreloop;
		desc.BB10_Scoreloop_ID = "";
		desc.BB10_Scoreloop_Secret = "";
		desc.BB10_Scoreloop_Currency = "";

		desc.BB10_Scoreoid_APIKey = scoreoid_apiKey;
		desc.BB10_Scoreoid_GameID = scoreoid_gameID;

		// iOS
		desc.iOS_ScoreAPI = ScoreAPIs.GameCenter;
		desc.iOS_Scoreoid_APIKey = scoreoid_apiKey;
		desc.iOS_Scoreoid_GameID = scoreoid_gameID;

		// Android
		#if GOOGLEPLAY
		desc.Android_ScoreAPI = ScoreAPIs.GooglePlay;
		#elif AMAZON
		desc.Android_ScoreAPI = ScoreAPIs.GameCircle;
		#else
		desc.Android_ScoreAPI = ScoreAPIs.Scoreoid;
		#endif
		desc.Android_Scoreoid_APIKey = scoreoid_apiKey;
		desc.Android_Scoreoid_GameID = scoreoid_gameID;

		// init
		ScoreManager.Init(desc, createdCallback);
		ScoreManager.Authenticate(authenticateCallback);

		// <<< Scoreoid manual methods >>>
		//ScoreManager.RequestScores(...);
		//ScoreManager.RequestAchievements(...);
		//ScoreManager.ManualLogin(...);
		//ScoreManager.ManualCreateUser(...);
	}

	private void createdCallback(bool success, string errorMessage)
	{
		if (!success) Debug.LogError(errorMessage);
	}

	private void scoreFormatCallback(int score, out string scoreValue)
	{
		scoreValue = System.TimeSpan.FromSeconds(score).ToString();
	}

	private void authenticateCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Authenticated: " + succeeded);
		if (!succeeded && errorMessage != null) Debug.LogError(errorMessage);
	}

	void OnGUI()
	{
		// disable any unwanted gui if this is enabled to make room for the built-in Reign Scoreoid GUI.
		if (ScoreManager.PerformingGUIOperation) return;

		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Leaderboards & Achievements Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
		{
			gameObject.SetActive(false);
			Application.LoadLevel("MainDemo");
			return;
		}
		offset += 34;

		if (ScoreManager.IsAuthenticated)
		{
			GUI.Label(new Rect(0, offset, Screen.width, Screen.height/8), "Authenticated UserID: " + ScoreManager.UserID);

			// Show Leaderboards
			if (GUI.Button(new Rect(0, Screen.height-64, 256, 64), "Show Leaderboard Scores") || Input.GetKeyUp(KeyCode.L))
			{
				ScoreManager.ShowNativeScoresPage("Level1", showNativePageCallback);
			}

			// Show Achievements
			if (GUI.Button(new Rect(256, Screen.height-64, 256, 64), "Show Achievements") || Input.GetKeyUp(KeyCode.A))
			{
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
				string value = "Achievement" + 1;//Random.Range(1, 5);
				ScoreManager.ReportAchievement(value, reportAchievementCallback);
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
		Application.Quit();
	}
}
