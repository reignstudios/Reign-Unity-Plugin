// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

//#define TEST_ASYNC
#if (UNITY_WINRT && !UNITY_EDITOR) || TEST_ASYNC
#define ASYNC
#endif

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Reign.Plugin;

namespace Reign
{
	enum StreamManagerQueTypes
	{
		FileExists,
		DeleteFile,
		SaveFile,
		SaveFileDialog,
		LoadFile,
		LoadFileDialog
	}

	class StreamManagerQue
	{
		public StreamManagerQueTypes Type;
		public StreamExistsCallbackMethod streamExistsCallback;
		public StreamDeleteCallbackMethod streamDeleteCallback;
		public StreamSavedCallbackMethod streamSavedCallback;
		public StreamLoadedCallbackMethod streamLoadedCallback;

		public string FileName;
		public FolderLocations FolderLocation;
		public Stream Stream;
		public byte[] Data;
		public string[] FileTypes;

		public StreamManagerQue(StreamManagerQueTypes type)
		{
			Type = type;
		}
	}

	/// <summary>
	/// Used to manage IO streams.
	/// </summary>
    public static class StreamManager
    {
		private static IStreamPlugin plugin;
		private static bool savingStream, loadingStream, checkingIfFileExists, deletingFile;
		private static List<StreamManagerQue> ques;
		private static StreamExistsCallbackMethod streamExistsCallback;
		private static StreamDeleteCallbackMethod streamDeleteCallback;
		private static StreamSavedCallbackMethod streamSavedCallback;
		private static StreamLoadedCallbackMethod streamLoadedCallback;

		#if ASYNC
		private static bool asyncExistsDone, asyncDeleteDone, asyncExistsSucceeded, asyncDeleteSucceeded;
		private static bool asyncSaveDone, asyncLoadDone, asyncSaveSucceeded, asyncLoadSucceeded;
		private static Stream asyncLoadStream;
		#endif

		static StreamManager()
		{
			ReignServices.CheckStatus();
			ques = new List<StreamManagerQue>();

			#if !DISABLE_REIGN
			#if UNITY_WINRT && !UNITY_EDITOR
			plugin = new StreamPlugin_WinRT();
			#else
			plugin = new StreamPlugin();
			#endif

			ReignServices.AddService(update, null, null);
			#endif
		}

		#if ASYNC
		private static void update()
		{
			plugin.Update();

			if (asyncExistsDone)
			{
				checkingIfFileExists = false;
				asyncExistsDone = false;
				if (streamExistsCallback != null) streamExistsCallback(asyncExistsSucceeded);
			}

			if (asyncDeleteDone)
			{
				deletingFile = false;
				asyncDeleteDone = false;
				if (streamDeleteCallback != null) streamDeleteCallback(asyncDeleteSucceeded);
			}
		
			if (asyncSaveDone)
			{
				savingStream = false;
				asyncSaveDone = false;
				if (streamSavedCallback != null) streamSavedCallback(asyncSaveSucceeded);
			}

			if (asyncLoadDone)
			{
				loadingStream = false;
				asyncLoadDone = false;
				var stream = asyncLoadStream;
				asyncLoadStream = null;
				if (streamLoadedCallback != null) streamLoadedCallback(stream, asyncLoadSucceeded);
			}

			updateQues();
		}

		private static void async_streamExistsCallback(bool exists)
		{
			asyncExistsSucceeded = exists;
			asyncExistsDone = true;
		}

		private static void async_streamDeleteCallback(bool succeeded)
		{
			asyncDeleteSucceeded = succeeded;
			asyncDeleteDone = true;
		}

		private static void async_streamSavedCallback(bool succeeded)
		{
			asyncSaveSucceeded = succeeded;
			asyncSaveDone = true;
		}

		private static void async_streamLoadedCallback(Stream stream, bool succeeded)
		{
			asyncLoadStream = stream;
			asyncLoadSucceeded = succeeded;
			asyncLoadDone = true;
		}
		#else
		private static void update()
		{
			plugin.Update();
			updateQues();
		}

