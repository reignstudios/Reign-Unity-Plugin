#if REIGN_POSTBUILD
using System;
using System.Collections.Generic;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
#endif

#if UNITY_WP_8_1
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
#endif

namespace Reign.Plugin
{
	public class SocialPlugin_Native : ISocialPlugin
	{
		public void Share(byte[] data, SocialShareTypes type)
		{
			StreamManager.SaveFile("SharedImage.png", data, FolderLocations.Pictures, imageSavedCallback);
		}

		private void imageSavedCallback(bool succeeded)
		{
			#if WINDOWS_PHONE
			string filename;
			using (var m = new MediaLibrary())
			using (var image = m.SavePicture("SharedImage.png", data))
			{
				filename = MediaLibraryExtensions.GetPath(image);
			}

			var shareMediaTask = new ShareMediaTask();
			shareMediaTask.FilePath = filename;
			shareMediaTask.Show();
			#elif UNITY_WP_8_1
			var dataTransferManager = DataTransferManager.GetForCurrentView();
			dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(shareStorageItemsHandler);
			#endif
		}

		#if UNITY_WP_8_1
		private async void shareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
		{
			DataRequest request = e.Request;
			request.Data.Properties.Title = "Share StorageItems Example";
			request.Data.Properties.Description = "Demonstrates how to share files.";
			DataRequestDeferral deferral = request.GetDeferral();
			try
			{
				StorageFile shareFile = await ApplicationData.Current.LocalFolder.GetFileAsync("SharedImage.png");
				var storageItems = new List<IStorageItem>();
				storageItems.Add(shareFile);
				request.Data.SetStorageItems(storageItems);       
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

		public void Share(byte[] data, SocialShareTypes type)
		{
			Native.Share(data, type);
		}
	}
}
#endif