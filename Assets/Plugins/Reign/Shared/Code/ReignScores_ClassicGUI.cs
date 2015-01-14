using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
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

	public class ReignScores_ClassicGUI : MonoBehaviour, IScores_UI
	{
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

		public void RequestLogin()
		{
			
		}

		public void LoginCallback(bool succeeded)
		{
			
		}






		private ReignScores_GuiModes guiMode = ReignScores_GuiModes.None;
		private AuthenticateCallbackMethod guiAuthenticateCallback;
		private MonoBehaviour guiServices;

		// =========================================================================================================
		// =========================================================================================================
		// =========================================================================================================
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
		//public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		//{
		//	guiMode = ReignScores_GuiModes.LoadingScores;
		//	PerformingGUIOperation = true;
		//	guiShowNativeViewDoneCallback = callback;
		//	guiLeaderboardID = leaderboardID;
		//	guiScoreOffset = 0;
		//	RequestScores(leaderboardID, guiScoreOffset, ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, services);
		//}

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
		//public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		//{
		//	guiMode = ReignScores_GuiModes.LoadingAchievements;
		//	PerformingGUIOperation = true;
		//	guiAchievementOffset = 0;
		//	guiShowNativeViewDoneCallback = callback;
		//	RequestAchievements(guiRequestAchievementsCallback, services);
		//}

		private string userAccount_Name = "", userAccount_Pass = "", userAccount_ConfPass = "", errorText;
		void OnGUI()
		{
			if (guiMode == ReignScores_GuiModes.None) return;

			GUI.color = Color.white;
			GUI.matrix = Matrix4x4.identity;
			float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

			// draw background
			if (ReignScores_BackgroudTexture != null)
			{
				var size = MathUtilities.FillView(ReignScores_BackgroudTexture.width, ReignScores_BackgroudTexture.height, Screen.width, Screen.height);
				float offsetX = -Mathf.Max((size.x-Screen.width)*.5f, 0f);
				float offsetY = -Mathf.Max((size.y-Screen.height)*.5f, 0f);
				GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), ReignScores_BackgroudTexture);
			}

			float buttonWidth = 128 * scale;
			float buttonHeight = 64 * scale;
			float textWidth = 256 * scale;
			float textHeight = 32 * scale;
			float y = Screen.height / 2;
			if (guiMode == ReignScores_GuiModes.Login)
			{
				// title
				if (!string.IsNullOrEmpty(ReignScores_LoginTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), ReignScores_LoginTitle, style);
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
						//guiServices.StartCoroutine(login(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
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
				if (!string.IsNullOrEmpty(ReignScores_CreateUserTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), ReignScores_CreateUserTitle, style);
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
						//guiServices.StartCoroutine(createUser(userAccount_Name, userAccount_Pass, guiAuthenticateCallbackTEMP));
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
				if (ReignScores_TopScoreBoardTexture != null)
				{
					// draw board
					var size = MathUtilities.FitInView(ReignScores_TopScoreBoardTexture.width, ReignScores_TopScoreBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), ReignScores_TopScoreBoardTexture);

					// get main scale value
					var mainScale = MathUtilities.ScaleToFitInView(ReignScores_TopScoreBoardTexture.width, ReignScores_TopScoreBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(ReignScores_TopScoreBoardFrame_CloseBox, ReignScores_TopScoreBoardButton_CloseNormal, ReignScores_TopScoreBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = ReignScores_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(ReignScores_TopScoreBoardFrame_PrevButton, ReignScores_TopScoreBoardButton_PrevNormal, ReignScores_TopScoreBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiScoreOffset != 0)
						{
							guiScoreOffset -= ReignScores_TopScoresToListPerPage;
							if (guiScoreOffset < 0) guiScoreOffset = 0;
							RequestScores(guiLeaderboardID, guiScoreOffset, ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					if (processButton(ReignScores_TopScoreBoardFrame_NextButton, ReignScores_TopScoreBoardButton_NextNormal, ReignScores_TopScoreBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiScores.Length == ReignScores_TopScoresToListPerPage)
						{
							guiScoreOffset += ReignScores_TopScoresToListPerPage;
							RequestScores(guiLeaderboardID, guiScoreOffset, ReignScores_TopScoresToListPerPage, guiRequestScoresCallback, guiServices);
						}
					}

					// draw names and scores
					var usernameRect = calculateFrame(ReignScores_TopScoreBoardFrame_Usernames, mainScale, offsetX, offsetY);
					var scoreRect = calculateFrame(ReignScores_TopScoreBoardFrame_Scores, mainScale, offsetX, offsetY);
					if (ReignScores_EnableTestRects)
					{
						GUI.Button(usernameRect, "TEST RECT");
						GUI.Button(scoreRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(ReignScores_TopScoreBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = ReignScores_TopScoreBoardFont_Color;
					int userI = 0, scoreI = 0;
					foreach (var score in guiScores)
					{
						// username
						float height = usernameRect.height / ReignScores_TopScoresToListPerPage;
						GUI.Label(new Rect(usernameRect.x, usernameRect.y + userI, usernameRect.width, height), score.UserName, style);
						userI += (int)height;

						// score
						height = scoreRect.height / ReignScores_TopScoresToListPerPage;
						string scoreValue;
						if (ReignScores_ScoreFormatCallback != null) ReignScores_ScoreFormatCallback(score.Score, out scoreValue);
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
				if (ReignScores_AchievementBoardTexture != null)
				{
					// draw board
					var size = MathUtilities.FitInView(ReignScores_AchievementBoardTexture.width, ReignScores_AchievementBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), ReignScores_AchievementBoardTexture);

					// get main scale value
					var mainScale = MathUtilities.ScaleToFitInView(ReignScores_AchievementBoardTexture.width, ReignScores_AchievementBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(ReignScores_AchievementBoardFrame_CloseBox, ReignScores_AchievementBoardButton_CloseNormal, ReignScores_AchievementBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						PerformingGUIOperation = false;
						guiMode = ReignScores_GuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(ReignScores_AchievementBoardFrame_PrevButton, ReignScores_AchievementBoardButton_PrevNormal, ReignScores_AchievementBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset != 0)
						{
							guiAchievementOffset -= ReignScores_AchievementsToListPerPage;
							if (guiAchievementOffset < 0) guiAchievementOffset = 0;
						}
					}

					if (processButton(ReignScores_AchievementBoardFrame_NextButton, ReignScores_AchievementBoardButton_NextNormal, ReignScores_AchievementBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset + ReignScores_AchievementsToListPerPage < guiAchievements.Length)
						{
							guiAchievementOffset += ReignScores_AchievementsToListPerPage;
						}
					}

					// draw names and scores
					var nameRect = calculateFrame(ReignScores_AchievementBoardFrame_Names, mainScale, offsetX, offsetY);
					var descRect = calculateFrame(ReignScores_AchievementBoardFrame_Descs, mainScale, offsetX, offsetY);
					if (ReignScores_EnableTestRects)
					{
						GUI.Button(nameRect, "TEST RECT");
						GUI.Button(descRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(ReignScores_AchievementBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = ReignScores_AchievementBoardFont_Color;
					int nameI = 0, descI = 0;
					for (int i = guiAchievementOffset; i < guiAchievementOffset+ReignScores_AchievementsToListPerPage; ++i)
					{
						if (i == guiAchievements.Length) break;
						var ach = guiAchievements[i];

						// icon
						float height = nameRect.height / ReignScores_AchievementsToListPerPage;
						float iconSize = height * .8f;
						GUI.DrawTexture(new Rect(nameRect.x, nameRect.y + nameI + height - iconSize, iconSize, iconSize), ach.IsAchieved ? ach.AchievedImage : ach.UnachievedImage);

						// name
						GUI.Label(new Rect(height + nameRect.x, nameRect.y + nameI, nameRect.width, height), ach.Name, style);
						nameI += (int)height;

						// desc
						height = descRect.height / ReignScores_AchievementsToListPerPage;
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
				if (ReignScores_AudioSource != null && ReignScores_ButtonClick != null) ReignScores_AudioSource.PlayOneShot(ReignScores_ButtonClick);
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
	}
}