		private static void noAsync_streamExistsCallback(bool exists)
		{
			checkingIfFileExists = false;
			if (streamExistsCallback != null) streamExistsCallback(exists);
		}

		private static void noAsync_streamDeleteCallback(bool succeeded)
		{
			deletingFile = false;
			if (streamDeleteCallback != null) streamDeleteCallback(succeeded);
		}
		
		private static void noAsync_streamSavedCallback(bool succeeded)
		{
			savingStream = false;
			if (streamSavedCallback != null) streamSavedCallback(succeeded);
		}
		
		private static void noAsync_streamLoadedCallback(Stream stream, bool succeeded)
		{
			loadingStream = false;
			if (streamLoadedCallback != null) streamLoadedCallback(stream, succeeded);
		}
		#endif

		private static void updateQues()
		{
			if (ques.Count == 0) return;

			foreach (var que in ques.ToArray())
			{
				switch (que.Type)
				{
					case StreamManagerQueTypes.FileExists:
						if (!checkingIfFileExists)
						{
							FileExists(que.FileName, que.FolderLocation, que.streamExistsCallback);
							ques.Remove(que);
						}
						break;

					case StreamManagerQueTypes.DeleteFile:
						if (!deletingFile)
						{
							DeleteFile(que.FileName, que.FolderLocation, que.streamDeleteCallback);
							ques.Remove(que);
						}
						break;

					case StreamManagerQueTypes.SaveFile:
						if (!savingStream)
						{
							if(que.Data != null) SaveFile(que.FileName, que.Data, que.FolderLocation, que.streamSavedCallback);
							else SaveFile(que.FileName, que.Stream, que.FolderLocation, que.streamSavedCallback);
							ques.Remove(que);
						}
						break;

					case StreamManagerQueTypes.SaveFileDialog:
						if (!savingStream)
						{
							if(que.Data != null) SaveFileDialog(que.Data, que.FolderLocation, que.FileTypes, que.streamSavedCallback);
							else SaveFileDialog(que.Stream, que.FolderLocation, que.FileTypes, que.streamSavedCallback);
							ques.Remove(que);
						}
						break;

					case StreamManagerQueTypes.LoadFile:
						if (!loadingStream)
						{
							LoadFile(que.FileName, que.FolderLocation, que.streamLoadedCallback);
							ques.Remove(que);
						}
						break;

					case StreamManagerQueTypes.LoadFileDialog:
						if (!loadingStream)
						{
							LoadFileDialog(que.FolderLocation, que.FileTypes, que.streamLoadedCallback);
							ques.Remove(que);
						}
						break;
				}
			}
		}
		
		private static string getCorrectUnityPath(string fileName, FolderLocations folderLocation)
		{
			if (folderLocation == FolderLocations.Storage) return ConvertToPlatformSlash(Application.persistentDataPath + "/" + fileName);
			else return ConvertToPlatformSlash(fileName);
		}

		/// <summary>
		/// Use to save a ScreenShot of you game to the pictures folder.
		/// </summary>
		/// <param name="streamSavedCallback"></param>
		public static void SaveScreenShotToPictures(StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				Debug.LogError("You must wait for the last saved file to finish!");
				if (streamSavedCallback != null) streamSavedCallback(false);
				return;
			}

			StreamManager.streamSavedCallback = streamSavedCallback;
			ReignServices.Singleton.frameDoneCallback = saveScreenShotFrameDone;
			ReignServices.Singleton.requestFrameDone = true;
		}

		private static void saveScreenShotFrameDone()
		{
			var width = Screen.width;
			var height = Screen.height;
			var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
			texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			texture.Apply();
			var data = texture.EncodeToPNG();
			GameObject.Destroy(texture);
			SaveFile("ScreenShot.png", data, FolderLocations.Pictures, streamSavedCallback);
		}

