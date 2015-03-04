#if UNITY_BLACKBERRY
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Reign.Plugin
{
	public class SocialPlugin_BB10 : ISocialPlugin
	{
		private IntPtr invoke;
		private SocialDesc desc;

		private byte[] shareData;
		private SocialShareTypes shareType;

		public void Init(SocialDesc desc)
		{
			this.desc = desc;
			desc.BB10_CloseButton.onClick.AddListener(closeButtonClicked);
			desc.BB10_ShareSelectorBBM.onClick.AddListener(bbmButtonClicked);
			desc.BB10_ShareSelectorFacebook.onClick.AddListener(facebookButtonClicked);
			desc.BB10_ShareSelectorTwitter.onClick.AddListener(twitterButtonClicked);
		}

		private void closeButtonClicked()
		{
			this.desc.BB10_ShareSelectorUI.SetActive(false);
		}

		private void bbmButtonClicked()
		{
			this.desc.BB10_ShareSelectorUI.SetActive(false);
			share(0);
		}

		private void facebookButtonClicked()
		{
			this.desc.BB10_ShareSelectorUI.SetActive(false);
			share(1);
		}

		private void twitterButtonClicked()
		{
			this.desc.BB10_ShareSelectorUI.SetActive(false);
			share(2);
		}

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Share(data, title, desc, 0, 0, 10, 10, type);
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			this.desc.BB10_ShareSelectorTitle.text = title;
			shareData = data;
			shareType = type;
			this.desc.BB10_ShareSelectorUI.SetActive(true);
		}

		private void share(int appType)
		{
			// store temp data so the GC can collect after done
			var data = shareData;
			shareData = null;

			// share
			if (shareType == SocialShareTypes.Image_PNG || shareType == SocialShareTypes.Image_JPG)
			{
				if (Common.navigator_invoke_invocation_create(ref invoke) != 0) return;
				if (appType == 0 && Common.navigator_invoke_invocation_set_target(invoke, "sys.bbm.sharehandler") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if (appType == 1 && Common.navigator_invoke_invocation_set_target(invoke, "Facebook") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if (appType == 2 && Common.navigator_invoke_invocation_set_target(invoke, "Twitter") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if (Common.navigator_invoke_invocation_set_action(invoke, "bb.action.SHARE") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				if ((appType == 1 || appType == 2) && Common.navigator_invoke_invocation_set_type(invoke, shareType == SocialShareTypes.Image_PNG ? "image/png" : "image/jpg") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}

				string filename = "data/ReignSocialImage" + (shareType == SocialShareTypes.Image_PNG ? ".png" : ".jpg");
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
				Debug.LogError("Unusported Share type: " + shareType);
			}
		}
	}
}
#endif