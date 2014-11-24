// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	/// <summary>
	/// Used to manage leaderboard and achievement APIs.
	/// </summary>
	public static class ScoreManager
	{
		/// <summary>
		/// Use to check if the API is doing GUI operations.
		/// NOTE: If Scoreoid, you should disable your UI to let the Reign UI take over.
		/// </summary>
		public static bool PerformingGUIOperation
		{
			get
			{
				if (plugin == null) return false;
				return plugin.PerformingGUIOperation;
			}
		}

		/// <summary>
		/// Use to check if the user is authenticated.
		/// </summary>
		public static bool IsAuthenticated
		{
			get
			{
				if (plugin == null) return false;
				return plugin.IsAuthenticated;
			}
		}

		/// <summary>
		/// Use to get the authenticated user ID.
		/// </summary>
		public static string UserID
		{
			get
			{
				if (plugin == null) return "???";
				return plugin.UserID;
			}
		}

		private static IScorePlugin plugin;

		private static bool waitingForOperation;
		private static AuthenticateCallbackMethod authenticateCallback;
		private static ReportScoreCallbackMethod reportScoreCallback;
		private static RequestScoresCallbackMethod requestScoresCallback;
		private static ReportAchievementCallbackMethod reportAchievementCallback;
		private static RequestAchievementsCallbackMethod requestAchievementsCallback;
		private static ShowNativeViewDoneCallbackMethod showNativeViewCallback;
		private static CreatedScoreAPICallbackMethod createdCallback;

		private static void createdCallbackAsyncStatic(bool succeeded, string errorMessage)
		{
			ReignServices.Singleton.StartCoroutine(createdCallbackDelay(succeeded, errorMessage));
		}

		private static IEnumerator createdCallbackDelay(bool succeeded, string errorMessage)
		{
			yield return null;
			if (createdCallback != null) createdCallback(succeeded, errorMessage);
		}

		/// <summary>
		/// Use to init a score API.
		/// </summary>
		/// <param name="desc">Score Desc.</param>
		public static void Init(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			ScoreManager.createdCallback = callback;
			ReignServices.CheckStatus();
			plugin = ScorePluginAPI.New(desc, createdCallbackAsyncStatic);
			ReignServices.AddService(update, onGUI, null);
		}

		private static void onGUI()
		{
			plugin.OnGUI();
		}

		private static void update()
		{
			plugin.Update();
		}

		private static void async_authenticateCallback(bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (authenticateCallback != null) authenticateCallback(succeeded, errorMessage);
		}

		private static void async_reportScoreCallback(bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (reportScoreCallback != null) reportScoreCallback(succeeded, errorMessage);
		}

		private static void async_requestScoresCallback(LeaderboardScore[] scores, bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (requestScoresCallback != null) requestScoresCallback(scores, succeeded, errorMessage);
		}

		private static void async_reportAchievementCallback(bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (reportAchievementCallback != null) reportAchievementCallback(succeeded, errorMessage);
		}

		private static void async_requestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (requestAchievementsCallback != null) requestAchievementsCallback(achievements, succeeded, errorMessage);
		}

		private static void async_showNativeViewCallback(bool succeeded, string errorMessage)
		{
			waitingForOperation = false;
			if (showNativeViewCallback != null) showNativeViewCallback(succeeded, errorMessage);
		}

		/// <summary>
		/// Use to authenticate the user.
		/// </summary>
		/// <param name="callback">The callback that fires when done.</param>
		public static void Authenticate(AuthenticateCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			authenticateCallback = callback;
			plugin.Authenticate(async_authenticateCallback, ReignServices.Singleton);
		}

		/// <summary>
		/// Log user out
		/// </summary>
		public static void Logout()
		{
			plugin.Logout();
		}

		/// <summary>
		/// Use to manualy login a user.
		/// NOTE: Only supports Scoreoid.
		/// </summary>
		/// <param name="userID">Username or UserID.</param>
		/// <param name="password">User Password.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ManualLogin(string userID, string password, AuthenticateCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			authenticateCallback = callback;
			plugin.ManualLogin(userID, password, async_authenticateCallback, ReignServices.Singleton);
		}

		/// <summary>
		/// Use to manualy create a user.
		/// NOTE: Only supports Scoreoid.
		/// </summary>
		/// <param name="userID">Username or UserID.</param>
		/// <param name="password">User Password.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ManualCreateUser(string userID, string password, AuthenticateCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			authenticateCallback = callback;
			plugin.ManualCreateUser(userID, password, async_authenticateCallback, ReignServices.Singleton);
		}
	
		/// <summary>
		/// Use to report a score.
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID.</param>
		/// <param name="score">Score to report.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ReportScore(string leaderboardID, int score, ReportScoreCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			reportScoreCallback = callback;
			plugin.ReportScore(leaderboardID, score, async_reportScoreCallback, ReignServices.Singleton);
		}
		
		/// <summary>
		/// Use to request scores.
		/// NOTE: Only supports Scoreoid.
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID.</param>
		/// <param name="offset">Item offset.</param>
		/// <param name="range">Item count to load.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			requestScoresCallback = callback;
			plugin.RequestScores(leaderboardID, offset, range, async_requestScoresCallback, ReignServices.Singleton);
		}
		
		/// <summary>
		/// Use to report a achievement.
		/// </summary>
		/// <param name="achievementID">Achievement ID.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ReportAchievement(string achievementID, ReportAchievementCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			reportAchievementCallback = callback;
			plugin.ReportAchievement(achievementID, async_reportAchievementCallback, ReignServices.Singleton);
		}
		
		/// <summary>
		/// Use to request achievements.
		/// NOTE: Only supports Scoreoid.
		/// </summary>
		/// <param name="callback">The callback that fires when done.</param>
		public static void RequestAchievements(RequestAchievementsCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			requestAchievementsCallback = callback;
			plugin.RequestAchievements(async_requestAchievementsCallback);
		}

		/// <summary>
		/// Use to show native score UI view.
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			showNativeViewCallback = callback;
			plugin.ShowNativeScoresPage(leaderboardID, async_showNativeViewCallback, ReignServices.Singleton);
		}

		/// <summary>
		/// Use to show native achievement UI view.
		/// </summary>
		/// <param name="callback">The callback that fires when done.</param>
		public static void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			if (waitingForOperation)
			{
				Debug.LogError("Must wait for last Score operation to complete.");
				return;
			}

			waitingForOperation = true;
			showNativeViewCallback = callback;
			plugin.ShowNativeAchievementsPage(async_showNativeViewCallback);
		}
	}
}

