#if REIGN_POSTBUILD
using System;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif

namespace Reign.Plugin
{
    public class MarketingPlugin_Native : IIMarketingPlugin
    {
		#if UNITY_METRO
		private async void async_OpenStore(string url)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + url));
		}
		#endif

		public void OpenStore(MarketingDesc desc)
		{
			#if WINDOWS_PHONE
			var task = new MarketplaceDetailTask();
			task.Show();
			#else
			async_OpenStore(desc.WinRT_PackageFamilyName);
			#endif
		}

		#if UNITY_METRO
		private async void async_OpenStoreForReview(string url)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + url));
		}
		#endif

		public void OpenStoreForReview(MarketingDesc desc)
		{
			#if WINDOWS_PHONE
			var task = new MarketplaceReviewTask();
			task.Show();
			#else
			async_OpenStoreForReview(desc.WinRT_PackageFamilyName);
			#endif
		}
    }
}
#elif UNITY_WINRT
namespace Reign.Plugin
{
    public class MarketingPlugin_WinRT : IIMarketingPlugin
    {
		public IIMarketingPlugin Native;

		public delegate void InitNativeMethod(MarketingPlugin_WinRT plugin);
		public static InitNativeMethod InitNative;

		public MarketingPlugin_WinRT()
		{
			InitNative(this);
		}

		public void OpenStore(MarketingDesc desc)
		{
			Native.OpenStore(desc);
		}

		public void OpenStoreForReview(MarketingDesc desc)
		{
			Native.OpenStoreForReview(desc);
		}
    }
}
#endif