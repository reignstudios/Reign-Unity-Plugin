using UnityEngine;
using System.Collections;

namespace Reign
{
	public enum SocialShareTypes
	{
		Image_PNG,
		Image_JPG
	}
}

namespace Reign.Plugin
{
	public interface ISocialPlugin
	{
		void Share(byte[] data, string title, string desc, SocialShareTypes type);
	}
}