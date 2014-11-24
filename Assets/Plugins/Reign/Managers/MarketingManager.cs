// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using Reign.Plugin;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Reign
{
	/// <summary>
	/// Used to manage marketing features.
	/// </summary>
	public static class MarketingManager
	{
		private static IIMarketingPlugin plugin;

		static MarketingManager()
		{
			ReignServices.CheckStatus();
			
			#if !DISABLE_REIGN
			#if UNITY_WINRT && !UNITY_EDITOR
			plugin = new MarketingPlugin_WinRT();
			#else
			plugin = new MarketingPlugin();
			#endif
			#endif
		}

		/// <summary>
		/// Show your app in the store.
		/// </summary>
		/// <param name="desc">Marketing Desc.</param>
		public static void OpenStore(MarketingDesc desc)
		{
			plugin.OpenStore(desc);
		}

		/// <summary>
		/// Show your app in the store for the user to review.
		/// </summary>
		/// <param name="desc">Marketing Desc.</param>
		public static void OpenStoreForReview(MarketingDesc desc)
		{
			plugin.OpenStoreForReview(desc);
		}
	}
}