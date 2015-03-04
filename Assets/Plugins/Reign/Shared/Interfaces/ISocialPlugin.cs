using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Reign
{
	public enum SocialShareTypes
	{
		Image_PNG,
		Image_JPG
	}

	public class SocialDesc
	{
		public GameObject BB10_ShareSelectorUI;
		public Text BB10_ShareSelectorTitle;
		public Button BB10_CloseButton, BB10_ShareSelectorBBM, BB10_ShareSelectorFacebook, BB10_ShareSelectorTwitter;
	}
}

namespace Reign.Plugin
{
	public interface ISocialPlugin
	{
		void Init(SocialDesc desc);
		void Share(byte[] data, string title, string desc, SocialShareTypes type);
		void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type);
	}
}