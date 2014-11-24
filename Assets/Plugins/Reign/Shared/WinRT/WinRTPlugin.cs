#if REIGN_POSTBUILD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Reign.Plugin;
using System.Threading;

#if UNITY_METRO
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
#endif

#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Threading;
#endif

namespace Reign
{
	public static class WinRTPlugin
	{
		public static Grid AdGrid {get; private set;}

		#if UNITY_METRO
		public static CoreWindow CoreWindow {get; private set;}
		private static string privacyPolicyURL;

		public static void Init(Grid adGrid, string privacyPolicyURL, bool usePrivacyPolicy)
		{
			CoreWindow = CoreWindow.GetForCurrentThread();
			WinRTPlugin.AdGrid = adGrid;

			if (usePrivacyPolicy)
			{
				WinRTPlugin.privacyPolicyURL = privacyPolicyURL;
				SettingsPane.GetForCurrentView().CommandsRequested += showPrivacyPolicy;
			}

			#if !REIGN_TEST
			initMethodPointers();
			#endif
		}

		private static void showPrivacyPolicy(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
			var privacyPolicyCommand = new SettingsCommand("privacyPolicy","Privacy Policy", (uiCommand) => {launchPrivacyPolicyUrl();});
			args.Request.ApplicationCommands.Add(privacyPolicyCommand);
		}

		private static async void launchPrivacyPolicyUrl()
		{
			Uri privacyPolicyUrl = new Uri(privacyPolicyURL);
			var result = await Windows.System.Launcher.LaunchUriAsync(privacyPolicyUrl);
		}
		#endif

		#if WINDOWS_PHONE
		public static Dispatcher Dispatcher;

		/// <summary>
		/// Used to reference objects required by the Reign plugin.
		/// </summary>
		/// <param name="adGrid">Pass in the "DrawingSurfaceBackground" object</param>
		/// <param name="dispatcher">Pass in the "Dispatcher" object</param>
		public static void Init(Grid adGrid, Dispatcher dispatcher)
		{
			WinRTPlugin.Dispatcher = dispatcher;
			WinRTPlugin.AdGrid = adGrid;

			#if !REIGN_TEST
			initMethodPointers();
			#endif
		}
		#endif

		#if !REIGN_TEST
		private static void initMethodPointers()
		{
			StreamPlugin_WinRT.InitNative = init_StreamPlugin;
			MicrosoftStore_InAppPurchasePlugin_WinRT.InitNative = init_MicrosoftStore_InAppPurchasePlugin;
			MicrosoftAdvertising_AdPlugin_WinRT.InitNative = init_MicrosoftAdvertising_AdPlugin;
			MessageBoxPlugin_WinRT.InitNative = init_MessageBoxPlugin;
			MarketingPlugin_WinRT.InitNative = init_MarketingPlugin;
			EmailPlugin_WinRT.InitNative = init_EmailPlugin;

			#if WINDOWS_PHONE
			AdMob_AdPlugin_WP8.InitNative = init_AdMob_AdPlugin;
			AdMob_InterstitialAdPlugin_WP8.InitNative = init_AdMob_InterstitialAdPlugin;
			#endif
		}

		private static void init_StreamPlugin(StreamPlugin_WinRT plugin)
		{
			plugin.Native = new StreamPlugin_Native();
		}

		private static void init_MicrosoftStore_InAppPurchasePlugin(MicrosoftStore_InAppPurchasePlugin_WinRT plugin, InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			plugin.Native = new MicrosoftStore_InAppPurchasePlugin_Native(desc, createdCallback);
		}

		private static void init_MicrosoftAdvertising_AdPlugin(MicrosoftAdvertising_AdPlugin_WinRT plugin, AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			plugin.Native = new MicrosoftAdvertising_AdPlugin_Native(desc, createdCallback);
		}

		private static void init_MessageBoxPlugin(MessageBoxPlugin_WinRT plugin)
		{
			plugin.Native = new MessageBoxPlugin_Native();
		}

		private static void init_MarketingPlugin(MarketingPlugin_WinRT plugin)
		{
			plugin.Native = new MarketingPlugin_Native();
		}

		private static void init_EmailPlugin(EmailPlugin_WinRT plugin)
		{
			plugin.Native = new EmailPlugin_Native();
		}

		#if WINDOWS_PHONE
		private static void init_AdMob_AdPlugin(AdMob_AdPlugin_WP8 plugin, AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			plugin.Native = new AdMob_AdPlugin_Native(desc, createdCallback);
		}

		private static void init_AdMob_InterstitialAdPlugin(AdMob_InterstitialAdPlugin_WP8 plugin, InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod createdCallback)
		{
			plugin.Native = new AdMob_InterstitialAdPlugin_Native(desc, createdCallback);
		}
		#endif
		#endif
	}

	static class Thread
	{
		public static void Sleep(int milli)
		{
			using (var _event = new ManualResetEvent(false))
			{
				_event.WaitOne(milli);
			}
		}
	}
}
#endif