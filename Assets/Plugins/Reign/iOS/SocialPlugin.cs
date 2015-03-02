#if UNITY_IOS
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class SocialPlugin_iOS : ISocialPlugin
	{
		[DllImport("__Internal", EntryPoint="InitSocial")]
		private static extern void InitSocial();

		[DllImport("__Internal", EntryPoint="DisposeSocial")]
		private static extern void DisposeSocial();

		[DllImport("__Internal", EntryPoint="Social_ShareImage")]
		private static extern void Social_ShareImage(byte[] data, int dataLength, bool isPNG, int x, int y, int width, int height);

		public SocialPlugin_iOS()
		{
			InitSocial();
		}

		~SocialPlugin_iOS()
		{
			DisposeSocial();
		}

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Share(data, title, desc, 0, 0, 10, 10, type);
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			if (type == SocialShareTypes.Image_PNG || type == SocialShareTypes.Image_JPG) Social_ShareImage(data, data.Length, type == SocialShareTypes.Image_PNG, x, y, width, height);
			else Debug.LogError("Unusported Share type: " + type);
		}
	}
}
#endif