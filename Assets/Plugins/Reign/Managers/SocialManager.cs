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
			#else
			plugin = new SocialPlugin_Dumy();
			#endif
		}

		public static void ShareImage(byte[] data, SocialShareTypes type)
		{
			plugin.Share(data, type);
		}
	}

	public class SocialPlugin_Dumy : ISocialPlugin
	{
		public void Share(byte[] data, SocialShareTypes type)
		{
			Debug.Log("ShareImage not supported in this environment!");
		}
	}
}