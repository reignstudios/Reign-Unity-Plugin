using System;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Reign.Plugin
{
	namespace XML
	{
		public enum ResponseTypes
		{
			Error,
			Succeeded
		}

		public class WebResponse_Score
		{
			[XmlElement("UserID")] public string UserID;
			[XmlElement("Username")] public string Username;
			[XmlElement("Score")] public int Score;
		}

		[XmlRoot("ClientResponse")]
		public class WebResponse
		{
			[XmlAttribute("Type")] public ResponseTypes Type;
			[XmlElement("ErrorMessage")] public string ErrorMessage;
			[XmlElement("UserID")] public string UserID;
			[XmlElement("Score")] public List<WebResponse_Score> Scores;

			public WebResponse() {}
			public WebResponse(ResponseTypes type)
			{
				this.Type = type;
			}
		}
	}

	enum ReignScores_GuiModes
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

	public class ReignScores_ScorePlugin : IScorePlugin
	{
		private const string reignScoresURL = "https://reignscores.azurewebsites.net/Services/";
		private const string userAPIKey = "CE8E55E1-F383-4F05-9388-5C89F27B7FF2";
		private const string gameAPIKey = "04E0676D-AAF8-4836-A584-DE0C1D618D84";

		public bool PerformingGUIOperation {get; private set;}
		private ReignScores_GuiModes guiMode = ReignScores_GuiModes.None;
		private AuthenticateCallbackMethod guiAuthenticateCallback;
		private MonoBehaviour guiServices;

		public bool IsAuthenticated {get; private set;}
		public string Username {get; private set;}
		public string UserID {get; private set;}

		private ScoreDesc desc;
		private string gameID;
		private Achievement[] achievements;
	
		public ReignScores_ScorePlugin(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			this.desc = desc;
			#if UNITY_EDITOR
			gameID = desc.Editor_ReignScores_GameID;
			#elif UNITY_STANDALONE_WIN
			gameID = desc.Win32_ReignScores_GameID;
			#elif UNITY_STANDALONE_OSX
			gameID = desc.OSX_ReignScores_GameID;
			#elif UNITY_STANDALONE_LINUX
			gameID = desc.Linux_ReignScores_GameID;
			#elif UNITY_WEBPLAYER
			gameID = desc.Web_ReignScores_GameID;
			#elif UNITY_METRO
			gameID = desc.Win8_ReignScores_GameID;
			#elif UNITY_WP8
			gameID = desc.WP8_ReignScores_GameID;
			#elif UNITY_BLACKBERRY
			gameID = desc.BB10_ReignScores_GameID;
			#elif UNITY_IPHONE
			gameID = desc.iOS_ReignScores_GameID;
			#elif UNITY_ANDROID
			gameID = desc.Android_ReignScores_GameID;
			#endif

			if (callback != null) callback(true, null);
		}

		private static byte[] getBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private bool checkError(WWW www, out XML.WebResponse response, out string errorMessage)
		{
			try
			{
				var xml = new XmlSerializer(typeof(XML.WebResponse));
				using (var data = new MemoryStream(www.bytes))
				{
					response = (XML.WebResponse)xml.Deserialize(data);
					if (response.Type == XML.ResponseTypes.Error)
					{
						errorMessage = response.ErrorMessage;
						Debug.LogError("Reign Scores Error: " + response.ErrorMessage);
						return true;
					}
					else
					{
						errorMessage = null;
						return false;
					}
				}
			}
			catch (Exception e)
			{
				response = null;
				errorMessage = e.Message;
				return true;
			}
		}

		private IEnumerator login(string username, string password, AuthenticateCallbackMethod callback)
		{
			var form = new WWWForm();
			form.AddField("api_key", userAPIKey);
			form.AddField("game_id", gameID);
			form.AddField("username", username);
			form.AddField("password", password);
			var www = new WWW(reignScoresURL+"Users/Login.cshtml", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "ReignScores Login failed: " + www.error;
				Debug.LogError(error);
				IsAuthenticated = false;
				if (callback != null) callback(false, error);
				yield break;
			}

			XML.WebResponse response;
			if (!checkError(www, out response, out error))
			{
				Username = username;
				UserID = response.UserID;
				IsAuthenticated = true;
				PlayerPrefs.SetString("ReignScores_UserID", username);
				PlayerPrefs.SetString("ReignScores_Pass", password);
				Debug.Log("Authenticated as: " + username);
				if (callback != null) callback(true, null);
				yield break;
			}

			if (callback != null) callback(false, error);
		}

		private IEnumerator createUser(string username, string password, AuthenticateCallbackMethod callback)
		{
			var form = new WWWForm();
			form.AddField("api_key", gameAPIKey);
			form.AddField("game_id", gameID);
			form.AddField("username", username);
			form.AddField("password", password);
			var www = new WWW(reignScoresURL+"Games/CreateUser.cshtml", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "ReignScores CreateUser failed: " + www.error;
				Debug.LogError(error);
				IsAuthenticated = false;
				if (callback != null) callback(false, error);
				yield break;
			}

			XML.WebResponse response;
			if (!checkError(www, out response, out error))
			{
				Username = username;
				UserID = response.UserID;
				IsAuthenticated = true;
				PlayerPrefs.SetString("ReignScores_UserID", username);
				PlayerPrefs.SetString("ReignScores_Pass", password);
				Debug.Log("ReignScores CreateUser success: " + username);
				if (callback != null) callback(true, null);
				yield break;
			}

			if (callback != null) callback(false, error);
		}

		private void authenticateCallbackMethod(bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				if (guiAuthenticateCallback != null) guiAuthenticateCallback(true, errorMessage);
			}
			else if (desc.ReignScores_AutoTriggerAuthenticateGUI)
			{
				PerformingGUIOperation = true;
				guiMode = ReignScores_GuiModes.Login;
			}
		}
		
		public void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			try
			{
				// check if player exists
				if (PlayerPrefs.HasKey("ReignScores_UserID"))
				{
					PerformingGUIOperation = false;
					guiAuthenticateCallback = callback;
					guiServices = services;
					services.StartCoroutine(login(PlayerPrefs.GetString("ReignScores_UserID"), PlayerPrefs.GetString("ReignScores_Pass"), authenticateCallbackMethod));
				}
				else
				{
					if (desc.ReignScores_AutoTriggerAuthenticateGUI)
					{
						PerformingGUIOperation = true;
						guiAuthenticateCallback = callback;
						guiServices = services;
						guiMode = ReignScores_GuiModes.Login;
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
				string error = "ReignScores Authenticate error: " + e.Message;
				Debug.LogError(error);
				IsAuthenticated = false;
				PerformingGUIOperation = false;
				if (callback != null) callback(false, error);
			}
		}

		public void Logout()
		{
			IsAuthenticated = false;
			Username = "???";
			if (PlayerPrefs.HasKey("ReignScores_UserID")) PlayerPrefs.DeleteKey("ReignScores_UserID");
			if (PlayerPrefs.HasKey("ReignScores_Pass")) PlayerPrefs.DeleteKey("ReignScores_Pass");
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

		private IEnumerator async_ReportScore(int score, LeaderboardDesc leaderboardDesc, ReportScoreCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var leaderboardID = leaderboardDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var leaderboardID = leaderboardDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var leaderboardID = leaderboardDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var leaderboardID = leaderboardDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var leaderboardID = leaderboardDesc.Web_ReignScores_ID;
			#elif UNITY_METRO
			var leaderboardID = leaderboardDesc.Win8_ReignScores_ID;
			#elif UNITY_WP8
			var leaderboardID = leaderboardDesc.WP8_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var leaderboardID = leaderboardDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var leaderboardID = leaderboardDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var leaderboardID = leaderboardDesc.Android_ReignScores_ID;
			#endif

			// report score
			var form = new WWWForm();
			form.AddField("api_key", userAPIKey);
			form.AddField("user_id", UserID);
			form.AddField("leaderboard_id", leaderboardID.ToString());
			form.AddField("score", score.ToString());
			var www = new WWW(reignScoresURL+"Users/ReportScore.cshtml", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "ReignScores ReportScore failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				yield break;
			}

			XML.WebResponse response;
			if (!checkError(www, out response, out error))
			{
				Debug.Log("ReignScores createScore success");
				if (callback != null) callback(true, null);
				yield break;
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

			services.StartCoroutine(async_ReportScore(score, leaderboardDesc, callback));
		}

		private IEnumerator async_RequestScores(int offset, int range, LeaderboardDesc leaderboardDesc, RequestScoresCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var leaderboardID = leaderboardDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var leaderboardID = leaderboardDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var leaderboardID = leaderboardDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var leaderboardID = leaderboardDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var leaderboardID = leaderboardDesc.Web_ReignScores_ID;
			#elif UNITY_METRO
			var leaderboardID = leaderboardDesc.Win8_ReignScores_ID;
			#elif UNITY_WP8
			var leaderboardID = leaderboardDesc.WP8_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var leaderboardID = leaderboardDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var leaderboardID = leaderboardDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var leaderboardID = leaderboardDesc.Android_ReignScores_ID;
			#endif

			// report score
			var form = new WWWForm();
			form.AddField("api_key", gameAPIKey);
			form.AddField("leaderboard_id", leaderboardID.ToString());
			form.AddField("limit", string.Format("{0},{1}", offset, range));
			var www = new WWW(reignScoresURL+"Games/RequestScores.cshtml", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "ReignScores RequestScores failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(null, false, error);
				yield break;
			}

			XML.WebResponse response;
			if (!checkError(www, out response, out error))
			{
				int count = (response.Scores != null ? response.Scores.Count : 0);
				var newScores = new LeaderboardScore[count];
				for (int i = 0; i != count; ++i)
				{
					newScores[i] = new LeaderboardScore(response.Scores[i].Username, response.Scores[i].Score);
				}

				Debug.Log("ReignScores RequestScores success: " + count);
				if (callback != null) callback(newScores, true, null);
				yield break;
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

			services.StartCoroutine(async_RequestScores(offset, range, leaderboardDesc, callback));
		}

		private IEnumerator async_ReportAchievement(AchievementDesc achievementDesc, float percentComplete, ReportAchievementCallbackMethod callback)
		{
			#if UNITY_EDITOR
			var achievementID = achievementDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var achievementID = achievementDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var achievementID = achievementDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var achievementID = achievementDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var achievementID = achievementDesc.Web_ReignScores_ID;
			#elif UNITY_METRO
			var achievementID = achievementDesc.Win8_ReignScores_ID;
			#elif UNITY_WP8
			var achievementID = achievementDesc.WP8_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var achievementID = achievementDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var achievementID = achievementDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var achievementID = achievementDesc.Android_ReignScores_ID;
			#endif

			// report achievement
			var form = new WWWForm();
			form.AddField("api_key", userAPIKey);
			form.AddField("user_id", UserID);
			form.AddField("achievement_id", achievementID.ToString());
			form.AddField("percent_complete", percentComplete.ToString());
			var www = new WWW(reignScoresURL+"Users/RequestAchievments.cshtml", form);
			yield return www;
			string error;
			if (!string.IsNullOrEmpty(www.error))
			{
				error = "ReignScores RequestAchievments failed: " + www.error;
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				yield break;
			}

			XML.WebResponse response;
			if (!checkError(www, out response, out error))
			{
				Debug.Log("ReignScores RequestAchievments success");
				if (callback != null) callback(true, null);
				yield break;
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

			services.StartCoroutine(async_ReportAchievement(achievementDesc, 100, callback));
		}
		
		public void RequestAchievements(RequestAchievementsCallbackMethod callback)
		{
			/*// get pre-loaded achievements
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
					string id = achDesc.Editor_ReignScores_ID;
					#elif UNITY_STANDALONE_WIN
					var id = achDesc.Win32_ReignScores_ID;
					#elif UNITY_STANDALONE_OSX
					var id = achDesc.OSX_ReignScores_ID;
					#elif UNITY_STANDALONE_LINUX
					var id = achDesc.Linux_ReignScores_ID;
					#elif UNITY_WEBPLAYER
					var id = achDesc.Web_ReignScores_ID;
					#elif UNITY_METRO
					var id = achDesc.Win8_ReignScores_ID;
					#elif UNITY_WP8
					var id = achDesc.WP8_ReignScores_ID;
					#elif UNITY_BLACKBERRY
					var id = achDesc.BB10_ReignScores_ID;
					#elif UNITY_BLACKBERRY
					var id = achDesc.iOS_ReignScores_ID;
					#elif UNITY_IPHONE
					var id = achDesc.iOS_ReignScores_ID;
					#elif UNITY_ANDROID
					var id = achDesc.Android_ReignScores_ID;
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
					string fileName = "Reign/ReignScores/" + achDesc.ID + "_achieved";
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
					string fileName = "Reign/ReignScores/" + achDesc.ID + "_unachieved";
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
			if (callback != null) callback(achievements, true, null);*/
		}

		private ShowNativeViewDoneCallbackMethod guiShowNativeViewDoneCallback;
		private LeaderboardScore[] guiScores;
		private void guiRequestScoresCallback(LeaderboardScore[] scores, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiScores = scores;
				guiMode = ReignScores_GuiModes.ShowingScores;
			}
			else
			{
				PerformingGUIOperation = false;
				guiMode = ReignScores_GuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiScoreOffset;
		private string guiLeaderboardID;
		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			guiMode = ReignScores_GuiModes.LoadingScores;
			PerformingGUIOperation = true;
			guiShowNativeViewDoneCallback = callback;
			guiLeaderboardID = leaderboardID;
			guiScoreOffset = 0;
			RequestScores(leaderboardID, guiScoreOffset, desc.ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, services);
		}

		private Achievement[] guiAchievements;
		private void guiRequestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiAchievements = achievements;
				guiMode = ReignScores_GuiModes.ShowingAchievements;
			}
			else
			{
				PerformingGUIOperation = false;
				guiMode = ReignScores_GuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiAchievementOffset;
		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			guiMode = ReignScores_GuiModes.LoadingAchievements;
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
			if (guiMode == ReignScores_GuiModes.None) return;

			GUI.color = Color.white;
			GUI.matrix = Matrix4x4.identity;
			float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

			// draw background
			if (desc.ReignScores_BackgroudTexture != null)
			{
				var size = fillView(desc.ReignScores_BackgroudTexture.width, desc.ReignScores_BackgroudTexture.height, Screen.width, Screen.height);
				float offsetX = -Mathf.Max((size.x-Screen.width)*.5f, 0f);
				float offsetY = -Mathf.Max((size.y-Screen.height)*.5f, 0f);
				GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.ReignScores_BackgroudTexture);
			}

			float buttonWidth = 128 * scale;
			float buttonHeight = 64 * scale;
			float textWidth = 256 * scale;
			float textHeight = 32 * scale;
			float y = Screen.height / 2;
			if (guiMode == ReignScores_GuiModes.Login)
			{
				// title
				if (!string.IsNullOrEmpty(desc.ReignScores_LoginTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), desc.ReignScores_LoginTitle, style);
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
					guiMode = ReignScores_GuiModes.None;
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
						guiMode = ReignScores_GuiModes.LoggingIn;
						guiServices.StartCoroutine(login(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Create New User"))
				{
					guiMode = ReignScores_GuiModes.CreateUser;
					errorText = null;
				}
			}
			else if (guiMode == ReignScores_GuiModes.CreateUser)
			{
				// title
				if (!string.IsNullOrEmpty(desc.ReignScores_CreateUserTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), desc.ReignScores_CreateUserTitle, style);
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
					guiMode = ReignScores_GuiModes.None;
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
						guiMode = ReignScores_GuiModes.CreatingUser;
						guiServices.StartCoroutine(createUser(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Login Existing User"))
				{
					guiMode = ReignScores_GuiModes.Login;
					errorText = null;
				}
			}
			else if (guiMode == ReignScores_GuiModes.LoggingIn)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Logging In...", style);
			}
			else if (guiMode == ReignScores_GuiModes.ShowingScores)
			{
				if (desc.ReignScores_TopScoreBoardTexture != null)
				{
					// draw board
					var size = fitInView(desc.ReignScores_TopScoreBoardTexture.width, desc.ReignScores_TopScoreBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.ReignScores_TopScoreBoardTexture);

					// get main scale value
					var mainScale = scaleToFitInView(desc.ReignScores_TopScoreBoardTexture.width, desc.ReignScores_TopScoreBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(desc.ReignScores_TopScoreBoardFrame_CloseBox, desc.ReignScores_TopScoreBoardButton_CloseNormal, desc.ReignScores_TopScoreBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = ReignScores_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(desc.ReignScores_TopScoreBoardFrame_PrevButton, desc.ReignScores_TopScoreBoardButton_PrevNormal, desc.ReignScores_TopScoreBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiScoreOffset != 0)
						{
							guiScoreOffset -= desc.ReignScores_TopScoresToListPerPage;
							if (guiScoreOffset < 0) guiScoreOffset = 0;
							RequestScores(guiLeaderboardID, guiScoreOffset, desc.ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					if (processButton(desc.ReignScores_TopScoreBoardFrame_NextButton, desc.ReignScores_TopScoreBoardButton_NextNormal, desc.ReignScores_TopScoreBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiScores.Length == desc.ReignScores_TopScoresToListPerPage)
						{
							guiScoreOffset += desc.ReignScores_TopScoresToListPerPage;
							RequestScores(guiLeaderboardID, guiScoreOffset, desc.ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					// draw names and scores
					var usernameRect = calculateFrame(desc.ReignScores_TopScoreBoardFrame_Usernames, mainScale, offsetX, offsetY);
					var scoreRect = calculateFrame(desc.ReignScores_TopScoreBoardFrame_Scores, mainScale, offsetX, offsetY);
					if (desc.ReignScores_EnableTestRects)
					{
						GUI.Button(usernameRect, "TEST RECT");
						GUI.Button(scoreRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(desc.ReignScores_TopScoreBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = desc.ReignScores_TopScoreBoardFont_Color;
					int userI = 0, scoreI = 0;
					foreach (var score in guiScores)
					{
						// username
						float height = usernameRect.height / desc.ReignScores_TopScoresToListPerPage;
						GUI.Label(new Rect(usernameRect.x, usernameRect.y + userI, usernameRect.width, height), score.UserName, style);
						userI += (int)height;

						// score
						height = scoreRect.height / desc.ReignScores_TopScoresToListPerPage;
						string scoreValue;
						if (desc.ReignScores_ScoreFormatCallback != null) desc.ReignScores_ScoreFormatCallback(score.Score, out scoreValue);
						else scoreValue = score.Score.ToString();
						GUI.Label(new Rect(scoreRect.x, scoreRect.y + scoreI, scoreRect.width, height), scoreValue, style);
						scoreI += (int)height;
					}
				}
				else
				{
					errorText = "ReignScores_TopScoreBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			else if (guiMode == ReignScores_GuiModes.ShowingAchievements)
			{
				if (desc.ReignScores_AchievementBoardTexture != null)
				{
					// draw board
					var size = fitInView(desc.ReignScores_AchievementBoardTexture.width, desc.ReignScores_AchievementBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), desc.ReignScores_AchievementBoardTexture);

					// get main scale value
					var mainScale = scaleToFitInView(desc.ReignScores_AchievementBoardTexture.width, desc.ReignScores_AchievementBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(desc.ReignScores_AchievementBoardFrame_CloseBox, desc.ReignScores_AchievementBoardButton_CloseNormal, desc.ReignScores_AchievementBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = ReignScores_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(desc.ReignScores_AchievementBoardFrame_PrevButton, desc.ReignScores_AchievementBoardButton_PrevNormal, desc.ReignScores_AchievementBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset != 0)
						{
							guiAchievementOffset -= desc.ReignScores_AchievementsToListPerPage;
							if (guiAchievementOffset < 0) guiAchievementOffset = 0;
						}
					}

					if (processButton(desc.ReignScores_AchievementBoardFrame_NextButton, desc.ReignScores_AchievementBoardButton_NextNormal, desc.ReignScores_AchievementBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset + desc.ReignScores_AchievementsToListPerPage < guiAchievements.Length)
						{
							guiAchievementOffset += desc.ReignScores_AchievementsToListPerPage;
						}
					}

					// draw names and scores
					var nameRect = calculateFrame(desc.ReignScores_AchievementBoardFrame_Names, mainScale, offsetX, offsetY);
					var descRect = calculateFrame(desc.ReignScores_AchievementBoardFrame_Descs, mainScale, offsetX, offsetY);
					if (desc.ReignScores_EnableTestRects)
					{
						GUI.Button(nameRect, "TEST RECT");
						GUI.Button(descRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(desc.ReignScores_AchievementBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = desc.ReignScores_AchievementBoardFont_Color;
					int nameI = 0, descI = 0;
					for (int i = guiAchievementOffset; i < guiAchievementOffset+desc.ReignScores_AchievementsToListPerPage; ++i)
					{
						if (i == guiAchievements.Length) break;
						var ach = guiAchievements[i];

						// icon
						float height = nameRect.height / desc.ReignScores_AchievementsToListPerPage;
						float iconSize = height * .8f;
						GUI.DrawTexture(new Rect(nameRect.x, nameRect.y + nameI + height - iconSize, iconSize, iconSize), ach.IsAchieved ? ach.AchievedImage : ach.UnachievedImage);

						// name
						GUI.Label(new Rect(height + nameRect.x, nameRect.y + nameI, nameRect.width, height), ach.Name, style);
						nameI += (int)height;

						// desc
						height = descRect.height / desc.ReignScores_AchievementsToListPerPage;
						GUI.Label(new Rect(descRect.x, descRect.y + descI, descRect.width, height), ach.Desc, style);
						descI += (int)height;
					}
				}
				else
				{
					errorText = "ReignScores_AchievementBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			else if (guiMode == ReignScores_GuiModes.CreatingUser)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Creating User...", style);
			}
			else if (guiMode == ReignScores_GuiModes.LoadingScores || guiMode == ReignScores_GuiModes.LoadingAchievements)
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
				if (desc.ReignScores_AudioSource != null && desc.ReignScores_ButtonClick != null) desc.ReignScores_AudioSource.PlayOneShot(desc.ReignScores_ButtonClick);
				return true;
			}

			return false;
		}

		private void guiAuthenticateCallbackTEMP(bool success, string errorMessage)
		{
			if (success)
			{
				PerformingGUIOperation = false;
				guiMode = ReignScores_GuiModes.None;
				if (guiAuthenticateCallback != null) guiAuthenticateCallback(true, null);
			}
			else
			{
				Debug.LogError(errorMessage);
				errorText = errorMessage;
				if (guiMode == ReignScores_GuiModes.LoggingIn) guiMode = ReignScores_GuiModes.Login;
				else if (guiMode == ReignScores_GuiModes.CreatingUser) guiMode = ReignScores_GuiModes.CreateUser;
			}
		}

		public void Update()
		{
			// do nothing...
		}
	}
}

