#if UNITY_ANDROID
using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
	public class SocialPlugin_Android : ISocialPlugin
	{
		private AndroidJavaClass native;

		public SocialPlugin_Android()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.SocialNative");
		}

		public void Init(SocialDesc desc)
		{
			// do nothing...
		}

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			if (type == SocialShareTypes.Image_PNG || type == SocialShareTypes.Image_JPG) native.CallStatic("ShareImage", data, title, type == SocialShareTypes.Image_PNG);
			else Debug.LogError("Unusported Share type: " + type);
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			Share(data, title, desc, type);
		}
	}
}
#endif