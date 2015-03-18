using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	public static class PushNotificationManager
	{
		private static IPushNotificationPlugin plugin;

		static PushNotificationManager()
		{
			#if UNITY_WINRT
			plugin = new PushNotificationPlugin_WinRT();
			#endif
		}

		public static void Init(PushNotificationsDesc desc)
		{
			plugin.Init(desc);
		}
	}
}