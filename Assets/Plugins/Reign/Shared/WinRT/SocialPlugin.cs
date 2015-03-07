#if REIGN_POSTBUILD
using System;
using System.Collections.Generic;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
#endif

#if UNITY_METRO || UNITY_WP_8_1
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
#endif

namespace Reign.Plugin
{
	public class SocialPlugin_Native : ISocialPlugin
	{
		#if UNITY_METRO || UNITY_WP_8_1
		private string shareTitle, shareDesc;
		#endif

		public void Init(SocialDesc desc)
		{
			// do nothing...
		}

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			#if WINDOWS_PHONE
			string filename;
			using (var m = new MediaLibrary())
			using (var image = m.SavePicture("ReignSharedImage.png", data))
			{
				filename = MediaLibraryExtensions.GetPath(image);
			}

			var shareMediaTask = new ShareMediaTask();
			shareMediaTask.FilePath = filename;
			shareMediaTask.Show();
			#else
			shareTitle = title;
			shareDesc = desc;
			StreamManager.SaveFile("ReignSharedImage.png", data, FolderLocations.Storage, imageSavedCallback);
			#endif
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			Share(data, title, desc, type);
		}

		#if UNITY_METRO || UNITY_WP_8_1
		private async void imageSavedCallback(bool succeeded)
		{
			await WinRTPlugin.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate()
			{
				var dataTransferManager = DataTransferManager.GetForCurrentView();
				dataTransferManager.DataRequested -= shareStorageItemsHandler;
				dataTransferManager.DataRequested += shareStorageItemsHandler;
				DataTransferManager.ShowShareUI();
			});
		}

		private async void shareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
		{
			DataRequest request = e.Request;
			request.Data.Properties.Title = shareTitle;
			request.Data.Properties.Description = shareDesc;
			DataRequestDeferral deferral = request.GetDeferral();
			try
			{
				StorageFile shareFile = await ApplicationData.Current.LocalFolder.GetFileAsync("ReignSharedImage.png");
				var storageItems = new List<IStorageItem>();
				storageItems.Add(shareFile);
				request.Data.SetStorageItems(storageItems);       
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
			finally
			{
				deferral.Complete();
			}
		}
		#endif
	}
}
#elif UNITY_WINRT
using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
	public class SocialPlugin_WinRT : ISocialPlugin
	{
		public ISocialPlugin Native;
		public delegate void InitNativeMethod(SocialPlugin_WinRT plugin);
		public static InitNativeMethod InitNative;

		public SocialPlugin_WinRT()
		{
			InitNative(this);
		}

		public void Init(SocialDesc desc)
		{
			Native.Init(desc);
		}

		public void Share(byte[] data, string title, string desc, SocialShareTypes type)
		{
			Native.Share(data, title, desc, type);
		}

		public void Share(byte[] data, string title, string desc, int x, int y, int width, int height, SocialShareTypes type)
		{
			Native.Share(data, title, desc, x, y, width, height, type);
		}
	}
}
#endif