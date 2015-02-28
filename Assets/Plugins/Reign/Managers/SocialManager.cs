using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	public static class SocialManager
	{
		private static ISocialPlugin plugin;

		static SocialManager()
		{
			#if UNITY_EDITOR
			plugin = new SocialPlugin_Dumy();
			#elif UNITY_WINRT
			plugin = new SocialPlugin_WinRT();
			#elif UNITY_ANDROID
			plugin = new SocialPlugin_Android();
			#else
			plugin = new SocialPlugin_Dumy();
			#endif
		}

		public static void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			plugin.Share(data, title, desc, type);
		}
	}

	public class SocialPlugin_Dumy : ISocialPlugin
	{
		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Debug.Log("Share not supported in this environment!");
		}
	}
}