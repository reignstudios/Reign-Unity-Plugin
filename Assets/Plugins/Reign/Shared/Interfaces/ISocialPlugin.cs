using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
	public enum SocialShareTypes
	{
		Images
	}

	public interface ISocialPlugin
	{
		void Share(byte[] data, SocialShareTypes type);
	}
}