namespace Reign.Plugin
{
	/// <summary>
	/// Dumy object.
	/// </summary>
	public class Dumy_ScorePluginPlugin : IScorePlugin
	{
		public bool PerformingGUIOperation {get; private set;}
		public bool IsAuthenticated {get; private set;}
		public string UserID {get; private set;}

		/// <summary>
		/// Dumy constructor.
		/// </summary>
		/// <param name="desc">Score desc.</param>
		public Dumy_ScorePluginPlugin(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			PerformingGUIOperation = false;
			IsAuthenticated = false;
			UserID = "???";
			if (callback != null) callback(false, "Dumy Score object");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Logout()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="password"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void ManualCreateUser(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="password"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void ManualLogin(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="achievementID"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void ReportAchievement(string achievementID, ReportAchievementCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="leaderboardID"></param>
		/// <param name="score"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void ReportScore(string leaderboardID, int score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="callback"></param>
		public void RequestAchievements(RequestAchievementsCallbackMethod callback)
		{
			if (callback != null) callback(null, false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="leaderboardID"></param>
		/// <param name="offset"></param>
		/// <param name="range"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(null, false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="callback"></param>
		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="leaderboardID"></param>
		/// <param name="callback"></param>
		/// <param name="services"></param>
		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Dumy Score Obj");
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void OnGUI()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Update()
		{
			// do nothing...
		}
	}

	/// <summary>
	/// Use to create a platform specific interface.
	/// </summary>
	static class ScorePluginAPI
	{
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="desc">Score desc.</param>
		/// <returns>Returns Score plugin interface</returns>
		public static IScorePlugin New(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			#if DISABLE_REIGN
			return new Dumy_ScorePluginPlugin(desc, callback);
			#elif UNITY_EDITOR
			if (desc.Editor_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.Editor_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported Editor_ScoreAPI: " + desc.Editor_ScoreAPI);
			#elif UNITY_WP8
			if (desc.WP8_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.WP8_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported WP8_ScoreAPI: " + desc.WP8_ScoreAPI);
			#elif UNITY_METRO
			if (desc.Win8_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.Win8_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported Win8_ScoreAPI: " + desc.Win8_ScoreAPI);
			#elif UNITY_ANDROID
			if (desc.Android_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.Android_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else if (desc.Android_ScoreAPI == ScoreAPIs.GooglePlay) return new GooglePlay_ScorePlugin(desc, callback);
			else if (desc.Android_ScoreAPI == ScoreAPIs.GameCircle) return new Amazon_GameCircle_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported Android_ScoreAPI: " + desc.Android_ScoreAPI);
			#elif UNITY_IOS
			if (desc.iOS_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.iOS_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else if (desc.iOS_ScoreAPI == ScoreAPIs.GameCenter) return new GameCenter_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported iOS_ScoreAPI: " + desc.iOS_ScoreAPI);
			#elif UNITY_BB10
			if (desc.BB10_ScoreAPI == ScoreAPIs.None) return new Dumy_ScorePluginPlugin(desc, callback);
			else if (desc.BB10_ScoreAPI == ScoreAPIs.Scoreoid) return new Scoreoid_ScorePlugin(desc, callback);
			else if (desc.BB10_ScoreAPI == ScoreAPIs.Scoreloop) return new Scoreloop_ScorePlugin(desc, callback);
			else throw new Exception("Unsuported BB10_ScoreAPI: " + desc.BB10_ScoreAPI);
			#else
			return new Dumy_ScorePluginPlugin(desc, callback);
			#endif
		}
	}
}