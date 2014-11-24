#if UNITY_BLACKBERRY && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Reign.Plugin
{
	struct SC_InitData_t
	{
	    public int currentVersion;
	    public int minimumRequiredVersion;
	    public IntPtr logWriter;
	    public int runLoopType;
	    public uint eventDomain;
	    public IntPtr eventNotifier;
	    public IntPtr mainDispatchQueue;
	    public IntPtr clientType;
	    public IntPtr eventNotifierContext;
	}
	
	struct SC_ScoresSearchList_t
	{
	    public int timeInterval;
	    public int countrySelector;
	    public IntPtr country;
	    public int usersSelector;
	    public IntPtr buddyhoodUser;
	}
	
	struct SC_Range_t
	{
	    public uint offset;
	    public uint length;
	}

	public class Scoreloop_ScorePlugin : IScorePlugin
	{
		private static ScoreDesc desc;
		public bool PerformingGUIOperation {get; private set;}

		private static bool isAuthenticated;
		public bool IsAuthenticated {get{return isAuthenticated;}}

		private static string userID;
		public string UserID {get{return userID;}}

		private static AuthenticateCallbackMethod authenticatedCallback;
		private static ReportScoreCallbackMethod reportScoreCallback;
		private static RequestScoresCallbackMethod requestScoresCallback;
		private static ReportAchievementCallbackMethod reportAchievementCallback;
		private static RequestAchievementsCallbackMethod requestAchievementsCallback;
		private static LeaderboardScore[] scores;
		private static Achievement[] Achievements;
		
		private SC_InitData_t initData;
		private static IntPtr client, uiClient, userController, scoreObj, scoreController, scoresController, achievementsController;
		private static bool nativeGUISupported;
		
		// constants
		private const int SC_INIT_VERSION_1_0 = 0x100;
		
		// callbacks
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		private delegate void SC_RequestControllerCompletionCallback_t(IntPtr userData, uint completionStatus);
		private static SC_RequestControllerCompletionCallback_t authenticateCallbackObject;
		private static SC_RequestControllerCompletionCallback_t reportScoreCallbackObject, requestScoresCallbackObject;
		private static SC_RequestControllerCompletionCallback_t reportAchievementCallbackObject, requestAchievementsCallbackObject;

		private delegate void SCUI_ViewResultCallback_t(IntPtr cookie, int viewResult, IntPtr data);
		private static SCUI_ViewResultCallback_t authenticateUICallbackObject;
		
		// init
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_InitData_InitWithSizeAndVersion")]
		private static extern void SC_InitData_InitWithSizeAndVersion(ref SC_InitData_t data, uint initDataSize, int currentVersion);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_Release")]
		private static extern void SC_Client_Release(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_String_GetData")]
		private static extern IntPtr SC_String_GetData(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_String_Release")]
		private static extern void SC_String_Release(IntPtr self);
		
		// Authentication
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_GetVersionInfo")]
		private unsafe static extern bool SC_GetVersionInfo(ref SC_InitData_t pInitData, byte* buffer, uint bufferSize);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_New")]
		private static extern uint SC_Client_New(ref IntPtr pSelf, ref SC_InitData_t initData, string gameIdentifier, string gameSecret, string gameVersion, string currency, string languages);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_CreateUserController")]
		private static extern uint SC_Client_CreateUserController(IntPtr self, ref IntPtr pUserController, IntPtr callback, IntPtr cookie);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_UserController_LoadUser")]
		private static extern int SC_UserController_LoadUser(IntPtr initData);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_UserController_Release")]
		private static extern void SC_UserController_Release(IntPtr initData);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_GetSession")]
		private static extern IntPtr SC_Client_GetSession(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Session_GetUser")]
		private static extern IntPtr SC_Session_GetUser(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_User_GetLogin")]
		private static extern IntPtr SC_User_GetLogin(IntPtr self);
		
		// Scores
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_CreateScoresController")]
		private static extern int SC_Client_CreateScoresController(IntPtr self, ref IntPtr pScoresController, IntPtr callback, IntPtr cookie);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoresController_Release")]
		private static extern void SC_ScoresController_Release(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoresController_SetMode")]
		private static extern int SC_ScoresController_SetMode(IntPtr self, uint mode);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoresController_SetSearchList")]
		private static extern int SC_ScoresController_SetSearchList(IntPtr self, SC_ScoresSearchList_t searchList);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoresController_LoadScores")]
		private static extern int SC_ScoresController_LoadScores(IntPtr self, SC_Range_t range);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_GetMode")]
		private static extern uint SC_Score_GetMode(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoresController_GetScores")]
		private static extern IntPtr SC_ScoresController_GetScores(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_GetScoreFormatter")]
		private static extern IntPtr SC_Client_GetScoreFormatter(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoreList_GetCount")]
		private static extern uint SC_ScoreList_GetCount(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoreList_GetAt")]
		private static extern IntPtr SC_ScoreList_GetAt(IntPtr self, uint index);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_GetUser")]
		private static extern IntPtr SC_Score_GetUser(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_GetRank")]
		private static extern uint SC_Score_GetRank(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_GetResult")]
		private static extern double SC_Score_GetResult(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_CreateScoreController")]
		private static extern int SC_Client_CreateScoreController(IntPtr self, ref IntPtr pScoreController, IntPtr callback, IntPtr cookie);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_CreateScore")]
		private static extern int SC_Client_CreateScore(IntPtr self, ref IntPtr pScore);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoreController_Release")]
		private static extern void SC_ScoreController_Release(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_SetResult")]
		private static extern int SC_Score_SetResult(IntPtr self, double result);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_SetMode")]
		private static extern int SC_Score_SetMode(IntPtr self, uint result);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_ScoreController_SubmitScore")]
		private static extern int SC_ScoreController_SubmitScore(IntPtr self, IntPtr score);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Score_Release")]
		private static extern void SC_Score_Release(IntPtr self);
		
		// Achievements
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Client_CreateLocalAchievementsController")]
		private static extern int SC_Client_CreateLocalAchievementsController(IntPtr self, ref IntPtr pAchievementsController, IntPtr callback, IntPtr cookie);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_AchievementList_GetCount")]
		private static extern uint SC_AchievementList_GetCount(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_Release")]
		private static extern void SC_LocalAchievementsController_Release(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_GetAchievements")]
		private static extern IntPtr SC_LocalAchievementsController_GetAchievements(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_AchievementList_GetAt")]
		private static extern IntPtr SC_AchievementList_GetAt(IntPtr self, uint index);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Achievement_GetAward")]
		private static extern IntPtr SC_Achievement_GetAward(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_GetIdentifier")]
		private static extern IntPtr SC_Award_GetIdentifier(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_GetLocalizedTitle")]
		private static extern IntPtr SC_Award_GetLocalizedTitle(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_GetLocalizedDescription")]
		private static extern IntPtr SC_Award_GetLocalizedDescription(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_GetAchievedImageName")]
		private static extern IntPtr SC_Award_GetAchievedImageName(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_GetUnachievedImageName")]
		private static extern IntPtr SC_Award_GetUnachievedImageName(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_Award_IsAchievedByValue")]
		private static extern bool SC_Award_IsAchievedByValue(IntPtr self, int value);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_IsAchievedForAwardIdentifier")]
		private static extern bool SC_LocalAchievementsController_IsAchievedForAwardIdentifier(IntPtr self, string awardIdentifier);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_SetAchievedValueForAwardIdentifier")]
		private static extern int SC_LocalAchievementsController_SetAchievedValueForAwardIdentifier(IntPtr self, string awardIdentifier, ref bool pGetAchieved);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_ShouldSynchronize")]
		private static extern bool SC_LocalAchievementsController_ShouldSynchronize(IntPtr self);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_LocalAchievementsController_Synchronize")]
		private static extern int SC_LocalAchievementsController_Synchronize(IntPtr self);
		
		// events
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_GetBPSEventDomain")]
		private static extern int SC_GetBPSEventDomain(ref SC_InitData_t initData);
		
		[DllImport("libscoreloopcore.so.1", EntryPoint="SC_HandleBPSEvent")]
		private static extern int SC_HandleBPSEvent(ref SC_InitData_t initData, IntPtr _event);

		// native views
		[DllImport("libscoreloopcore.so.1", EntryPoint="SCUI_Client_NewWithCallback")]
		private static extern int SCUI_Client_NewWithCallback(ref IntPtr pSelf, IntPtr client, IntPtr callback, IntPtr cookie);

		[DllImport("libscoreloopcore.so.1", EntryPoint="SCUI_Client_Release")]
		private static extern void SCUI_Client_Release(IntPtr self);

		[DllImport("libscoreloopcore.so.1", EntryPoint="SCUI_Client_HandleEvent")]
		private static extern bool SCUI_Client_HandleEvent(IntPtr self, IntPtr _event);

		[DllImport("libscoreloopcore.so.1", EntryPoint="SCUI_Client_ShowLeaderboardView")]
		private static extern int SCUI_Client_ShowLeaderboardView(IntPtr self, uint modeSelected, int listSelected, IntPtr optionalScore);

		[DllImport("libscoreloopcore.so.1", EntryPoint="SCUI_Client_ShowAchievementsView")]
		private static extern int SCUI_Client_ShowAchievementsView(IntPtr self);
	
		public Scoreloop_ScorePlugin(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			try
			{
				Scoreloop_ScorePlugin.desc = desc;
				SC_InitData_InitWithSizeAndVersion(ref initData, (uint)Marshal.SizeOf(initData), SC_INIT_VERSION_1_0);
				
				// create callback methods
				authenticateCallbackObject = new SC_RequestControllerCompletionCallback_t(authenticateCallbackNative);
				authenticateUICallbackObject = new SCUI_ViewResultCallback_t(authenticateUICallbackNative);
				reportScoreCallbackObject = new SC_RequestControllerCompletionCallback_t(reportScoreCallbackNative);
				requestScoresCallbackObject = new SC_RequestControllerCompletionCallback_t(requestScoresCallbackNative);
				reportAchievementCallbackObject = new SC_RequestControllerCompletionCallback_t(achieveAchievementCallbackNative);
				requestAchievementsCallbackObject = new SC_RequestControllerCompletionCallback_t(requestAchievementsCallbackNative);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (callback != null) callback(false, e.Message);
				return;
			}

			if (callback != null) callback(true, null);
		}
		
		~Scoreloop_ScorePlugin()
		{
			if (uiClient != IntPtr.Zero)
			{
				SCUI_Client_Release(uiClient);
				uiClient = IntPtr.Zero;
			}

			if (client != IntPtr.Zero)
			{
				SC_Client_Release(client);
				client = IntPtr.Zero;	
			}
		}
		
		public void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			isAuthenticated = false;
			authenticatedCallback = callback;
			
			unsafe
			{
				var version = new byte[256];
				fixed (byte* ptr = version)
				{
					if (SC_GetVersionInfo(ref initData, ptr, 256)) Debug.Log("Scoreloop Version: " + Marshal.PtrToStringAnsi(new IntPtr(ptr)));
				}
			}
			
			if (SC_Client_New(ref client, ref initData, desc.BB10_Scoreloop_ID, desc.BB10_Scoreloop_Secret, desc.BB10_Scoreloop_Version, desc.BB10_Scoreloop_Currency, desc.BB10_Scoreloop_Language) != 0)
			{
				string error = "SC_Client_New Failed.";
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}
			
			if (SC_Client_CreateUserController(client, ref userController, Marshal.GetFunctionPointerForDelegate(authenticateCallbackObject), IntPtr.Zero) != 0)
			{
				string error = "SC_Client_CreateUserController Failed.";
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}
			
			if (SC_UserController_LoadUser(userController) != 0)
			{
				string error = "SC_UserController_LoadUser Failed.";
				Debug.LogError(error);
				SC_UserController_Release(userController);
				if (callback != null) callback(false, error);
				return;
			}
		}

		private static void authenticateCallbackNative(IntPtr userData, uint completionStatus)
		{
			if (completionStatus != 0)
			{
				string error = "authenticateCallbackNative Failed.";
				Debug.LogError(error);
				SC_UserController_Release(userController);
				if (authenticatedCallback != null) authenticatedCallback(false, error);
				return;
			}

			var session = SC_Client_GetSession(client);
			var user = SC_Session_GetUser(session);
			var login = SC_User_GetLogin(user);
			var name = SC_String_GetData(login);
			userID = Marshal.PtrToStringAnsi(name);
			Debug.Log("UserID: " + userID);

			SC_UserController_Release(userController);

			nativeGUISupported = false;
			try
			{
				if (SCUI_Client_NewWithCallback(ref uiClient, client, Marshal.GetFunctionPointerForDelegate(authenticateUICallbackObject), IntPtr.Zero) != 0)
				{
					Debug.LogError("SCUI_Client_NewWithCallback failed");
				}
				else
				{
					nativeGUISupported = true;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("SCUI_Client_NewWithCallback failed: " + e.Message);
			}

			isAuthenticated = true;
			if (authenticatedCallback != null) authenticatedCallback(true, null);
		}

		// this never gets called for some reason?
		private static void authenticateUICallbackNative(IntPtr cookie, int viewResult, IntPtr data)
		{
			/*if (viewResult != 0)
			{
				string error = "authenticateUICallbackNative Failed: " + viewResult;
				Debug.LogError(error);
				nativeGUISupported = false;
			}
			else
			{
				nativeGUISupported = true;
			}

			isAuthenticated = true;
			if (authenticatedCallback != null) authenticatedCallback(true, null);*/
		}

		public void Logout()
		{
			isAuthenticated = false;
			userID = "???";
			if (uiClient != IntPtr.Zero)
			{
				SCUI_Client_Release(uiClient);
				uiClient = IntPtr.Zero;
			}

			if (client != IntPtr.Zero)
			{
				SC_Client_Release(client);
				client = IntPtr.Zero;	
			}
		}
		
		public void ManualCreateUser (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			Debug.LogError("ManualCreateUser not supported in Scoreloop.");
		}

		public void ManualLogin (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			Debug.LogError("ManualLogin not supported in Scoreloop.");
		}
		
		private int findLeadboardMode(string id)
		{
			foreach (var l in Scoreloop_ScorePlugin.desc.LeaderboardDescs)
			{
				if (l.ID == id)
				{
					return l.BB10_Scoreloop_Mode;
				}
			}
			
			return -1;
		}
	
		public void ReportScore(string leaderboardID, int score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			reportScoreCallback = callback;
		    if (SC_Client_CreateScoreController(client, ref scoreController, Marshal.GetFunctionPointerForDelegate(reportScoreCallbackObject), IntPtr.Zero) != 0)
		    {
				string error = "SC_Client_CreateScoreController Failed.";
		        Debug.LogError(error);
		        if (callback != null) callback(false, error);
		        return;
		    }
			
		    if (SC_Client_CreateScore(client, ref scoreObj) != 0)
		    {
		        SC_ScoreController_Release(scoreController);
				string error = "SC_Client_CreateScore Failed.";
		        Debug.LogError(error);
		        if (callback != null) callback(false, error);
		        return;
		    }
		    
		    // find mode
			int mode = findLeadboardMode(leaderboardID);
			if (mode == -1)
			{
				string error = "Failed to find mode for LeaderboardID.";
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}
		    
		    if (SC_Score_SetMode(scoreObj, (uint)mode) != 0)
		    {
				string error = "SC_Score_SetMode Failed.";
		    	Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
		    }
		    
		    if (SC_Score_SetResult(scoreObj, score) != 0)
		    {
				string error = "SC_Score_SetResult Failed.";
		    	Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
		    }
		
		    if (SC_ScoreController_SubmitScore(scoreController, scoreObj) != 0)
		    {
		        SC_ScoreController_Release(scoreController);
		        SC_Score_Release(scoreObj);
				string error = "SC_ScoreController_SubmitScore Failed.";
		        Debug.LogError(error);
		        if (callback != null) callback(false, error);
		        return;
		    }
		}
		
		private static void reportScoreCallbackNative(IntPtr userData, uint completionStatus)
		{
		    if (completionStatus != 0)
		    {
				string error = "reportScoreCallbackNative Failed.";
				Debug.LogError(error);
		        SC_ScoreController_Release(scoreController);
		        SC_Score_Release(scoreObj);
				if (reportScoreCallback != null) reportScoreCallback(false, error);
		        return;
		    }
		    
		    SC_ScoreController_Release(scoreController);
		    SC_Score_Release(scoreObj);
			if (reportScoreCallback != null) reportScoreCallback(true, null);
		}
		
		public void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			requestScoresCallback = callback;
		    if (SC_Client_CreateScoresController(client, ref scoresController, Marshal.GetFunctionPointerForDelegate(requestScoresCallbackObject), IntPtr.Zero) != 0)
		    {
				string error = "SC_Client_CreateScoresController Failed.";
		    	Debug.LogError(error);
		        if (callback != null) callback(null, false, error);
		        return;
		    }
		
			// find mode
			int mode = findLeadboardMode(leaderboardID);
			if (mode == -1)
			{
				string error = "Failed to find mode for LeaderboardID.";
				Debug.LogError(error);
				if (callback != null) callback(null, false, error);
				return;
			}
			
		    SC_ScoresController_SetMode(scoresController, (uint)mode);
		    if (SC_ScoresController_SetSearchList(scoresController, new SC_ScoresSearchList_t()) != 0)
		    {
				string error = "SC_ScoresController_SetSearchList Failed.";
		    	Debug.LogError(error);
		        SC_ScoresController_Release(scoresController);
		        if (callback != null) callback(null, false, error);
		        return;
		    }
		
			var rangeObject = new SC_Range_t();
			rangeObject.offset = (uint)offset;
			rangeObject.length = (uint)range;
		    if (SC_ScoresController_LoadScores(scoresController, rangeObject) != 0)
		    {
				string error = "SC_ScoresController_LoadScores Failed.";
		    	Debug.LogError(error);
		        SC_ScoresController_Release(scoresController);
		        if (callback != null) callback(null, false, error);
		        return;
		    }
		}
		
		private static void requestScoresCallbackNative(IntPtr userData, uint completionStatus)
		{
		    if (completionStatus != 0)
		    {
				string error = "requestScoresCallbackNative Failed.";
				Debug.LogError(error);
		        SC_ScoresController_Release(scoresController);
				if (requestScoresCallback != null) requestScoresCallback(null, false, error);
		        return;
		    }
		
		    IntPtr scoreList = SC_ScoresController_GetScores(scoresController);
		    if (scoreList == IntPtr.Zero)
		    {
				string error = "SC_ScoresController_GetScores Failed.";
				Debug.LogError(error);
		        SC_ScoresController_Release(scoresController);
				if (requestScoresCallback != null) requestScoresCallback(null, false, error);
		        return;
		    }
		
		    IntPtr scoreFormatter = SC_Client_GetScoreFormatter(client);
		    if (scoreFormatter == IntPtr.Zero)
		    {
				string error = "SC_Client_GetScoreFormatter Failed.";
				Debug.LogError(error);
		        SC_ScoresController_Release(scoresController);
				if (requestScoresCallback != null) requestScoresCallback(null, false, error);
		        return;
		    }
		
		    uint i, numScores = SC_ScoreList_GetCount(scoreList);
		    scores = new LeaderboardScore[numScores];
		    for (i = 0; i < numScores; ++i)
		    {
		        IntPtr score = SC_ScoreList_GetAt(scoreList, i);
		        IntPtr user = SC_Score_GetUser(score);
		        IntPtr login = user != IntPtr.Zero ? SC_User_GetLogin(user) : IntPtr.Zero;
		        
		        string authenticateUserID = "???";
		        if (login != IntPtr.Zero)
		        {
			        var name = SC_String_GetData(login);
	    			authenticateUserID = Marshal.PtrToStringAnsi(name);
    			}
		        scores[i] = new LeaderboardScore(authenticateUserID, (int)SC_Score_GetResult(score));//, (int)SC_Score_GetRank(score));
		    }
		
		    SC_ScoresController_Release(scoresController);
			if (requestScoresCallback != null) requestScoresCallback(scores, true, null);
		}
		
		public void ReportAchievement(string achievementID, ReportAchievementCallbackMethod callback, MonoBehaviour services)
		{
			reportAchievementCallback = callback;
		    if (SC_Client_CreateLocalAchievementsController(client, ref achievementsController, Marshal.GetFunctionPointerForDelegate(reportAchievementCallbackObject), IntPtr.Zero) != 0)
		    {
				string error = "SC_Client_CreateLocalAchievementsController Failed.";
		        Debug.LogError(error);
		        if (callback != null) callback(false, error);
		        return;
		    }
		    
		    // find Scoreloop achievement ID
		    string scoreloopAchievementID = null;
		    foreach (var a in Scoreloop_ScorePlugin.desc.AchievementDescs)
		    {
		    	if (a.ID == achievementID)
		    	{
		    		scoreloopAchievementID = a.BB10_Scoreloop_ID;
		    		break;
		    	}
		    }
		    
		    if (scoreloopAchievementID == null)
		    {
				string error = "Failed to find Scoreloop AchievementID.";
		    	Debug.LogError(error);
		    	if (callback != null) callback(false, error);
		    	return;
		    }
		
		    bool achieved = false;
		    if (SC_LocalAchievementsController_SetAchievedValueForAwardIdentifier(achievementsController, scoreloopAchievementID, ref achieved) != 0)
		    {
		        SC_LocalAchievementsController_Release(achievementsController);
				string error = "SC_LocalAchievementsController_SetAchievedValueForAwardIdentifier Failed.";
		        Debug.LogError(error);
		        if (callback != null) callback(false, error);
		        return;
		    }
		
		    if (SC_LocalAchievementsController_ShouldSynchronize(achievementsController))
		    {
		        if (SC_LocalAchievementsController_Synchronize(achievementsController) != 0)
		        {
		            SC_LocalAchievementsController_Release(achievementsController);
					string error = "SC_LocalAchievementsController_Synchronize Failed.";
		            Debug.LogError(error);
		            if (callback != null) callback(false, error);
		            return;
		        }
		    }
		    else
		    {
		        SC_LocalAchievementsController_Release(achievementsController);
		        if (callback != null) callback(false, null);
		    }
		}
		
		private static void achieveAchievementCallbackNative(IntPtr userData, uint completionStatus)
		{
		    if (completionStatus != 0)
		    {
				string error = "achieveAchievementCallbackNative Failed.";
				Debug.LogError(error);
		        SC_LocalAchievementsController_Release(achievementsController);
				if (reportAchievementCallback != null) reportAchievementCallback(false, error);
		        return;
		    }
		
		    SC_LocalAchievementsController_Release(achievementsController);
			if (reportAchievementCallback != null) reportAchievementCallback(true, null);
		}
		
		public void RequestAchievements(RequestAchievementsCallbackMethod callback)
		{
			requestAchievementsCallback = callback;
		
			if (!IsAuthenticated)
			{
				string error = "RequestAchievements Failed because user is not authenticated.";
				Debug.LogError(error);
				if (callback != null) callback(null, false, error);
				return;
			}
		
			if (SC_Client_CreateLocalAchievementsController(client, ref achievementsController, Marshal.GetFunctionPointerForDelegate(requestAchievementsCallbackObject), IntPtr.Zero) != 0)
			{
				string error = "SC_Client_CreateLocalAchievementsController Failed.";
				Debug.LogError(error);
				if (callback != null) callback(null, false, error);
				return;
			}
			
			Achievements = null;
			requestAchievementsCallbackNative(IntPtr.Zero, 0);// For some reason we just skip the callback here...
		}
		
		private static void requestAchievementsCallbackNative(IntPtr userData, uint completionStatus)
		{
		    if (completionStatus != 0)
		    {
				string error = "requestAchievementsCallbackNative Failed.";
				Debug.LogError(error);
		        SC_LocalAchievementsController_Release(achievementsController);
				if (requestAchievementsCallback != null) requestAchievementsCallback(null, false, error);
		        return;
		    }
		
		    var achievementList = SC_LocalAchievementsController_GetAchievements(achievementsController);
		    if (achievementList == IntPtr.Zero)
		    {
				string error = "SC_LocalAchievementsController_GetAchievements Failed.";
				Debug.LogError(error);
		        SC_LocalAchievementsController_Release(achievementsController);
				if (requestAchievementsCallback != null) requestAchievementsCallback(null, false, error);
		        return;
		    }
		
		    uint numAchievements = SC_AchievementList_GetCount(achievementList);
		    if (Achievements == null || Achievements.Length != numAchievements) Achievements = new Achievement[numAchievements];
		    for (uint i = 0; i < numAchievements; ++i)
		    {
		        var achievement = SC_AchievementList_GetAt(achievementList, i);
		        var award = SC_Achievement_GetAward(achievement);
		        string id = Marshal.PtrToStringAnsi(SC_String_GetData(SC_Award_GetIdentifier(award)));
		        string name = Marshal.PtrToStringAnsi(SC_String_GetData(SC_Award_GetLocalizedTitle(award)));
		        string desc = Marshal.PtrToStringAnsi(SC_String_GetData(SC_Award_GetLocalizedDescription(award)));
		        string achievedImageFile = Marshal.PtrToStringAnsi(SC_String_GetData(SC_Award_GetAchievedImageName(award)));
		        string unachievedImageFile = Marshal.PtrToStringAnsi(SC_String_GetData(SC_Award_GetUnachievedImageName(award)));
		        bool isAchieved = SC_LocalAchievementsController_IsAchievedForAwardIdentifier(achievementsController, id);
		        
		        // find overrides
		        foreach (var a in Scoreloop_ScorePlugin.desc.AchievementDescs)
		        {
		        	if (a.BB10_Scoreloop_ID == id)
		        	{
		        		if (a.BB10_Scoreloop_NameOverride != null) name = a.BB10_Scoreloop_NameOverride;
		        		if (a.BB10_Scoreloop_DescOverride != null) desc = a.BB10_Scoreloop_DescOverride;
		        		break;
		        	}
		        }
		        
		        UnityEngine.Texture achievedImage = null, unachievedImage = null;
	        	if (Achievements[i] != null)
	        	{
	        		achievedImage = Achievements[i].AchievedImage;
	        		unachievedImage = Achievements[i].UnachievedImage;
	        	}
	        	else
	        	{
			        string fileNameAchieved = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.Length-4)+"scoreloop/SLAwards.bundle/"+achievedImageFile;
			        string fileNameUnachieved = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.Length-4)+"scoreloop/SLAwards.bundle/"+unachievedImageFile;
			        achievedImage = new UnityEngine.Texture2D(4, 4);
			        unachievedImage = new UnityEngine.Texture2D(4, 4);
			        try
			        {
				        using (var file = new System.IO.FileStream(fileNameAchieved, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				        {
				        	var data = new byte[file.Length];
				        	file.Read(data, 0, data.Length);
				        	((UnityEngine.Texture2D)achievedImage).LoadImage(data);
				        }
				        
				        using (var file = new System.IO.FileStream(fileNameUnachieved, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				        {
				        	var data = new byte[file.Length];
				        	file.Read(data, 0, data.Length);
				        	((UnityEngine.Texture2D)unachievedImage).LoadImage(data);
				        }
			        }
			        catch(Exception e)
			        {
			        	Debug.Log("Achievement Texture load ERROR: " + e.Message);
			        }
		        }
		        
		        Achievements[i] = new Achievement(isAchieved, id, name, desc, achievedImage, unachievedImage);
		    }
		
		    SC_LocalAchievementsController_Release(achievementsController);
			if (requestAchievementsCallback != null) requestAchievementsCallback(Achievements, true, null);
		}
		
		public void ShowNativeAchievementsPage (ShowNativeViewDoneCallbackMethod callback)
		{
			if (!nativeGUISupported)
			{
				string error = "Native GUI not supported";
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}

			try
			{
				if (SCUI_Client_ShowAchievementsView(uiClient) != 0)
				{
					string error = "SCUI_Client_ShowAchievementsView Failed";
					Debug.LogError(error);
					if (callback != null) callback(false, error);
					return;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (callback != null) callback(false, e.Message);
				return;
			}

			if (callback != null) callback(true, null);
		}

		public void ShowNativeScoresPage (string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			if (!nativeGUISupported)
			{
				string error = "Native GUI not supported";
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}

			int mode = findLeadboardMode(leaderboardID);
			if (mode == -1)
			{
				string error = "Failed to find leaderboardID: " + leaderboardID;
				Debug.LogError(error);
				if (callback != null) callback(false, error);
				return;
			}

			try
			{
				if (SCUI_Client_ShowLeaderboardView(uiClient, (uint)mode, 0, IntPtr.Zero) != 0)
				{
					string error = "SCUI_Client_ShowLeaderboardView Failed";
					Debug.LogError(error);
					if (callback != null) callback(false, error);
					return;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (callback != null) callback(false, e.Message);
				return;
			}

			if (callback != null) callback(true, null);
		}
		
		public void OnGUI()
		{
			// do nothing...
		}

		public void Update()
		{
			// do nothing...
		}
	}
}
#endif