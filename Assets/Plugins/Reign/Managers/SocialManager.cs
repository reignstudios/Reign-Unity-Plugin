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
			#elif UNITY_IOS
			plugin = new SocialPlugin_iOS();
			#else
			plugin = new SocialPlugin_Dumy();
			#endif
		}

		public static void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			plugin.Share(data, title, desc, type);
		}

		public static void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			plugin.Share(data, title, desc, x, y, width, height, type);
		}
	}

	public class SocialPlugin_Dumy : ISocialPlugin
	{
		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Debug.Log("Share not supported in this environment!");
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			Debug.Log("Share not supported in this environment!");
		}
	}
}