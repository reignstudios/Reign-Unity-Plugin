#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

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

		public override void LoadFileDialog(FolderLocations folderLocation, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (streamLoadedCallback == null) return;
			string fileName = EditorUtility.OpenFilePanel("Load file", "", "png");
			if (!string.IsNullOrEmpty(fileName)) streamLoadedCallback(new FileStream(fileName, FileMode.Open, FileAccess.Read), true);
			else streamLoadedCallback(null, false);
		}
	}
}
#endif