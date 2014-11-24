using System;
using System.IO;

namespace Reign
{
	/// <summary>
	/// Folder Location types
	/// </summary>
	public enum FolderLocations
	{
		/// <summary>
		/// Application (Not supported on all platforms)
		/// </summary>
		Application,

		/// <summary>
		/// Storage
		/// </summary>
		Storage,

		/// <summary>
		/// Documents (Not supported on all platforms)
		/// </summary>
		Documents,

		/// <summary>
		/// Pictures
		/// </summary>
		Pictures,

		/// <summary>
		/// Music (Not supported on all platforms)
		/// </summary>
		Music,

		/// <summary>
		/// Video (Not supported on all platforms)
		/// </summary>
		Video
	}

	/// <summary>
	/// Used to check for existing files
	/// </summary>
	/// <param name="exists">True if file exists</param>
	public delegate void StreamExistsCallbackMethod(bool exists);

	/// <summary>
	/// Used to delete files
	/// </summary>
	/// <param name="succeeded">True is success</param>
	public delegate void StreamDeleteCallbackMethod(bool succeeded);

	/// <summary>
	/// Used to save files
	/// </summary>
	/// <param name="succeeded">True is success</param>
	public delegate void StreamSavedCallbackMethod(bool succeeded);

	/// <summary>
	/// Used to load/open files
	/// </summary>
	/// <param name="stream">Steam loaded</param>
	/// <param name="succeeded">True is success</param>
	public delegate void StreamLoadedCallbackMethod(Stream stream, bool succeeded);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base Stream interface object
	/// </summary>
	public interface IStreamPlugin
	{
		/// <summary>
		/// Use to check if a file exists
		/// </summary>
		/// <param name="fileName">Filename to check</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="callback">Callback fired when done</param>
		void FileExists(string fileName, FolderLocations folderLocation, StreamExistsCallbackMethod callback);

		/// <summary>
		/// Use to delete a file
		/// </summary>
		/// <param name="fileName">Filename to delete</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="callback">Callback fired when done</param>
		void DeleteFile(string fileName, FolderLocations folderLocation, StreamDeleteCallbackMethod callback);

		/// <summary>
		/// Use to save a file
		/// </summary>
		/// <param name="fileName">Filename to save</param>
		/// <param name="stream">Stream to save</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="steamSavedCallback">Callback fired when done</param>
		void SaveFile(string fileName, Stream stream, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback);

		/// <summary>
		/// Use to save a file
		/// </summary>
		/// <param name="fileName">Filename to save</param>
		/// <param name="data">Data to save</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="steamSavedCallback">Callback fired when done</param>
		void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback);

		/// <summary>
		/// Use to load a file
		/// </summary>
		/// <param name="fileName">Filename to load</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="streamLoadedCallback">Callback fired when done</param>
		void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback);

		/// <summary>
		/// Use to have the user pick where to save a file
		/// </summary>
		/// <param name="stream">Stream to save</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="fileTypes">File types use can see in popup</param>
		/// <param name="streamSavedCallback">Callback fired when done</param>
		void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback);

		/// <summary>
		/// Use to have the user pick where to save a file
		/// </summary>
		/// <param name="data">Data to save</param>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="fileTypes">File types use can see in popup</param>
		/// <param name="streamSavedCallback">Callback fired when done</param>
		void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback);

		/// <summary>
		/// Use to have the user pick a file
		/// </summary>
		/// <param name="folderLocation">Folder location</param>
		/// <param name="x">iOS popup X pos</param>
		/// <param name="y">iOS popup Y pos</param>
		/// <param name="width">iOS popup Width</param>
		/// <param name="height">iOS popup Height</param>
		/// <param name="fileTypes">File types use can see in popup</param>
		/// <param name="streamLoadedCallback">Callback fired when done</param>
		void LoadFileDialog(FolderLocations folderLocation, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback);

		/// <summary>
		/// Used to handle internal events
		/// </summary>
		void Update();
	}
}
