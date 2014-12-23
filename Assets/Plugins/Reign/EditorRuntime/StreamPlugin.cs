#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using ImageTools.IO.Png;
using ImageTools.Filtering;
using ImageTools;
using System;

namespace Reign.Plugin
{
	public class StreamPlugin : StreamPluginBase
	{
		public override void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			if (!string.IsNullOrEmpty(fileName))
			{
				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read(data, 0, data.Length);
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}

				if (streamSavedCallback != null) streamSavedCallback(true);
			}
			else
			{
				if (streamSavedCallback != null) streamSavedCallback(false);
			}
		}

		public override void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			if (!string.IsNullOrEmpty(fileName))
			{
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}

				if (streamSavedCallback != null) streamSavedCallback(true);
			}
			else
			{
				if (streamSavedCallback != null) streamSavedCallback(false);
			}
		}

		public override void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (streamLoadedCallback == null) return;
			string fileName = EditorUtility.OpenFilePanel("Load file", "", "png");
			if (!string.IsNullOrEmpty(fileName))
			{
				if (maxWidth == 0 || maxHeight == 0)
				{
					streamLoadedCallback(new FileStream(fileName, FileMode.Open, FileAccess.Read), true);
				}
				else
				{
					var newStream = new MemoryStream();
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
						{
							var decoder = new PngDecoder();
							var image = new ExtendedImage();
							decoder.Decode(image, stream);
							var newImage = ExtendedImage.Resize(image, 32, 32, new NearestNeighborResizer());

							var encoder = new PngEncoder();
							encoder.Encode(newImage, newStream);
						}
					}
					catch (Exception e)
					{
						newStream.Dispose();
						newStream = null;
						Debug.LogError(e.Message);
					}
					finally
					{
						streamLoadedCallback(newStream, true);
					}
				}
			}
			else
			{
				streamLoadedCallback(null, false);
			}
		}
	}
}
#endif