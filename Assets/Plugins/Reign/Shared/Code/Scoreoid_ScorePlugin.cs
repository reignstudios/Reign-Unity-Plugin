using System;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace Reign.Plugin
{
	namespace ScoreoidXML
	{
		public class playerScore
		{
			[XmlAttribute("score")] public int score;
			[XmlAttribute("platform")] public string platform;
			[XmlAttribute("created")] public string created;
		}

		public class player
		{
			[XmlAttribute("username")] public string username;
			[XmlAttribute("achievements")] public string achievements;
			[XmlElement("score")] public playerScore score;
		}

		[XmlRoot("players")]
		public class players
		{
			[XmlElement("player")] public player[] player;
		}

		[XmlRoot("score")]
		public class score
		{
			[XmlText] public string Content;
		}

		[XmlRoot("scores")]
		public class scores
		{
			[XmlElement("player")] public player[] player;
		}

		[XmlRoot("error")]
		public class error
		{
			[XmlText] public string Content;
		}

		[XmlRoot("success")]
		public class success
		{
			[XmlText] public string Content;
		}
	}

	enum Scoreoid_GuiModes
	{
		None,
		Login,
		CreateUser,
		LoggingIn,
		CreatingUser,
		ShowingScores,
		ShowingAchievements,
		LoadingScores,
		LoadingAchievements
	}

	public class Scoreoid_ScorePlugin : IScorePlugin
	{
		private const string scoreoidURL = "https://api.scoreoid.com/v1/";

		public bool PerformingGUIOperation {get; private set;}
		private Scoreoid_GuiModes guiMode = Scoreoid_GuiModes.None;
		private AuthenticateCallbackMethod guiAuthenticateCallback;
		private MonoBehaviour guiServices;

		public bool IsAuthenticated {get; private set;}
		public string UserID {get; private set;}

		private ScoreDesc desc;
		private string apiKey, gameID, achievementString;
		private Achievement[] achievements;
	
		public Scoreoid_ScorePlugin(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			this.desc = desc;
			#if UNITY_EDITOR
			apiKey = desc.Editor_Scoreoid_APIKey;
			gameID = desc.Editor_Scoreoid_GameID;
			#elif UNITY_STANDALONE_WIN
			apiKey = desc.Win32_Scoreoid_APIKey;
			gameID = desc.Win32_Scoreoid_GameID;
			#elif UNITY_STANDALONE_OSX
			apiKey = desc.OSX_Scoreoid_APIKey;
			gameID = desc.OSX_Scoreoid_GameID;
			#elif UNITY_STANDALONE_LINUX
			apiKey = desc.Linux_Scoreoid_APIKey;
			gameID = desc.Linux_Scoreoid_GameID;
			#elif UNITY_WEBPLAYER
			apiKey = desc.Web_Scoreoid_APIKey;
			gameID = desc.Web_Scoreoid_GameID;
			#elif UNITY_METRO
			apiKey = desc.Win8_Scoreoid_APIKey;
			gameID = desc.Win8_Scoreoid_GameID;
			#elif UNITY_WP8
			apiKey = desc.WP8_Scoreoid_APIKey;
			gameID = desc.WP8_Scoreoid_GameID;
			#elif UNITY_BLACKBERRY
			apiKey = desc.BB10_Scoreoid_APIKey;
			gameID = desc.BB10_Scoreoid_GameID;
			#elif UNITY_IPHONE
			apiKey = desc.iOS_Scoreoid_APIKey;
			gameID = desc.iOS_Scoreoid_GameID;
			#elif UNITY_ANDROID
			apiKey = desc.Android_Scoreoid_APIKey;
			gameID = desc.Android_Scoreoid_GameID;
			#endif

			if (callback != null) callback(true, null);
		}

		private static byte[] getBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private bool checkError(WWW www, out string errorMessage)
		{
			errorMessage = null;
			try
			{
				var xml = new XmlSerializer(typeof(ScoreoidXML.error));
				using (var data = new MemoryStream(www.bytes))
				{
					var error = (ScoreoidXML.error)xml.Deserialize(data);
					errorMessage = error.Content;
					Debug.LogError("Scoreoid error: " + error.Content);
				}
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
				return false;
			}

			return true;
		}

		private IEnumerator login(string userID, string password, AuthenticateCallbackMethod callback)
		{
			var form = new WWWForm();
			form.AddField("api_key", apiKey);
			form.AddField("game_id", gameID);
			form.AddField("response", "xml");
			form.AddField("username", userID);
			form.AddField("password", password);
			var www = new WWW(scoreoidURL+"getPlayer", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "Scoreoid getPlayer failed: " + www.error;
				Debug.LogError(error);
				IsAuthenticated = false;
				if (callback != null) callback(false, error);
				yield break;
			}

			if (!checkError(www, out error))
			{
				try
				{
					var xml = new XmlSerializer(typeof(ScoreoidXML.players));
					using (var data = new MemoryStream(www.bytes))
					{
						var players = (ScoreoidXML.players)xml.Deserialize(data);
						UserID = players.player[0].username;
						achievementString = players.player[0].achievements != null ? players.player[0].achievements : "";
						IsAuthenticated = true;
						PlayerPrefs.SetString("ReignScoreoid_UserID", userID);
						PlayerPrefs.SetString("ReignScoreoid_Pass", password);
						Debug.Log("Authenticated as: " + UserID);
						if (callback != null) callback(true, null);
						yield break;
					}
				}
				catch (Exception e)
				{
					Debug.Log("Server Text: " + www.text);
					error = e.Message;
				}
			}

			if (callback != null) callback(false, error);
		}

		private IEnumerator createUser(string userID, string password, AuthenticateCallbackMethod callback)
		{
			var form = new WWWForm();
			form.AddField("api_key", apiKey);
			form.AddField("game_id", gameID);
			form.AddField("response", "xml");
			form.AddField("username", userID);
			form.AddField("password", password);
			var www = new WWW(scoreoidURL+"createPlayer", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "Scoreoid createPlayer failed: " + www.error;
				Debug.LogError(error);
				IsAuthenticated = false;
				if (callback != null) callback(false, error);
				yield break;
			}

			if (!checkError(www, out error))
			{
				try
				{
					var xml = new XmlSerializer(typeof(ScoreoidXML.success));
					using (var data = new MemoryStream(www.bytes))
					{
						var success = (ScoreoidXML.success)xml.Deserialize(data);
						UserID = userID;
						IsAuthenticated = true;
						PlayerPrefs.SetString("ReignScoreoid_UserID", userID);
						PlayerPrefs.SetString("ReignScoreoid_Pass", password);
						Debug.Log("Scoreoid createPlayer success: " + success.Content);
						if (callback != null) callback(true, null);
						yield break;
					}
				}
				catch (Exception e)
				{
					Debug.Log("Server Text: " + www.text);
					error = e.Message;
				}
			}

			if (callback != null) callback(false, error);
		}

		private void authenticateCallbackMethod(bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				if (guiAuthenticateCallback != null) guiAuthenticateCallback(true, errorMessage);
			}
			else if (desc.Scoreoid_AutoTriggerAuthenticateGUI)
			{
				PerformingGUIOperation = true;
				guiMode = Scoreoid_GuiModes.Login;
			}
		}
		
		public void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			try
			{
				// check if player exists
				if (PlayerPrefs.HasKey("ReignScoreoid_UserID"))
				{
					PerformingGUIOperation = false;
					guiAuthenticateCallback = callback;
					guiServices = services;
					services.StartCoroutine(login(PlayerPrefs.GetString("ReignScoreoid_UserID"), PlayerPrefs.GetString("ReignScoreoid_Pass"), authenticateCallbackMethod));
				}
				else
				{
					if (desc.Scoreoid_AutoTriggerAuthenticateGUI)
					{
						PerformingGUIOperation = true;
						guiAuthenticateCallback = callback;
						guiServices = services;
						guiMode = Scoreoid_GuiModes.Login;
					}
					else if (callback != null)
					{
						PerformingGUIOperation = false;
						guiAuthenticateCallback = null;
						guiServices = null;
						callback(false, "No user loggin info.");
					}
				}
			}
			catch (Exception e)
			{
				string error = "Scoreoid Authenticate error: " + e.Message;
				Debug.LogError(error);
				IsAuthenticated = false;
				PerformingGUIOperation = false;
				if (callback != null) callback(false, error);
			}
		}

		public void Logout()
		{
			IsAuthenticated = false;
			UserID = "???";
		}

		public void ManualLogin(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			services.StartCoroutine(login(userID, password, callback));
		}

		public void ManualCreateUser(string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			services.StartCoroutine(createUser(userID, password, callback));
		}

		private LeaderboardDesc findLeaderboard(string leaderboardID)
		{
			LeaderboardDesc leaderboardDesc = null;
			foreach (var id in desc.LeaderboardDescs)
			{
				if (id.ID == leaderboardID)
				{
					leaderboardDesc = id;
					break;
				}
			}

			if (leaderboardDesc == null)
			{
				Debug.LogError("Failed to find leaderboardID: " + leaderboardID);
				return null;
			}

			return leaderboardDesc;
		}

		private AchievementDesc findAchievement(string achievementID)
		{
			AchievementDesc achievementDesc = null;
			foreach (var id in desc.AchievementDescs)
			{
				if (id.ID == achievementID)
				{
					achievementDesc = id;
					break;
				}
			}

			if (achievementDesc == null)
			{
				Debug.LogError("Failed: achievementID not found.");
				return null;
			}

			return achievementDesc;
		}

		private IEnumerator async_ReportScore(string leaderboardID, int score, LeaderboardDesc leaderboardDesc, ReportScoreCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var scoreoidID = leaderboardDesc.Editor_Scoreoid_ID;
			#elif UNITY_STANDALONE_WIN
			var scoreoidID = leaderboardDesc.Win32_Scoreoid_ID;
			#elif UNITY_STANDALONE_OSX
			var scoreoidID = leaderboardDesc.OSX_Scoreoid_ID;
			#elif UNITY_STANDALONE_LINUX
			var scoreoidID = leaderboardDesc.Linux_Scoreoid_ID;
			#elif UNITY_WEBPLAYER
			var scoreoidID = leaderboardDesc.Web_Scoreoid_ID;
			#elif UNITY_METRO
			var scoreoidID = leaderboardDesc.Win8_Scoreoid_ID;
			#elif UNITY_WP8
			var scoreoidID = leaderboardDesc.WP8_Scoreoid_ID;
			#elif UNITY_BLACKBERRY
			var scoreoidID = leaderboardDesc.BB10_Scoreoid_ID;
			#elif UNITY_IPHONE
			var scoreoidID = leaderboardDesc.iOS_Scoreoid_ID;
			#elif UNITY_ANDROID
			var scoreoidID = leaderboardDesc.Android_Scoreoid_ID;
			#endif

			// report score
			var form = new WWWForm();
			form.AddField("api_key", apiKey);
			form.AddField("game_id", gameID);
			form.AddField("response", "xml");
			form.AddField("platform", scoreoidID.ToString());
			form.AddField("username", UserID);
			form.AddField("score", score.ToString());
			var www = new WWW(scoreoidURL+"createScore", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "Scoreoid createScore failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				yield break;
			}

			if (!checkError(www, out error))
			{
				try
				{
					var xml = new XmlSerializer(typeof(ScoreoidXML.success));
					using (var data = new MemoryStream(www.bytes))
					{
						var scoreXML = (ScoreoidXML.success)xml.Deserialize(data);
						Debug.Log("Scoreoid createScore success: " + scoreXML.Content);
						if (callback != null) callback(true, null);
						yield break;
					}
				}
				catch (Exception e)
				{
					Debug.Log("Server Text: " + www.text);
					error = e.Message;
				}
			}

			if (callback != null) callback(false, error);
		}
	
		public void ReportScore(string leaderboardID, int score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			// find leaderboard
			var leaderboardDesc = findLeaderboard(leaderboardID);
			if (leaderboardDesc == null)
			{
				if (callback != null) callback(false, "Failed to find leaderboardID: " + leaderboardID);
				return;
			}

			services.StartCoroutine(async_ReportScore(leaderboardID, score, leaderboardDesc, callback));
		}

		private IEnumerator async_RequestScores(string leaderboardID, int offset, int range, LeaderboardDesc leaderboardDesc, RequestScoresCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var scoreoidID = leaderboardDesc.Editor_Scoreoid_ID;
			#elif UNITY_STANDALONE_WIN
			var scoreoidID = leaderboardDesc.Win32_Scoreoid_ID;
			#elif UNITY_STANDALONE_OSX
			var scoreoidID = leaderboardDesc.OSX_Scoreoid_ID;
			#elif UNITY_STANDALONE_LINUX
			var scoreoidID = leaderboardDesc.Linux_Scoreoid_ID;
			#elif UNITY_WEBPLAYER
			var scoreoidID = leaderboardDesc.Web_Scoreoid_ID;
			#elif UNITY_METRO
			var scoreoidID = leaderboardDesc.Win8_Scoreoid_ID;
			#elif UNITY_WP8
			var scoreoidID = leaderboardDesc.WP8_Scoreoid_ID;
			#elif UNITY_BLACKBERRY
			var scoreoidID = leaderboardDesc.BB10_Scoreoid_ID;
			#elif UNITY_IPHONE
			var scoreoidID = leaderboardDesc.iOS_Scoreoid_ID;
			#elif UNITY_ANDROID
			var scoreoidID = leaderboardDesc.Android_Scoreoid_ID;
			#endif

			// report score
			var form = new WWWForm();
			form.AddField("api_key", apiKey);
			form.AddField("game_id", gameID);
			form.AddField("response", "xml");
			form.AddField("platform", scoreoidID.ToString());
			form.AddField("order_by", "score");
			form.AddField("limit", string.Format("{0},{1}", offset+1, range));
			var www = new WWW(scoreoidURL+"getScores", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "Scoreoid getScores failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(null, false, error);
				yield break;
			}

			if (!checkError(www, out error))
			{
				try
				{
					var xml = new XmlSerializer(typeof(ScoreoidXML.scores));
					using (var data = new MemoryStream(www.bytes))
					{
						var scoreXML = (ScoreoidXML.scores)xml.Deserialize(data);
						int count = (scoreXML.player != null ? scoreXML.player.Length : 0);
						var newScores = new LeaderboardScore[count];
						for (int i = 0; i != count; ++i)
						{
							newScores[i] = new LeaderboardScore(scoreXML.player[i].username, scoreXML.player[i].score.score);
						}

						Debug.Log("Scoreoid getScores success: " + count);
						if (callback != null) callback(newScores, true, null);
						yield break;
					}
				}
				catch (Exception e)
				{
					Debug.Log("Server Text: " + www.text);
					error = e.Message;
				}
			}

			if (callback != null) callback(null, false, error);
		}
		
		public void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			// find leaderboard
			var leaderboardDesc = findLeaderboard(leaderboardID);
			if (leaderboardDesc == null)
			{
				if (callback != null) callback(null, false, "Failed to find leaderboardID: " + leaderboardID);
				return;
			}

			services.StartCoroutine(async_RequestScores(leaderboardID, offset, range, leaderboardDesc, callback));
		}

		private IEnumerator async_ReportAchievement(string achievementID, AchievementDesc achievementDesc, ReportAchievementCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var scoreoidID = achievementDesc.Editor_Scoreoid_ID;
			#elif UNITY_STANDALONE_WIN
			var scoreoidID = achievementDesc.Win32_Scoreoid_ID;
			#elif UNITY_STANDALONE_OSX
			var scoreoidID = achievementDesc.OSX_Scoreoid_ID;
			#elif UNITY_STANDALONE_LINUX
			var scoreoidID = achievementDesc.Linux_Scoreoid_ID;
			#elif UNITY_WEBPLAYER
			var scoreoidID = achievementDesc.Web_Scoreoid_ID;
			#elif UNITY_METRO
			var scoreoidID = achievementDesc.Win8_Scoreoid_ID;
			#elif UNITY_WP8
			var scoreoidID = achievementDesc.WP8_Scoreoid_ID;
			#elif UNITY_BLACKBERRY
			var scoreoidID = achievementDesc.BB10_Scoreoid_ID;
			#elif UNITY_IPHONE
			var scoreoidID = achievementDesc.iOS_Scoreoid_ID;
			#elif UNITY_ANDROID
			var scoreoidID = achievementDesc.Android_Scoreoid_ID;
			#endif

			// append achievement string
			var achievementIDs = achievementString.Split(',');
			string newAchievementString = "";
			foreach (var achievement in achievementIDs)
			{
				if (achievement == scoreoidID)
				{
					// already achieved
					if (callback != null) callback(true, null);
					yield break;
				}

				newAchievementString += string.IsNullOrEmpty(newAchievementString) ? achievement : (','+achievement);
			}

			newAchievementString += string.IsNullOrEmpty(newAchievementString) ? scoreoidID : (','+scoreoidID);
			
			// report achievement
			var form = new WWWForm();
			form.AddField("api_key", apiKey);
			form.AddField("game_id", gameID);
			form.AddField("response", "xml");
			form.AddField("username", UserID);
			form.AddField("field", "achievements");
			form.AddField("value", newAchievementString);
			var www = new WWW(scoreoidURL+"updatePlayerField", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "Scoreoid updatePlayerField failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				yield break;
			}

			if (!checkError(www, out error))
			{
				try
				{
					var xml = new XmlSerializer(typeof(ScoreoidXML.success));
					using (var data = new MemoryStream(www.bytes))
					{
						var success = (ScoreoidXML.success)xml.Deserialize(data);
						achievementString = newAchievementString;
						Debug.Log("Scoreoid updatePlayerField success: " + success.Content);
						if (callback != null) callback(true, null);
						yield break;
					}
				}
				catch (Exception e)
				{
					Debug.Log("Server Text: " + www.text);
					error = e.Message;
				}
			}

			if (callback != null) callback(false, error);
		}
		
		public void ReportAchievement(string achievementID, ReportAchievementCallbackMethod callback, MonoBehaviour services)
		{
			// find leaderboard
			var achievementDesc = findAchievement(achievementID);
			if (achievementDesc == null)
			{
				if (callback != null) callback(false, "Failed to find achievementID: " + achievementID);
				return;
			}

			services.StartCoroutine(async_ReportAchievement(achievementID, achievementDesc, callback));
		}
		
		public void RequestAchievements(RequestAchievementsCallbackMethod callback)
		{
			// get pre-loaded achievements
			var achievementIDs = achievementString.Split(',');
			var newAchievements = new Achievement[desc.AchievementDescs.Length];
			for (int i = 0; i != desc.AchievementDescs.Length; ++i)
			{
				var achDesc = desc.AchievementDescs[i];

				// check if is achieved
				bool isAchieved = false;
				foreach (var achievementID in achievementIDs)
				{
					#if UNITY_EDITOR
					string id = achDesc.Editor_Scoreoid_ID;
					#elif UNITY_STANDALONE_WIN
					var id = achDesc.Win32_Scoreoid_ID;
					#elif UNITY_STANDALONE_OSX
					var id = achDesc.OSX_Scoreoid_ID;
					#elif UNITY_STANDALONE_LINUX
					var id = achDesc.Linux_Scoreoid_ID;
					#elif UNITY_WEBPLAYER
					var id = achDesc.Web_Scoreoid_ID;
					#elif UNITY_METRO
					var id = achDesc.Win8_Scoreoid_ID;
					#elif UNITY_WP8
					var id = achDesc.WP8_Scoreoid_ID;
					#elif UNITY_BLACKBERRY
					var id = achDesc.BB10_Scoreoid_ID;
					#elif UNITY_BLACKBERRY
					var id = achDesc.iOS_Scoreoid_ID;
					#elif UNITY_IPHONE
					var id = achDesc.iOS_Scoreoid_ID;
					#elif UNITY_ANDROID
					var id = achDesc.Android_Scoreoid_ID;
					#endif
					if (achievementID == id)
					{
						isAchieved = true;
						break;
					}
				}

				// check if textures already loaded
				Texture achievedTexture = null, unachievedTexture = null;
				if (achievements != null)
				{
					foreach (var achievement in achievements)
					{
						if (achievement.ID == achDesc.ID)
						{
							achievedTexture = achievement.AchievedImage;
							unachievedTexture = achievement.UnachievedImage;
							break;
						}
					}
				}

				if (achievedTexture == null)
				{
					string fileName = "Reign/Scoreoid/" + achDesc.ID + "_achieved";
					achievedTexture = (Texture)Resources.Load(fileName);
					if (achievedTexture == null)
					{
						string error = "RequestAchievements Failed to load texture: " + fileName;
						Debug.LogError(error);
						if (callback != null) callback(null, false, error);
						return;
					}
				}

				if (unachievedTexture == null)
				{
					string fileName = "Reign/Scoreoid/" + achDesc.ID + "_unachieved";
					unachievedTexture = (Texture)Resources.Load(fileName);
					if (unachievedTexture == null)
					{
						string error = "RequestAchievements Failed to load texture: " + fileName;
						Debug.LogError(error);
						if (callback != null) callback(null, false, error);
						return;
					}
				}

				// create new entry
				newAchievements[i] = new Achievement(isAchieved, achDesc.ID, achDesc.Name, achDesc.Desc, achievedTexture, unachievedTexture);
			}

			achievements = newAchievements;
			if (callback != null) callback(achievements, true, null);
		}

		private ShowNativeViewDoneCallbackMethod guiShowNativeViewDoneCallback;
		private LeaderboardScore[] guiScores;
		private void guiRequestScoresCallback(LeaderboardScore[] scores, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiScores = scores;
				guiMode = Scoreoid_GuiModes.ShowingScores;
			}
			else
			{
				PerformingGUIOperation = false;
				guiMode = Scoreoid_GuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiScoreOffset;
		private string guiLeaderboardID;
		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			guiMode = Scoreoid_GuiModes.LoadingScores;
			PerformingGUIOperation = true;
			guiShowNativeViewDoneCallback = callback;
			guiLeaderboardID = leaderboardID;
			guiScoreOffset = 0;
			RequestScores(leaderboardID, guiScoreOffset, desc.Scoreoid_TopScoresToListPerPage, guiRequestScoresCallback, services);
		}

		private Achievement[] guiAchievements;
		private void guiRequestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiAchievements = achievements;
				guiMode = Scoreoid_GuiModes.ShowingAchievements;
			}
			else
			{
				PerformingGUIOperation = false;
				guiMode = Scoreoid_GuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiAchievementOffset;
		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			guiMode = Scoreoid_GuiModes.LoadingAchievements;
			PerformingGUIOperation = true;
			guiAchievementOffset = 0;
			guiShowNativeViewDoneCallback = callback;
			RequestAchievements(guiRequestAchievementsCallback);
		}

		private static Vector2 fillView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return fillView(objectSize, viewSize);
		}
	
		private static Vector2 fillView(Vector2 objectSize, Vector2 viewSize)
		{
			float objectSlope = objectSize.y / objectSize.x;
			float viewSlope = viewSize.y / viewSize.x;
		
			if (objectSlope <= viewSlope) return new Vector2(objectSize.x/objectSize.y, 1) * viewSize.y;
			else return new Vector2(1, objectSize.y/objectSize.x) * viewSize.x;
		}

		private static Vector2 fitInView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return fitInView(objectSize, viewSize);
		}
	
		private static Vector2 fitInView(Vector2 objectSize, Vector2 viewSize)
		{
			float objectSlope = objectSize.y / objectSize.x;
			float viewSlope = viewSize.y / viewSize.x;
		
			if (objectSlope >= viewSlope) return new Vector2(objectSize.x/objectSize.y, 1) * viewSize.y;
			else return new Vector2(1, objectSize.y/objectSize.x) * viewSize.x;
		}

		private static Vector2 scaleToFitInView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return scaleToFitInView(objectSize, viewSize);
		}
	
		private static Vector2 scaleToFitInView(Vector2 objectSize, Vector2 viewSize)
		{
			var fitViewSize = fitInView(objectSize, viewSize);
			objectSize.x /= fitViewSize.x;
			objectSize.y /= fitViewSize.y;
			return objectSize;
		}

		private string userAccount_Name = "", userAccount_Pass = "", userAccount_ConfPass = "", errorText;
		public void OnGUI()
		{
			if (guiMode == Scoreoid_GuiModes.None) return;

			GUI.color = Color.white;
			GUI.matrix = Matrix4x4.identity;
			float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

			// draw background
			if (desc.Scoreoid_BackgroudTexture != null)
			{
				var size = fillView(desc.Scoreoid_BackgroudTexture.width, desc.Scoreoid_BackgroudTexture.height, Screen.width, Screen.height);
				float offsetX = -Mathf.Max((size.x-Screen.width)*.5f, 0f);
				float offsetY = -Mathf.Max((size.y-Screen.height)*.5f, 0f);
				GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.Scoreoid_BackgroudTexture);
			}

			float buttonWidth = 128 * scale;
			float buttonHeight = 64 * scale;
			float textWidth = 256 * scale;
			float textHeight = 32 * scale;
			float y = Screen.height / 2;
			if (guiMode == Scoreoid_GuiModes.Login)
			{
				// title
				if (!string.IsNullOrEmpty(desc.Scoreoid_LoginTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), desc.Scoreoid_LoginTitle, style);
				}

				// labels
				GUI.Label(new Rect((Screen.width/2) - textWidth - (10*scale), y, textWidth, textHeight), "Username");
				GUI.Label(new Rect((Screen.width/2) + (10*scale), y, textWidth, textHeight), "Password");
				y += textHeight;

				// text fields
				userAccount_Name = GUI.TextField(new Rect((Screen.width/2) - textWidth - (10*scale), y, textWidth, textHeight), userAccount_Name);
				userAccount_Pass = GUI.PasswordField(new Rect((Screen.width/2) + (10*scale), y, textWidth, textHeight), userAccount_Pass, '*');
				y += textHeight * 2;

				// buttons
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, buttonWidth, buttonHeight), "Cancel"))
				{
					errorText = null;
					guiMode = Scoreoid_GuiModes.None;
					PerformingGUIOperation = false;
					if (guiAuthenticateCallback != null) guiAuthenticateCallback(false, "Canceled");
				}

				if (GUI.Button(new Rect((Screen.width/2) + (10*scale), y, buttonWidth, buttonHeight), "Login"))
				{
					errorText = null;
					bool validInfo = true;
					if (string.IsNullOrEmpty(userAccount_Name))
					{
						validInfo = false;
						errorText = "Invalid username.";
						Debug.LogError(errorText);
					}
					else if (string.IsNullOrEmpty(userAccount_Pass))
					{
						validInfo = false;
						errorText = "Invalid user password.";
						Debug.LogError(errorText);
					}

					if (validInfo)
					{
						PerformingGUIOperation = true;
						guiMode = Scoreoid_GuiModes.LoggingIn;
						guiServices.StartCoroutine(login(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Create New User"))
				{
					guiMode = Scoreoid_GuiModes.CreateUser;
					errorText = null;
				}
			}
			else if (guiMode == Scoreoid_GuiModes.CreateUser)
			{
				// title
				if (!string.IsNullOrEmpty(desc.Scoreoid_CreateUserTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), desc.Scoreoid_CreateUserTitle, style);
				}

				// labels
				float offsetX = ((10*scale) + textWidth) * -.5f;
				GUI.Label(new Rect((Screen.width/2) - textWidth - (10*scale) + offsetX, y, textWidth, textHeight), "Username");
				GUI.Label(new Rect((Screen.width/2) + (10*scale) + offsetX, y, textWidth, textHeight), "Password");
				GUI.Label(new Rect((Screen.width/2) + (20*scale) + textWidth + offsetX, y, textWidth, textHeight), "Confirm Password");
				y += textHeight;

				// text fields
				userAccount_Name = GUI.TextField(new Rect((Screen.width/2) - textWidth - (10*scale) + offsetX, y, textWidth, textHeight), userAccount_Name);
				userAccount_Pass = GUI.PasswordField(new Rect((Screen.width/2) + (10*scale) + offsetX, y, textWidth, textHeight), userAccount_Pass, '*');
				userAccount_ConfPass = GUI.PasswordField(new Rect((Screen.width/2) + (20*scale) + textWidth + offsetX, y, textWidth, textHeight), userAccount_ConfPass, '*');
				y += textHeight * 2;

				// buttons
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, buttonWidth, buttonHeight), "Cancel"))
				{
					errorText = null;
					guiMode = Scoreoid_GuiModes.None;
					PerformingGUIOperation = false;
					if (guiAuthenticateCallback != null) guiAuthenticateCallback(false, "Canceled");
				}

				if (GUI.Button(new Rect((Screen.width/2) + (10*scale), y, buttonWidth, buttonHeight), "Create"))
				{
					errorText = null;
					bool validInfo = true;
					if (string.IsNullOrEmpty(userAccount_Name))
					{
						validInfo = false;
						errorText = "Invalid username.";
						Debug.LogError(errorText);
					}
					else if (string.IsNullOrEmpty(userAccount_Pass) || string.IsNullOrEmpty(userAccount_ConfPass))
					{
						validInfo = false;
						errorText = "Invalid user password.";
						Debug.LogError(errorText);
					}
					else if (userAccount_Pass != userAccount_ConfPass)
					{
						validInfo = false;
						errorText = "Passwords dont match.";
						Debug.LogError(errorText);
					}
					else if (userAccount_Pass.Length < 6)
					{
						validInfo = false;
						errorText = "Passwords to short.";
						Debug.LogError(errorText);
					}

					if (validInfo)
					{
						PerformingGUIOperation = true;
						guiMode = Scoreoid_GuiModes.CreatingUser;
						guiServices.StartCoroutine(createUser(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Login Existing User"))
				{
					guiMode = Scoreoid_GuiModes.Login;
					errorText = null;
				}
			}
			else if (guiMode == Scoreoid_GuiModes.LoggingIn)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Logging In...", style);
			}
			else if (guiMode == Scoreoid_GuiModes.ShowingScores)
			{
				if (desc.Scoreoid_TopScoreBoardTexture != null)
				{
					// draw board
					var size = fitInView(desc.Scoreoid_TopScoreBoardTexture.width, desc.Scoreoid_TopScoreBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.Scoreoid_TopScoreBoardTexture);

					// get main scale value
					var mainScale = scaleToFitInView(desc.Scoreoid_TopScoreBoardTexture.width, desc.Scoreoid_TopScoreBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(desc.Scoreoid_TopScoreBoardFrame_CloseBox, desc.Scoreoid_TopScoreBoardButton_CloseNormal, desc.Scoreoid_TopScoreBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = Scoreoid_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(desc.Scoreoid_TopScoreBoardFrame_PrevButton, desc.Scoreoid_TopScoreBoardButton_PrevNormal, desc.Scoreoid_TopScoreBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiScoreOffset != 0)
						{
							guiScoreOffset -= desc.Scoreoid_TopScoresToListPerPage;
							if (guiScoreOffset < 0) guiScoreOffset = 0;
							RequestScores(guiLeaderboardID, guiScoreOffset, desc.Scoreoid_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					if (processButton(desc.Scoreoid_TopScoreBoardFrame_NextButton, desc.Scoreoid_TopScoreBoardButton_NextNormal, desc.Scoreoid_TopScoreBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiScores.Length == desc.Scoreoid_TopScoresToListPerPage)
						{
							guiScoreOffset += desc.Scoreoid_TopScoresToListPerPage;
							RequestScores(guiLeaderboardID, guiScoreOffset, desc.Scoreoid_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					// draw names and scores
					var usernameRect = calculateFrame(desc.Scoreoid_TopScoreBoardFrame_Usernames, mainScale, offsetX, offsetY);
					var scoreRect = calculateFrame(desc.Scoreoid_TopScoreBoardFrame_Scores, mainScale, offsetX, offsetY);
					if (desc.Scoreoid_EnableTestRects)
					{
						GUI.Button(usernameRect, "TEST RECT");
						GUI.Button(scoreRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(desc.Scoreoid_TopScoreBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = desc.Scoreoid_TopScoreBoardFont_Color;
					int userI = 0, scoreI = 0;
					foreach (var score in guiScores)
					{
						// username
						float height = usernameRect.height / desc.Scoreoid_TopScoresToListPerPage;
						GUI.Label(new Rect(usernameRect.x, usernameRect.y + userI, usernameRect.width, height), score.UserName, style);
						userI += (int)height;

						// score
						height = scoreRect.height / desc.Scoreoid_TopScoresToListPerPage;
						string scoreValue;
						if (desc.Scoreoid_ScoreFormatCallback != null) desc.Scoreoid_ScoreFormatCallback(score.Score, out scoreValue);
						else scoreValue = score.Score.ToString();
						GUI.Label(new Rect(scoreRect.x, scoreRect.y + scoreI, scoreRect.width, height), scoreValue, style);
						scoreI += (int)height;
					}
				}
				else
				{
					errorText = "Scoreoid_TopScoreBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			else if (guiMode == Scoreoid_GuiModes.ShowingAchievements)
			{
				if (desc.Scoreoid_AchievementBoardTexture != null)
				{
					// draw board
					var size = fitInView(desc.Scoreoid_AchievementBoardTexture.width, desc.Scoreoid_AchievementBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.Scoreoid_AchievementBoardTexture);

					// get main scale value
					var mainScale = scaleToFitInView(desc.Scoreoid_AchievementBoardTexture.width, desc.Scoreoid_AchievementBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(desc.Scoreoid_AchievementBoardFrame_CloseBox, desc.Scoreoid_AchievementBoardButton_CloseNormal, desc.Scoreoid_AchievementBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = Scoreoid_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(desc.Scoreoid_AchievementBoardFrame_PrevButton, desc.Scoreoid_AchievementBoardButton_PrevNormal, desc.Scoreoid_AchievementBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset != 0)
						{
							guiAchievementOffset -= desc.Scoreoid_AchievementsToListPerPage;
							if (guiAchievementOffset < 0) guiAchievementOffset = 0;
						}
					}

					if (processButton(desc.Scoreoid_AchievementBoardFrame_NextButton, desc.Scoreoid_AchievementBoardButton_NextNormal, desc.Scoreoid_AchievementBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset + desc.Scoreoid_AchievementsToListPerPage < guiAchievements.Length)
						{
							guiAchievementOffset += desc.Scoreoid_AchievementsToListPerPage;
						}
					}

					// draw names and scores
					var nameRect = calculateFrame(desc.Scoreoid_AchievementBoardFrame_Names, mainScale, offsetX, offsetY);
					var descRect = calculateFrame(desc.Scoreoid_AchievementBoardFrame_Descs, mainScale, offsetX, offsetY);
					if (desc.Scoreoid_EnableTestRects)
					{
						GUI.Button(nameRect, "TEST RECT");
						GUI.Button(descRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(desc.Scoreoid_AchievementBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = desc.Scoreoid_AchievementBoardFont_Color;
					int nameI = 0, descI = 0;
					for (int i = guiAchievementOffset; i < guiAchievementOffset+desc.Scoreoid_AchievementsToListPerPage; ++i)
					{
						if (i == guiAchievements.Length) break;
						var ach = guiAchievements[i];

						// icon
						float height = nameRect.height / desc.Scoreoid_AchievementsToListPerPage;
						float iconSize = height * .8f;
						GUI.DrawTexture(new Rect(nameRect.x, nameRect.y + nameI + height - iconSize, iconSize, iconSize), ach.IsAchieved ? ach.AchievedImage : ach.UnachievedImage);

						// name
						GUI.Label(new Rect(height + nameRect.x, nameRect.y + nameI, nameRect.width, height), ach.Name, style);
						nameI += (int)height;

						// desc
						height = descRect.height / desc.Scoreoid_AchievementsToListPerPage;
						GUI.Label(new Rect(descRect.x, descRect.y + descI, descRect.width, height), ach.Desc, style);
						descI += (int)height;
					}
				}
				else
				{
					errorText = "Scoreoid_AchievementBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			else if (guiMode == Scoreoid_GuiModes.CreatingUser)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Creating User...", style);
			}
			else if (guiMode == Scoreoid_GuiModes.LoadingScores || guiMode == Scoreoid_GuiModes.LoadingAchievements)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Loading...", style);
			}

			// error text
			if (!string.IsNullOrEmpty(errorText))
			{
				var errorStyle = new GUIStyle();
				errorStyle.fontSize = (int)(32 * scale);
				errorStyle.alignment = TextAnchor.MiddleCenter;
				errorStyle.normal.textColor = Color.red;
				GUI.Label(new Rect(0, Screen.height-(Screen.height/8), Screen.width, Screen.height/8), errorText, errorStyle);
			}
		}

		private Rect calculateFrame(Rect frame, Vector2 mainScale, float offsetX, float offsetY)
		{
			var rect = frame;
			rect.x = (rect.x / mainScale.x) + offsetX;
			rect.y = (rect.y / mainScale.y) + offsetY;
			rect.width /= mainScale.x;
			rect.height /= mainScale.y;

			return rect;
		}

		private bool processButton(Rect frame, Texture normal, Texture hover, Vector2 mainScale, float offsetX, float offsetY)
		{
			var rect = calculateFrame(frame, mainScale, offsetX, offsetY);

			bool pass;
			var style = new GUIStyle();
			if (normal != null) pass = GUI.Button(rect, normal, style);
			else pass = GUI.Button(rect, "???");

			if (hover != null)
			{
				var pos = Input.mousePosition;
				pos.y = Screen.height - pos.y;
				if (pos.x > rect.xMin && pos.x < rect.xMax && pos.y > rect.yMin && pos.y < rect.yMax) GUI.DrawTexture(rect, hover);
			}

			if (pass)
			{
				if (desc.Scoreoid_AudioSource != null && desc.Scoreoid_ButtonClick != null) desc.Scoreoid_AudioSource.PlayOneShot(desc.Scoreoid_ButtonClick);
				return true;
			}

			return false;
		}

		private void guiAuthenticateCallbackTEMP(bool success, string errorMessage)
		{
			if (success)
			{
				PerformingGUIOperation = false;
				guiMode = Scoreoid_GuiModes.None;
				if (guiAuthenticateCallback != null) guiAuthenticateCallback(true, null);
			}
			else
			{
				Debug.LogError(errorMessage);
				errorText = errorMessage;
				if (guiMode == Scoreoid_GuiModes.LoggingIn) guiMode = Scoreoid_GuiModes.Login;
				else if (guiMode == Scoreoid_GuiModes.CreatingUser) guiMode = Scoreoid_GuiModes.CreateUser;
			}
		}

		public void Update()
		{
			// do nothing...
		}
	}
}

