#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class InterstitialAdPlugin : IInterstitialAdPlugin
	{
		private InterstitialAdEventCallbackMethod eventCallback;
		private bool testing, showing;

		public InterstitialAdPlugin(InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod callback)
		{
			testing = desc.Testing;
			eventCallback = desc.EventCallback;
			if (callback != null) callback(true);
		}

		public void Dispose()
		{
			// do nothing...
		}
		
		public void Cache()
		{
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Cached, null);
		}

		public void Show()
		{
			showing = true;
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Shown, null);
		}

		public void OnGUI()
		{
			if (showing && testing)
			{
				float size = Screen.width <= Screen.height ? Screen.width : Screen.height;
				size *= .9f;
				var rect = new Rect((Screen.width-size)*.5f, (Screen.height-size)*.5f, size, size);
				if (GUI.Button(rect, "Editor Interstitial Ad"))
				{
					showing = false;
					if (eventCallback != null) eventCallback(InterstitialAdEvents.Clicked, null);
				}

				if (GUI.Button(new Rect(rect.xMax, rect.yMin, 64, 64), "Close"))
				{
					showing = false;
					if (eventCallback != null) eventCallback(InterstitialAdEvents.Canceled, null);
				}
			}
		}

		public void Update()
		{
			// do nothing...
		}
	}
}
#endif