		/// <summary>
		/// Use to check if a file exists.
		/// </summary>
		/// <param name="fileName">FileName to check.</param>
		/// <param name="folderLocation">Folder location to load file from.</param>
		/// <param name="streamExistsCallback">The callback that fires when done.</param>
		public static void FileExists(string fileName, FolderLocations folderLocation, StreamExistsCallbackMethod streamExistsCallback)
		{
			if (checkingIfFileExists)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.FileExists);
				que.streamExistsCallback = streamExistsCallback;
				que.FileName = fileName;
				que.FolderLocation = folderLocation;
				ques.Add(que);
				return;
			}

			checkingIfFileExists = true;
			StreamManager.streamExistsCallback = streamExistsCallback;
		
			#if ASYNC
			asyncExistsDone = false;
			plugin.FileExists(fileName, folderLocation, async_streamExistsCallback);
			#else
			plugin.FileExists(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamExistsCallback);
			#endif
		}

		/// <summary>
		/// Use to delete a file.
		/// </summary>
		/// <param name="fileName">FileName to delete.</param>
		/// <param name="folderLocation">Folder location to delete file from.</param>
		/// <param name="streamDeleteCallback">The callback that fires when done.</param>
		public static void DeleteFile(string fileName, FolderLocations folderLocation, StreamDeleteCallbackMethod streamDeleteCallback)
		{
			if (checkingIfFileExists)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.DeleteFile);
				que.streamDeleteCallback = streamDeleteCallback;
				que.FileName = fileName;
				que.FolderLocation = folderLocation;
				ques.Add(que);
				return;
			}

			deletingFile = true;
			StreamManager.streamDeleteCallback = streamDeleteCallback;
		
			#if ASYNC
			asyncDeleteDone = false;
			plugin.DeleteFile(fileName, folderLocation, async_streamDeleteCallback);
			#else
			plugin.DeleteFile(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamDeleteCallback);
			#endif
		}

		/// <summary>
		/// Use to save a file.
		/// </summary>
		/// <param name="fileName">FileName to save.</param>
		/// <param name="stream">Stream to save.</param>
		/// <param name="folderLocation">Folder location to save file.</param>
		/// <param name="streamSavedCallback">The callback that fires when done.</param>
		public static void SaveFile(string fileName, Stream stream, FolderLocations folderLocation, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.SaveFile);
				que.streamSavedCallback = streamSavedCallback;
				que.FileName = fileName;
				que.Stream = stream;
				que.FolderLocation = folderLocation;
				ques.Add(que);
				return;
			}

			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFile(fileName, stream, folderLocation, async_streamSavedCallback);
			#else
			plugin.SaveFile(getCorrectUnityPath(fileName, folderLocation), stream, folderLocation, noAsync_streamSavedCallback);
			#endif
		}

		/// <summary>
		/// Use to save a file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="data">Data to save.</param>
		/// <param name="folderLocation">Folder location to save file.</param>
		/// <param name="streamSavedCallback">The callback that fires when done.</param>
		public static void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.SaveFile);
				que.streamSavedCallback = streamSavedCallback;
				que.FileName = fileName;
				que.Data = data;
				que.FolderLocation = folderLocation;
				ques.Add(que);
				return;
			}

			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFile(fileName, data, folderLocation, async_streamSavedCallback);
			#else
			plugin.SaveFile(getCorrectUnityPath(fileName, folderLocation), data, folderLocation, noAsync_streamSavedCallback);
			#endif
		}

		/// <summary>
		/// Use to load a file.
		/// </summary>
		/// <param name="fileName">FileName to load.</param>
		/// <param name="folderLocation">Folder location to load from.</param>
		/// <param name="streamLoadedCallback">The callback that fires when done.</param>
		public static void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (loadingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.LoadFile);
				que.streamLoadedCallback = streamLoadedCallback;
				que.FileName = fileName;
				que.FolderLocation = folderLocation;
				ques.Add(que);
				return;
			}

			loadingStream = true;
			StreamManager.streamLoadedCallback = streamLoadedCallback;
		
			#if ASYNC
			asyncLoadDone = false;
			plugin.LoadFile(fileName, folderLocation, async_streamLoadedCallback);
			#else
			plugin.LoadFile(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamLoadedCallback);
			#endif
		}

		/// <summary>
		/// Use to have the user pic where a file should be saved.
		/// NOTE: Does not work on all platforms.
		/// </summary>
		/// <param name="stream">Stream to save.</param>
		/// <param name="folderLocation">Folder location the user should save in.</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamSavedCallback">The callback that fires when done.</param>
		public static void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.SaveFileDialog);
				que.streamSavedCallback = streamSavedCallback;
				que.Stream = stream;
				que.FolderLocation = folderLocation;
				que.FileTypes = fileTypes;
				ques.Add(que);
				return;
			}

			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFileDialog(stream, folderLocation, fileTypes, async_streamSavedCallback);
			#else
			plugin.SaveFileDialog(stream, folderLocation, fileTypes, noAsync_streamSavedCallback);
			#endif
		}

		/// <summary>
		/// Use to have the user pic where a file should be saved.
		/// NOTE: Does not work on all platforms.
		/// </summary>
		/// <param name="data">Data to save.</param>
		/// <param name="folderLocation">Folder location the user should save in.</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamSavedCallback">The callback that fires when done.</param>
		public static void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.SaveFileDialog);
				que.streamSavedCallback = streamSavedCallback;
				que.Data = data;
				que.FolderLocation = folderLocation;
				que.FileTypes = fileTypes;
				ques.Add(que);
				return;
			}

			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFileDialog(data, folderLocation, fileTypes, async_streamSavedCallback);
			#else
			plugin.SaveFileDialog(data, folderLocation, fileTypes, noAsync_streamSavedCallback);
			#endif
		}

		/// <summary>
		/// Use to have to user pic a file. (Remember to dispose loaded stream)
		/// </summary>
		/// <param name="folderLocation">Folder location from where the user should choose from.</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamLoadedCallback">The callback that fires when done.</param>
		public static void LoadFileDialog(FolderLocations folderLocation, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			LoadFileDialog(folderLocation, 0, 0, 0, 0, 10, 10, fileTypes, streamLoadedCallback);
		}

		/// <summary>
		/// Use to have to user pic a file. (Remember to dispose loaded stream)
		/// </summary>
		/// <param name="folderLocation">Folder location from where the user should choose from.</param>
		/// <param name="maxWidth">Image size returned will not be above the Max Width value (set 0 to disable)</param>
		/// <param name="maxHeight">Image size returned will not be above the Max Height value (set 0 to disable)</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamLoadedCallback">The callback that fires when done.</param>
		public static void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			LoadFileDialog(folderLocation, maxWidth, maxHeight, 0, 0, 10, 10, fileTypes, streamLoadedCallback);
		}
		
		/// <summary>
		/// Use to have to user pic a file on iOS. (Remember to dispose loaded stream)
		/// NOTE: The x, y, width, height is ONLY for iOS (other platforms ignore these values).
		/// </summary>
		/// <param name="folderLocation">Folder location from where the user should choose from.</param>
		/// <param name="x">iOS iPad dlg X.</param>
		/// <param name="y">iOS iPad dlg Y.</param>
		/// <param name="width">iOS iPad dlg Width.</param>
		/// <param name="height">iOS iPad dlg Height.</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamLoadedCallback">The callback that fires when done.</param>
		public static void LoadFileDialog(FolderLocations folderLocation, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			LoadFileDialog(folderLocation, 0, 0, 0, 0, 10, 10, fileTypes, streamLoadedCallback);
		}

		/// <summary>
		/// Use to have to user pic a file on iOS. (Remember to dispose loaded stream)
		/// NOTE: The x, y, width, height is ONLY for iOS (other platforms ignore these values).
		/// </summary>
		/// <param name="folderLocation">Folder location from where the user should choose from.</param>
		/// <param name="maxWidth">Image size returned will not be above the Max Width value (set 0 to disable)</param>
		/// <param name="maxHeight">Image size returned will not be above the Max Height value (set 0 to disable)</param>
		/// <param name="x">iOS iPad dlg X.</param>
		/// <param name="y">iOS iPad dlg Y.</param>
		/// <param name="width">iOS iPad dlg Width.</param>
		/// <param name="height">iOS iPad dlg Height.</param>
		/// <param name="fileTypes">File types the user can see in file popup.</param>
		/// <param name="streamLoadedCallback">The callback that fires when done.</param>
		public static void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (loadingStream)
			{
				var que = new StreamManagerQue(StreamManagerQueTypes.LoadFileDialog);
				que.streamLoadedCallback = streamLoadedCallback;
				que.FolderLocation = folderLocation;
				que.FileTypes = fileTypes;
				ques.Add(que);
				return;
			}

			loadingStream = true;
			StreamManager.streamLoadedCallback = streamLoadedCallback;
		
			#if ASYNC
			asyncLoadDone = false;
			plugin.LoadFileDialog(folderLocation, maxWidth, maxHeight, x, y, width, height, fileTypes, async_streamLoadedCallback);
			#else
			plugin.LoadFileDialog(folderLocation, maxWidth, maxHeight, x, y, width, height, fileTypes, noAsync_streamLoadedCallback);
			#endif
		}

		/// <summary>
		/// Copy any stream to a Memory stream.
		/// NOTE: Useful for streams that cant have there position values set.
		/// </summary>
		/// <param name="stream">Src stream.</param>
		/// <returns>Dst Stream.</returns>
		public static MemoryStream CopyToMemoryStream(Stream stream)
		{
			var memoryStream = new MemoryStream();
			var buffer = new byte[1024];
			while (true)
			{
				int readLength = stream.Read(buffer, 0, buffer.Length);
				memoryStream.Write(buffer, 0, readLength);
				if (readLength != buffer.Length) break;
			}
		
			memoryStream.Position = 0;
			return memoryStream;
		}
		
		/// <summary>
		/// Convert path slash char values to your current platform.
		/// </summary>
		/// <param name="path">Src path.</param>
		/// <returns>Dst path.</returns>
		public static string ConvertToPlatformSlash(string path)
		{
			#if UNITY_WINRT || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			return path.Replace('/', '\\');
			#else
			return path.Replace('\\', '/');
			#endif
		}
		
		/// <summary>
		/// Use to get file the file directory without the filename.
		/// </summary>
		/// <param name="fileName">Src path.</param>
		/// <returns>Dst path.</returns>
		public static string GetFileDirectory(string fileName)
		{
			bool pass = false;
			foreach (var c in fileName)
			{
				if (c == '/' || c == '\\')
				{
					pass = true;
					break;
				}
			}
			if (!pass) return "";

			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			#if UNITY_WINRT
			return fileName + '\\';
			#else
			return fileName + '/';
			#endif
		}
		
		/// <summary>
		/// Use to get the the filename with its ext.
		/// </summary>
		/// <param name="fileName">Src path.</param>
		/// <returns>Dst path.</returns>
		public static string GetFileNameWithExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = fileName.Substring(match.Value.Length, fileName.Length - match.Value.Length);
			}

			return fileName;
		}
		
		/// <summary>
		/// Use to get filename without its ext.
		/// </summary>
		/// <param name="fileName">Src path.</param>
		/// <returns>Dst path.</returns>
		public static string GetFileNameWithoutExt(string fileName)
		{
			fileName = GetFileNameWithExt(fileName);
			string ext = GetFileExt(fileName);
			return fileName.Substring(0, fileName.Length - ext.Length);
		}

		/// <summary>
		/// Use to get the file ext.
		/// </summary>
		/// <param name="fileName">Src path.</param>
		/// <returns>Out ext type.</returns>
		public static string GetFileExt(string fileName)
		{
			var names = fileName.Split('.');
			if (names.Length < 2) return null;
			return '.' + names[names.Length-1];
		}

		/// <summary>
		/// Use to trim off the file ext.
		/// </summary>
		/// <param name="fileName">Src path.</param>
		/// <returns>Dst path.</returns>
		public static string TrimFileExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*\.");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			return fileName;
		}

		/// <summary>
		/// Use to check if the file is a full path.
		/// </summary>
		/// <param name="fileName">Path.</param>
		/// <returns>Returns true or false.</returns>
		public static bool IsAbsolutePath(string fileName)
		{
			#if UNITY_STANDALONE_WIN
			var match = Regex.Match(fileName, @"A|C|D|E|F|G|H|I:/|\\");
			return match.Success;
			#else
			throw new NotImplementedException();
			#endif
		}

		/// <summary>
		/// Use to convert 4 bytes into an int.
		/// </summary>
		/// <param name="ch0">Byte 0.</param>
		/// <param name="ch1">Byte 1.</param>
		/// <param name="ch2">Byte 2.</param>
		/// <param name="ch3">Byte 3.</param>
		/// <returns>Returns int from 4 bytes.</returns>
		public static int MakeFourCC(char ch0, char ch1, char ch2, char ch3)
		{
			return (((int)(byte)(ch0)) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
		}
    }

	/// <summary>
	/// Used to help write vectors to streams.
	/// </summary>
	public static class StreamExtensions
	{
		#region Vectors
		/// <summary>
		/// Write a Vector to stream.
		/// </summary>
		/// <param name="writer">Dst writer.</param>
		/// <param name="value">Value to write.</param>
		public static void WriteVector(this BinaryWriter writer, Vector2 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
		}

		/// <summary>
		/// Write a Vector to stream.
		/// </summary>
		/// <param name="writer">Dst writer.</param>
		/// <param name="value">Value to write.</param>
		public static void WriteVector(this BinaryWriter writer, Vector3 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
			writer.Write(value.z);
		}

		/// <summary>
		/// Write a Vector to stream.
		/// </summary>
		/// <param name="writer">Dst writer.</param>
		/// <param name="value">Value to write.</param>
		public static void WriteVector(this BinaryWriter writer, Vector4 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
			writer.Write(value.z);
			writer.Write(value.w);
		}

		/// <summary>
		/// Read a Vector from a stream.
		/// </summary>
		/// <param name="reader">Src reader.</param>
		/// <returns>Returns value.</returns>
		public static Vector2 ReadVector2(this BinaryReader reader)
		{
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>
		/// Read a Vector from a stream.
		/// </summary>
		/// <param name="reader">Src reader.</param>
		/// <returns>Returns value.</returns>
		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>
		/// Read a Vector from a stream.
		/// </summary>
		/// <param name="reader">Src reader.</param>
		/// <returns>Returns value.</returns>
		public static Vector4 ReadVector4(this BinaryReader reader)
		{
			return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
		#endregion

		#region Matrices
		/// <summary>
		/// Write a Vector to stream.
		/// </summary>
		/// <param name="writer">Dst writer.</param>
		/// <param name="value">Value to write.</param>
		public static void WriteMatrix(this BinaryWriter writer, Matrix4x4 value)
		{
			writer.WriteVector(new Vector4(value.m00, value.m01, value.m02, value.m03));
			writer.WriteVector(new Vector4(value.m10, value.m11, value.m12, value.m13));
			writer.WriteVector(new Vector4(value.m20, value.m21, value.m22, value.m23));
			writer.WriteVector(new Vector4(value.m30, value.m31, value.m32, value.m33));
		}

		/// <summary>
		/// Read a Vector from a stream.
		/// </summary>
		/// <param name="reader">Src reader.</param>
		/// <returns>Returns value.</returns>
		public static Matrix4x4 ReadMatrix4(this BinaryReader reader)
		{
			var x = reader.ReadVector4();
			var y = reader.ReadVector4();
			var z = reader.ReadVector4();
			var w = reader.ReadVector4();
			return new Matrix4x4()
			{
				m00 = x.x, m01 = x.y, m02 = x.z, m03 = x.w,
				m10 = y.x, m11 = y.y, m12 = y.z, m13 = y.w,
				m20 = z.x, m21 = z.y, m22 = z.z, m23 = z.w,
				m30 = w.x, m31 = w.y, m32 = w.z, m33 = w.w,
			};
		}
		#endregion
	}
}
