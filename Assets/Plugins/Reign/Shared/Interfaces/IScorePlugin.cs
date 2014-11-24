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
		/// Scoreoid
		/// </summary>
		Scoreoid,

		/// <summary>
		/// Scoreloop
		/// </summary>
		Scoreloop,

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
		/// Desc (Only applies to platforms that dont have a native UI)
		/// </summary>
		public string Desc;

		/// <summary>
		/// Scoreoid ID (Unique index value)
		/// </summary>
		public int Editor_Scoreoid_ID, Win32_Scoreoid_ID, OSX_Scoreoid_ID, Linux_Scoreoid_ID, Web_Scoreoid_ID;

		/// <summary>
		/// Scoreoid ID (Unique index value)
		/// </summary>
		public int Win8_Scoreoid_ID, WP8_Scoreoid_ID, BB10_Scoreoid_ID, iOS_Scoreoid_ID, Android_Scoreoid_ID;

		/// <summary>
		/// Scoreloop Mode ID (Mode Index value)
		/// </summary>
		public int BB10_Scoreloop_Mode;

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
		/// Name (Only applies to platforms that dont have a native UI)
		/// </summary>
		public string Name;

		/// <summary>
		/// Desc (Only applies to platforms that dont have a native UI)
		/// </summary>
		public string Desc;

		// Scoreoid
		/// <summary>
		/// ID value (Normaly you keep this the same as ID)
		/// </summary>
		public string Editor_Scoreoid_ID, Win32_Scoreoid_ID, OSX_Scoreoid_ID, Linux_Scoreoid_ID, Web_Scoreoid_ID;

		/// <summary>
		/// ID value (Normaly you keep this the same as ID)
		/// </summary>
		public string Win8_Scoreoid_ID, WP8_Scoreoid_ID, BB10_Scoreoid_ID, iOS_Scoreoid_ID, Android_Scoreoid_ID;

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

		// Scoreloop
		/// <summary>
		/// ID value (Normaly you keep this the same as ID)
		/// NOTE: Must be lower case
		/// </summary>
		public string BB10_Scoreloop_ID;

		/// <summary>
		/// Achievement name override (Normaly you leave this null unless you are using your own UI)
		/// </summary>
		public string BB10_Scoreloop_NameOverride;
		
		/// <summary>
		/// Achievement desc override (Normaly you leave this null unless you are using your own UI)
		/// </summary>
		public string BB10_Scoreloop_DescOverride;
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
	
		// Scoreoid
		/// <summary>
		/// Set to true if you want the Reign UI to auto trigger on if the user is not authenticated. (NOTE: defaults to true)
		/// </summary>
		public bool Scoreoid_AutoTriggerAuthenticateGUI = true;

		/// <summary>
		/// Login UI Title
		/// </summary>
		public string Scoreoid_LoginTitle = "Login";
		
		/// <summary>
		/// Create User UI Title
		/// </summary>
		public string Scoreoid_CreateUserTitle = "Create Account";

		/// <summary>
		/// Background UI texture. (Or you can set to NULL and use your own backgrounds)
		/// </summary>
		public Texture Scoreoid_BackgroudTexture;
		
		/// <summary>
		/// Leaderboard background texture.
		/// </summary>
		public Texture Scoreoid_TopScoreBoardTexture;
		
		/// <summary>
		/// Achievement background texture.
		/// </summary>
		public Texture Scoreoid_AchievementBoardTexture;

		/// <summary>
		/// CloseBox texture
		/// </summary>
		public Texture Scoreoid_TopScoreBoardButton_CloseNormal, Scoreoid_TopScoreBoardButton_CloseHover;

		/// <summary>
		/// CloseBox button texture
		/// </summary>
		public Texture Scoreoid_AchievementBoardButton_CloseNormal, Scoreoid_AchievementBoardButton_CloseHover;

		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture Scoreoid_TopScoreBoardButton_PrevNormal, Scoreoid_TopScoreBoardButton_PrevHover, Scoreoid_TopScoreBoardButton_NextNormal, Scoreoid_TopScoreBoardButton_NextHover;
		
		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture Scoreoid_AchievementBoardButton_PrevNormal, Scoreoid_AchievementBoardButton_PrevHover, Scoreoid_AchievementBoardButton_NextNormal, Scoreoid_AchievementBoardButton_NextHover;
		
		/// <summary>
		/// All usernames will fit in this rect. (Auto scales to fit in Scoreoid_TopScoreBoardTexture)
		/// </summary>
		public Rect Scoreoid_TopScoreBoardFrame_Usernames;
		
		/// <summary>
		/// All score values will fit in this rect. (Auto scales to fit in Scoreoid_TopScoreBoardTexture)
		/// </summary>
		public Rect Scoreoid_TopScoreBoardFrame_Scores;
		
		/// <summary>
		/// Button rect. (Auto scales to fit in Scoreoid_TopScoreBoardTexture)
		/// </summary>
		public Rect Scoreoid_TopScoreBoardFrame_PrevButton, Scoreoid_TopScoreBoardFrame_NextButton, Scoreoid_TopScoreBoardFrame_CloseBox;

		/// <summary>
		/// All achievement names will fit in this rect. (Auto scales to fit in Scoreoid_AchievementBoardTexture)
		/// </summary>
		public Rect Scoreoid_AchievementBoardFrame_Names;
		
		/// <summary>
		/// All achievement descs will fit in this rect. (Auto scales to fit in Scoreoid_AchievementBoardTexture)
		/// </summary>
		public Rect Scoreoid_AchievementBoardFrame_Descs;
		
		/// <summary>
		/// Button rect. (Auto scales to fit in Scoreoid_AchievementBoardTexture)
		/// </summary>
		public Rect Scoreoid_AchievementBoardFrame_PrevButton, Scoreoid_AchievementBoardFrame_NextButton, Scoreoid_AchievementBoardFrame_CloseBox;

		/// <summary>
		/// Board font size (Defaults to 12)
		/// </summary>
		public int Scoreoid_TopScoreBoardFont_Size = 12, Scoreoid_AchievementBoardFont_Size = 12;

		/// <summary>
		/// Board font color (Defaults to white)
		/// </summary>
		public Color Scoreoid_TopScoreBoardFont_Color = Color.white, Scoreoid_AchievementBoardFont_Color = Color.white;

		/// <summary>
		/// Amount to show on board (Defaults to 10)
		/// </summary>
		public int Scoreoid_TopScoresToListPerPage = 10, Scoreoid_AchievementsToListPerPage = 10;

		/// <summary>
		/// Set to true to visual see where your board rects are placed.
		/// </summary>
		public bool Scoreoid_EnableTestRects;

		/// <summary>
		/// Set to your UI audio source
		/// </summary>
		public AudioSource Scoreoid_AudioSource;

		/// <summary>
		/// Button click sound
		/// </summary>
		public AudioClip Scoreoid_ButtonClick;

		/// <summary>
		/// This callback fires when a score needs to be formated (Such as converting an int to TimeSpan)
		/// </summary>
		public ScoreFormatCallbackMethod Scoreoid_ScoreFormatCallback;

		/// <summary>
		/// Leaderboard descs
		/// </summary>
		public LeaderboardDesc[] LeaderboardDescs;

		/// <summary>
		/// Achievement descs
		/// </summary>
		public AchievementDesc[] AchievementDescs;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Editor_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Editor_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Win32_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Win32_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Linux_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Linux_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string OSX_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string OSX_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Web_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Web_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Win8_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Win8_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string WP8_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string WP8_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string BB10_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string BB10_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string iOS_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string iOS_Scoreoid_GameID;

		/// <summary>
		/// Scoreoid API key
		/// </summary>
		public string Android_Scoreoid_APIKey;
		
		/// <summary>
		/// Scoreoid Game ID
		/// </summary>
		public string Android_Scoreoid_GameID;

		/// <summary>
		/// Scoreloop ID key
		/// </summary>
		public string BB10_Scoreloop_ID;
		
		/// <summary>
		/// Scoreloop Secret key
		/// </summary>
		public string BB10_Scoreloop_Secret;
		
		/// <summary>
		/// Scoreloop Currency type
		/// </summary>
		public string BB10_Scoreloop_Currency;
		
		/// <summary>
		/// Scoreloop version (Normaly leave to default)
		/// </summary>
		public string BB10_Scoreloop_Version = "1.0";
		
		/// <summary>
		/// Scoreloop Language (Normaly leave to default)
		/// </summary>
		public string BB10_Scoreloop_Language = "en";
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
		public bool IsAchieved {get; private set;}

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
		public Achievement(bool isAchieved, string id, string name, string desc, Texture achievedImage, Texture unachievedImage)
		{
			IsAchieved = isAchieved;
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
		string UserID {get;}
	
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
		/// <param name="userID">Username or UserID to login</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualLogin(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to manualy create a user
		/// </summary>
		/// <param name="userID">Username of UserID to create</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualCreateUser(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

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
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ReportAchievement(string achievementID, ReportAchievementCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to request achievements
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		void RequestAchievements(RequestAchievementsCallbackMethod callback);

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
		void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback);

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
