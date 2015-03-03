#if UNITY_BLACKBERRY
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Reign.Plugin
{
	public class SocialPlugin_BB10 : ISocialPlugin
	{
		private IntPtr invoke;

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Share(data, title, desc, 0, 0, 10, 10, type);
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			if (type == SocialShareTypes.Image_PNG || type == SocialShareTypes.Image_JPG)
			{
				if (Common.navigator_invoke_invocation_create(ref invoke) != 0) return;
				if (Common.navigator_invoke_invocation_set_target(invoke, "sys.bbm.sharehandler") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if (Common.navigator_invoke_invocation_set_action(invoke, "bb.action.SHARE") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				string filename = "data/ReignSocialImage" + (type == SocialShareTypes.Image_PNG ? ".png" : ".jpg");
				IntPtr filenamePtr = Marshal.StringToHGlobalAnsi(filename);
				try
				{
					using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
					{
						stream.Write(data, 0, data.Length);
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Failed to save share image: " + e.Message);
					return;
				}

				if (Common.navigator_invoke_invocation_set_data(invoke, filenamePtr, filename.Length) != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if (Common.navigator_invoke_invocation_send(invoke) != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				Common.navigator_invoke_invocation_destroy(invoke);
				Marshal.FreeHGlobal(filenamePtr);
			}
			else
			{
				Debug.LogError("Unusported Share type: " + type);
			}
		}
	}
}
#endif