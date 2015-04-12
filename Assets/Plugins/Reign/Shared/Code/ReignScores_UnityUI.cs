using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Reign.Plugin
{
	/// <summary>
	/// Internal use only!
	/// </summary>
	enum ReignScores_UnityUIModes
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

	public class ReignScores_UnityUI : MonoBehaviour, IScores_UI
	{
		/// <summary>
		/// Panel
		/// </summary>
		public GameObject LoginPanel, CreateUserPanel;

		/// <summary>
		/// Loading spinner
		/// </summary>
		public Transform Spinner;

		/// <summary>
		/// Login Screen button
		/// </summary>
		public Button LoginScreen_LoginButton, LoginScreen_CreateUserButton, LoginScreen_CancelButton;

		/// <summary>
		/// Login Screen field
		/// </summary>
		public InputField LoginScreen_UsernameInput, LoginScreen_PasswordInput;

		/// <summary>
		/// Create User Screen button
		/// </summary>
		public Button CreateUserScreen_CreateButton, CreateUserScreen_CancleButton;

		/// <summary>
		/// Create User Screen field
		/// </summary>
		public InputField CreateUserScreen_UsernameInput, CreateUserScreen_PasswordInput, CreateUserScreen_PasswordInput2;

		// =========================================================================================================
		// IScores_UI interface methods
		// =========================================================================================================
		public event ScoreFormatCallbackMethod ScoreFormatCallback;
		private IScorePlugin plugin;
		private AuthenticateCallbackMethod authenticateCallback;

		public void Init(IScorePlugin plugin)
		{
			this.plugin = plugin;
			mode = ReignScores_UnityUIModes.None;
		}

		public void RequestLogin(AuthenticateCallbackMethod callback)
		{
			if (mode == ReignScores_UnityUIModes.None)
			{
				mode = ReignScores_UnityUIModes.Login;
				authenticateCallback = callback;
			}
		}

		public void AutoLogin(AuthenticateCallbackMethod callback)
		{
			mode = ReignScores_UnityUIModes.None;
			authenticateCallback = callback;
		}

		public void LoginCallback(bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				//errorText = "";
				mode = ReignScores_UnityUIModes.None;
				if (authenticateCallback != null) authenticateCallback(true, null);
			}
			else
			{
				//errorText = errorMessage != null ? errorMessage : "???";
				if (mode == ReignScores_UnityUIModes.LoggingIn) mode = ReignScores_UnityUIModes.Login;
				else if (mode == ReignScores_UnityUIModes.CreatingUser) mode = ReignScores_UnityUIModes.CreateUser;
			}
		}

		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback)
		{
			
		}

		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			
		}

		// =========================================================================================================
		// UI Interaction
		// =========================================================================================================
		private ReignScores_UnityUIModes _mode;
		private ReignScores_UnityUIModes mode
		{
			get {return _mode;}
			set
			{
				_mode = value;
				// TODO
			}
		}
	}
}