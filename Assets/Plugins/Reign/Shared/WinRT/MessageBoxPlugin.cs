#if REIGN_POSTBUILD
using System;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace Reign.Plugin
{
	public class MessageBoxPlugin_Native : IMessageBoxPlugin
	{
		public void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			showAsync(title, message, type, callback);
		}

		#if WINDOWS_PHONE
		private void showAsync(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		#else
		private async void showAsync(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		#endif
		{
			#if WINDOWS_PHONE
			WinRTPlugin.Dispatcher.BeginInvoke(delegate()
			{
				var result = MessageBox.Show(message, title, type == MessageBoxTypes.Ok ? MessageBoxButton.OK : MessageBoxButton.OKCancel);
				if (callback != null) callback(result == System.Windows.MessageBoxResult.OK ? MessageBoxResult.Ok : MessageBoxResult.Cancel);
			});
			#else
			await WinRTPlugin.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate()
			{
				var msg = new MessageDialog(message, title);
				if (type == MessageBoxTypes.Ok)
				{
					await msg.ShowAsync();
					if (callback != null) callback(MessageBoxResult.Ok);
				}
				else if (type == MessageBoxTypes.OkCancel)
				{
					bool result = false;
					msg.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
					msg.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler((cmd) => result = false)));
					await msg.ShowAsync();
					if (callback != null) callback(result ? MessageBoxResult.Ok : MessageBoxResult.Cancel);
				}
			});
			#endif
		}

		public void Update()
		{
			// do nothing...
		}
	}
}
#elif UNITY_WINRT
namespace Reign.Plugin
{
	public class MessageBoxPlugin_WinRT : IMessageBoxPlugin
	{
		public IMessageBoxPlugin Native;

		public delegate void InitNativeMethod(MessageBoxPlugin_WinRT plugin);
		public static InitNativeMethod InitNative;

		public MessageBoxPlugin_WinRT()
		{
			InitNative(this);
		}

		public void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			Native.Show(title, message, type, callback);
		}

		public void Update()
		{
			Native.Update();
		}
	}
}
#endif