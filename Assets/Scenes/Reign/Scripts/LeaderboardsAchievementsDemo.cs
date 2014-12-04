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
		leaderboard.Editor_ReignScores_ID = System.Guid.Empty;// Any unique value

		// Win8
		leaderboard.Win8_ReignScores_ID = System.Guid.Empty;// Any unique value

		// WP8
		leaderboard.WP8_ReignScores_ID = System.Guid.Empty;// Any unique value

		// BB10
		leaderboard.BB10_ReignScores_ID = System.Guid.Empty;// Any unique value

		// iOS
		leaderboard.iOS_ReignScores_ID = System.Guid.Empty;// Any unique value
		leaderboard.iOS_GameCenter_ID = "Level1";// Set to your GameCenter leaderboard ID

		// Android
		leaderboard.Android_ReignScores_ID = System.Guid.Empty;// Any unique value
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
			achievement.Editor_ReignScores_ID = System.Guid.Empty;// Any unique value

			// Win8
			achievement.Win8_ReignScores_ID = System.Guid.Empty;// Any unique value

			// WP8
			achievement.WP8_ReignScores_ID = System.Guid.Empty;// Any unique value

			// BB10
			achievement.BB10_ReignScores_ID = System.Guid.Empty;// Any unique value

			// iOS
			achievement.iOS_ReignScores_ID = System.Guid.Empty;// Any unique index value
			achievement.iOS_GameCenter_ID = "Achievement1";// Set to your GameCenter achievement ID

			// Android
			achievement.Android_ReignScores_ID = System.Guid.Empty;// Any unique value
			achievement.Android_GooglePlay_ID = "";// Set to your GooglePlay achievement ID (Not Name)
			achievement.Android_GameCircle_ID = "";// Set to your GameCircle achievement ID (Not Name)
		}

		// Desc ---------------------------
		string reignScores_gameID = "";
		var desc = new ScoreDesc();
		desc.LeaderboardDescs = leaderboards;
		desc.AchievementDescs = achievements;

		// Global (Take note that you can adjust the built in ReignScores UI below)
		desc.ReignScores_EnableTestRects = false;
		desc.ReignScores_AutoTriggerAuthenticateGUI = true;
		desc.ReignScores_BackgroudTexture = BackgroundTexture;

		desc.ReignScores_TopScoresToListPerPage = 10;
		desc.ReignScores_TopScoreBoardTexture = ScoreBoardTexture;
		desc.ReignScores_TopScoreBoardFrame_Usernames = TopScoreUsernameFrame;
		desc.ReignScores_TopScoreBoardFrame_Scores = TopScoreFrame;
		desc.ReignScores_TopScoreBoardFrame_CloseBox = CloseBoxFrame;
		desc.ReignScores_TopScoreBoardButton_CloseNormal = CloseBoxNormal;
		desc.ReignScores_TopScoreBoardButton_CloseHover = CloseBoxHover;
		desc.ReignScores_TopScoreBoardFrame_PrevButton = PrefFrame;
		desc.ReignScores_TopScoreBoardButton_PrevNormal = PrevNormal;
		desc.ReignScores_TopScoreBoardButton_PrevHover = PrevHover;
		desc.ReignScores_TopScoreBoardFrame_NextButton = NextFrame;
		desc.ReignScores_TopScoreBoardButton_NextNormal = NextNormal;
		desc.ReignScores_TopScoreBoardButton_NextHover = NextHover;
		desc.ReignScores_TopScoreBoardFont_Size = 18;
		desc.ReignScores_TopScoreBoardFont_Color = Color.white;

		desc.ReignScores_AchievementsToListPerPage = 10;
		desc.ReignScores_AchievementBoardTexture = AchievementBoardTexture;
		desc.ReignScores_AchievementBoardFrame_Names = AchievementNameFrame;
		desc.ReignScores_AchievementBoardFrame_Descs = AchievementDescFrame;
		desc.ReignScores_AchievementBoardFrame_CloseBox = CloseBoxFrame;
		desc.ReignScores_AchievementBoardButton_CloseNormal = CloseBoxNormal;
		desc.ReignScores_AchievementBoardButton_CloseHover = CloseBoxHover;
		desc.ReignScores_AchievementBoardFrame_PrevButton = PrefFrame;
		desc.ReignScores_AchievementBoardButton_PrevNormal = PrevNormal;
		desc.ReignScores_AchievementBoardButton_PrevHover = PrevHover;
		desc.ReignScores_AchievementBoardFrame_NextButton = NextFrame;
		desc.ReignScores_AchievementBoardButton_NextNormal = NextNormal;
		desc.ReignScores_AchievementBoardButton_NextHover = NextHover;
		desc.ReignScores_AchievementBoardFont_Size = 18;
		desc.ReignScores_AchievementBoardFont_Color = Color.white;

		desc.ReignScores_LoginTitle = "Login";
		desc.ReignScores_CreateUserTitle = "Create Account";
		desc.ReignScores_AudioSource = audio;
		desc.ReignScores_ButtonClick = ButtonClick;
		desc.ReignScores_ScoreFormatCallback = scoreFormatCallback;

		// Editor
		desc.Editor_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Editor_ReignScores_GameID = reignScores_gameID;

		// Win8
		desc.Win8_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Win8_ReignScores_GameID = reignScores_gameID;

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

		// init
		ScoreManager.Init(desc, createdCallback);
		ScoreManager.Authenticate(authenticateCallback);

		// <<< ReignScores manual methods >>>
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
		// disable any unwanted gui if this is enabled to make room for the built-in Reign ReignScores GUI.
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
			GUI.Label(new Rect(0, offset, Screen.width, Screen.height/8), "Authenticated Username: " + ScoreManager.Username);

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
