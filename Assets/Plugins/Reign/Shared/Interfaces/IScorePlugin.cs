using System;
using UnityEngine;

namespace Reign
{
	/// <summary>
	/// Score API types
	/// </summary>
	public enum ScoreAPIs
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// ReignScores
		/// </summary>
		ReignScores,

		/// <summary>
		/// GooglePlay
		/// </summary>
		GooglePlay,

		/// <summary>
		/// GameCircle
		/// </summary>
		GameCircle,

		/// <summary>
		/// GameCenter
		/// </summary>
		GameCenter
	}

	/// <summary>
	/// Leaderboard desc object
	/// </summary>
	public class LeaderboardDesc
	{
		/// <summary>
		/// ID value (Can be any unique string)
		/// </summary>
		public string ID;

		/// <summary>
		/// Name
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Desc
		/// </summary>
		public string Desc;

		/// <summary>
		/// ReignScores ID (Unique value)
		/// </summary>
		public Guid Editor_ReignScores_ID, Win32_ReignScores_ID, OSX_ReignScores_ID, Linux_ReignScores_ID, Web_ReignScores_ID;

		/// <summary>
		/// ReignScores ID (Unique value)
		/// </summary>
		public Guid Win8_ReignScores_ID, WP8_ReignScores_ID, BB10_ReignScores_ID, iOS_ReignScores_ID, Android_ReignScores_ID;

		/// <summary>
		/// GooglePlay ID (NOTE: Not name)
		/// </summary>
		public string Android_GooglePlay_ID;

		/// <summary>
		/// GameCircle ID (Unique Leaderboard ID)
		/// </summary>
		public string Android_GameCircle_ID;

		/// <summary>
		/// GameCenter ID (NOTE: Not name)
		/// </summary>
		public string iOS_GameCenter_ID;
	}
	
	/// <summary>
	/// Achievement desc object
	/// </summary>
	public class AchievementDesc
	{
		/// <summary>
		/// ID Value (Can be any unique string)
		/// </summary>
		public string ID;

		/// <summary>
		/// Name
		/// </summary>
		public string Name;

		/// <summary>
		/// Desc
		/// </summary>
		public string Desc;

		/// <summary>
		/// The number completed an achievement must until its unlocked. (Default = 100)
		/// </summary>
		public int PercentCompletedAtValue = 100;

		/// <summary>
		/// Mark true if you want to use Incremental/PercentCompleted achievement. (Default = false)
		/// </summary>
		public bool IsIncremental;

		// ReignScores
		/// <summary>
		/// ID value (Unique value)
		/// </summary>
		public Guid Editor_ReignScores_ID, Win32_ReignScores_ID, OSX_ReignScores_ID, Linux_ReignScores_ID, Web_ReignScores_ID;

		/// <summary>
		/// ID value (Unique value)
		/// </summary>
		public Guid Win8_ReignScores_ID, WP8_ReignScores_ID, BB10_ReignScores_ID, iOS_ReignScores_ID, Android_ReignScores_ID;

		/// <summary>
		/// GooglePlay ID (NOTE: Not name)
		/// </summary>
		public string Android_GooglePlay_ID;

		/// <summary>
		/// GameCircle ID (Unique Achievement ID)
		/// </summary>
		public string Android_GameCircle_ID;

		/// <summary>
		/// GameCenter ID (NOTE: Not name)
		/// </summary>
		public string iOS_GameCenter_ID;
	}

	/// <summary>
	/// Score desc object
	/// </summary>
	public class ScoreDesc
	{
		// api
		/// <summary>
		/// Score API type
		/// </summary>
		public ScoreAPIs Editor_ScoreAPI = ScoreAPIs.None, Win32_ScoreAPI = ScoreAPIs.None, OSX_ScoreAPI = ScoreAPIs.None, Linux_ScoreAPI = ScoreAPIs.None,
			Web_ScoreAPI = ScoreAPIs.None, Win8_ScoreAPI = ScoreAPIs.None, WP8_ScoreAPI = ScoreAPIs.None, BB10_ScoreAPI = ScoreAPIs.None, iOS_ScoreAPI = ScoreAPIs.None,
			Android_ScoreAPI = ScoreAPIs.None;
	
		// Reign Scores
		/// <summary>
		/// Set to true if you want the Reign UI to auto trigger on if the user is not authenticated. (NOTE: defaults to true)
		/// </summary>
		public bool ReignScores_AutoTriggerAuthenticateGUI = true;

		/// <summary>
		/// Login UI Title
		/// </summary>
		public string ReignScores_LoginTitle = "Login";
		
		/// <summary>
		/// Create User UI Title
		/// </summary>
		public string ReignScores_CreateUserTitle = "Create Account";

		/// <summary>
		/// Background UI texture. (Or you can set to NULL and use your own backgrounds)
		/// </summary>
		public Texture ReignScores_BackgroudTexture;
		
		/// <summary>
		/// Leaderboard background texture.
		/// </summary>
		public Texture ReignScores_TopScoreBoardTexture;
		
		/// <summary>
		/// Achievement background texture.
		/// </summary>
		public Texture ReignScores_AchievementBoardTexture;

		/// <summary>
		/// CloseBox texture
		/// </summary>
		public Texture ReignScores_TopScoreBoardButton_CloseNormal, ReignScores_TopScoreBoardButton_CloseHover;

		/// <summary>
		/// CloseBox button texture
		/// </summary>
		public Texture ReignScores_AchievementBoardButton_CloseNormal, ReignScores_AchievementBoardButton_CloseHover;

		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture ReignScores_TopScoreBoardButton_PrevNormal, ReignScores_TopScoreBoardButton_PrevHover, ReignScores_TopScoreBoardButton_NextNormal, ReignScores_TopScoreBoardButton_NextHover;
		
		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture ReignScores_AchievementBoardButton_PrevNormal, ReignScores_AchievementBoardButton_PrevHover, ReignScores_AchievementBoardButton_NextNormal, ReignScores_AchievementBoardButton_NextHover;
		
		/// <summary>
		/// All usernames will fit in this rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect ReignScores_TopScoreBoardFrame_Usernames;
		
		/// <summary>
		/// All score values will fit in this rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect ReignScores_TopScoreBoardFrame_Scores;
		
		/// <summary>
		/// Button rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect ReignScores_TopScoreBoardFrame_PrevButton, ReignScores_TopScoreBoardFrame_NextButton, ReignScores_TopScoreBoardFrame_CloseBox;

		/// <summary>
		/// All achievement names will fit in this rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect ReignScores_AchievementBoardFrame_Names;
		
		/// <summary>
		/// All achievement descs will fit in this rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect ReignScores_AchievementBoardFrame_Descs;
		
		/// <summary>
		/// Button rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect ReignScores_AchievementBoardFrame_PrevButton, ReignScores_AchievementBoardFrame_NextButton, ReignScores_AchievementBoardFrame_CloseBox;

		/// <summary>
		/// Board font size (Defaults to 12)
		/// </summary>
		public int ReignScores_TopScoreBoardFont_Size = 12, ReignScores_AchievementBoardFont_Size = 12;

		/// <summary>
		/// Board font color (Defaults to white)
		/// </summary>
		public Color ReignScores_TopScoreBoardFont_Color = Color.white, ReignScores_AchievementBoardFont_Color = Color.white;

		/// <summary>
		/// Amount to show on board (Defaults to 10)
		/// </summary>
		public int ReignScores_TopScoresToListPerPage = 10, ReignScores_AchievementsToListPerPage = 10;

		/// <summary>
		/// Set to true to visual see where your board rects are placed.
		/// </summary>
		public bool ReignScores_EnableTestRects;

		/// <summary>
		/// Set to your UI audio source
		/// </summary>
		public AudioSource ReignScores_AudioSource;

		/// <summary>
		/// Button click sound
		/// </summary>
		public AudioClip ReignScores_ButtonClick;

		/// <summary>
		/// This callback fires when a score needs to be formated (Such as converting an int to TimeSpan)
		/// </summary>
		public ScoreFormatCallbackMethod ReignScores_ScoreFormatCallback;

		/// <summary>
		/// Leaderboard descs
		/// </summary>
		public LeaderboardDesc[] LeaderboardDescs;

		/// <summary>
		/// Achievement descs
		/// </summary>
		public AchievementDesc[] AchievementDescs;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Editor_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Win32_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Linux_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string OSX_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Web_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Win8_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string WP8_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string BB10_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string iOS_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Android_ReignScores_GameID;
	}
	
	public class LeaderboardScore
	{
		public string UserName {get; private set;}
		public int Score {get; private set;}
		
		public LeaderboardScore(string userName, int score)
		{
			UserName = userName;
			Score = score;
		}
	}
	
	/// <summary>
	/// Achievement object
	/// </summary>
	public class Achievement
	{
		/// <summary>
		/// Use to check if item is achieved
		/// </summary>
		public bool IsAchieved {get; internal set;}

		/// <summary>
		/// Use to check percent complete of achievement
		/// </summary>
		public float PercentComplete {get; internal set;}

		/// <summary>
		/// Use to get achievement ID
		/// </summary>
		public string ID {get; private set;}

		/// <summary>
		/// Use to get achievement name
		/// </summary>
		public string Name {get; private set;}

		/// <summary>
		/// Use to get achievement desc
		/// </summary>
		public string Desc {get; private set;}

		/// <summary>
		/// Use to get achievement achieved image
		/// </summary>
		public Texture AchievedImage {get; private set;}

		/// <summary>
		/// Use to get achievement unachieved name
		/// </summary>
		public Texture UnachievedImage {get; private set;}
		
		/// <summary>
		/// Used to construct an achievement object
		/// </summary>
		/// <param name="isAchieved">Is achieved</param>
		/// <param name="id">ID</param>
		/// <param name="name">Name</param>
		/// <param name="desc">Desc</param>
		/// <param name="achievedImage">Achieved Image</param>
		/// <param name="unachievedImage">Unachieved Image</param>
		public Achievement(bool isAchieved, float percentComplete, string id, string name, string desc, Texture achievedImage, Texture unachievedImage)
		{
			IsAchieved = isAchieved;
			PercentComplete = percentComplete;
			ID = id;
			Name = name;
			Desc = desc;
			AchievedImage = achievedImage;
			UnachievedImage = unachievedImage;
		}
	}

	/// <summary>
	/// Used for creating api
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void CreatedScoreAPICallbackMethod(bool succeeded, string errorMessage);
	
	/// <summary>
	/// Used for authenticating user
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void AuthenticateCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for reporting a score
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ReportScoreCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for reporting a achievement
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ReportAchievementCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for requesting scores
	/// </summary>
	/// <param name="scores">Score objects</param>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void RequestScoresCallbackMethod(LeaderboardScore[] scores, bool succeeded, string errorMessage);

	/// <summary>
	/// Used for requesting achievements
	/// </summary>
	/// <param name="achievements">Achievement objects</param>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void RequestAchievementsCallbackMethod(Achievement[] achievements, bool succeeded, string errorMessage);

	/// <summary>
	/// Used for showing native views
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ShowNativeViewDoneCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for formating score values
	/// </summary>
	/// <param name="score">Score value to format</param>
	/// <param name="scoreValue">Output formated score value</param>
	public delegate void ScoreFormatCallbackMethod(int score, out string scoreValue);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base Score interface object
	/// </summary>
	public interface IScorePlugin
	{
		/// <summary>
		/// Use to check if a GUI system is in use
		/// </summary>
		bool PerformingGUIOperation {get;}

		/// <summary>
		/// Use to check if the user is authenticated
		/// </summary>
		bool IsAuthenticated {get;}

		/// <summary>
		/// Use to get the username or ID
		/// </summary>
		string Username {get;}
	
		/// <summary>
		/// Use to authenticate user
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to logout a user
		/// </summary>
		void Logout();

		/// <summary>
		/// Use to manualy login a user
		/// </summary>
		/// <param name="userID">Username to login</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualLogin(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to manualy create a user
		/// </summary>
		/// <param name="userID">Username to create</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualCreateUser(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to report a score
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="score">Score to report</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ReportScore(string leaderboardID, int score, ReportScoreCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to request scores
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="offset">Score load offset</param>
		/// <param name="range">Score count to load</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to report a achievement
		/// </summary>
		/// <param name="achievementID">Achievement ID</param>
		/// <param name="percentComplete">Percent Complete</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ReportAchievement(string achievementID, float percentComplete, ReportAchievementCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to request achievements
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void RequestAchievements(RequestAchievementsCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to show native score page
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to show native achievement page
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Used for UI events
		/// </summary>
		void OnGUI();

		/// <summary>
		/// Used for update events
		/// </summary>
		void Update();
	}
}
