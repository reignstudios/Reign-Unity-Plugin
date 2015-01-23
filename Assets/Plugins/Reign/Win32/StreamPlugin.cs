#if UNITY_STANDALONE_WIN
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using ImageTools.IO.Png;
using ImageTools.Filtering;
using ImageTools;
using System;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	struct OPENFILENAME
	{
		public uint         lStructSize;
		public IntPtr          hwndOwner;
		public IntPtr     hInstance;
		public IntPtr       lpstrFilter;
		public IntPtr        lpstrCustomFilter;
		public uint         nMaxCustFilter;
		public uint         nFilterIndex;
		public IntPtr        lpstrFile;
		public uint         nMaxFile;
		public IntPtr        lpstrFileTitle;
		public uint         nMaxFileTitle;
		public IntPtr       lpstrInitialDir;
		public IntPtr       lpstrTitle;
		public uint         Flags;
		public Int16          nFileOffset;
		public Int16          nFileExtension;
		public IntPtr       lpstrDefExt;
		public IntPtr        lCustData;
		public IntPtr lpfnHook;
		public IntPtr       lpTemplateName;
		public IntPtr          pvReserved;
		public uint         dwReserved;
		public uint         FlagsEx;
	};

	public class StreamPlugin_Win32 : StreamPluginBase
	{
		[DllImport("Comdlg32.dll", EntryPoint="GetOpenFileName")]
		private extern static void GetOpenFileName(ref OPENFILENAME lpofn);

		private const uint MAX_PATH = 0x00000104;
		private const uint OFN_PATHMUSTEXIST = 0x00000800, OFN_FILEMUSTEXIST = 0x00001000, OFN_NOCHANGEDIR = 0x00000008;

		public override void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			//string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			//if (!string.IsNullOrEmpty(fileName))
			//{
			//	var data = new byte[stream.Length];
			//	stream.Position = 0;
			//	stream.Read(data, 0, data.Length);
			//	using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			//	{
			//		file.Write(data, 0, data.Length);
			//	}

			//	if (streamSavedCallback != null) streamSavedCallback(true);
			//}
			//else
			//{
			//	if (streamSavedCallback != null) streamSavedCallback(false);
			//}
		}

		public override void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			//string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			//if (!string.IsNullOrEmpty(fileName))
			//{
			//	using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			//	{
			//		file.Write(data, 0, data.Length);
			//	}

			//	if (streamSavedCallback != null) streamSavedCallback(true);
			//}
			//else
			//{
			//	if (streamSavedCallback != null) streamSavedCallback(false);
			//}
		}

		public override void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (streamLoadedCallback == null) return;
			//string fileName = EditorUtility.OpenFilePanel("Load file", "", "png");

			// open native dlg
			var file = new OPENFILENAME();
			file.lStructSize = (uint)Marshal.SizeOf(typeof(OPENFILENAME));
			file.hwndOwner = IntPtr.Zero;//this->Owner;
			file.lpstrDefExt = Marshal.StringToHGlobalAnsi("png");//this->DefaultExtension;
			file.lpstrFile = Marshal.AllocHGlobal((int)MAX_PATH);
			unsafe {((byte*)file.lpstrFile.ToPointer())[0] = 0;}
			file.nMaxFile = MAX_PATH;
			file.lpstrFilter = Marshal.StringToHGlobalAnsi("Images (*.jpg;*.png;*.bmp;*.tga;*.psd)*.jpg;*.png;*.bmp;*.tga;*.psd\0");//this->Filter;
			file.nFilterIndex = 0;//this->FilterIndex;
			file.lpstrInitialDir = Marshal.StringToHGlobalAnsi(Application.dataPath);
			file.lpstrTitle = Marshal.StringToHGlobalAnsi("Load file");
			file.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_NOCHANGEDIR;
			GetOpenFileName(ref file);

			// get native dlg result
			string filename = null;
			if (file.lpstrFile != IntPtr.Zero)
			{
				filename = Marshal.PtrToStringAnsi(file.lpstrFile);
				Debug.Log("Loading file: " + filename);
			}

			Marshal.FreeHGlobal(file.lpstrFile);
			Marshal.FreeHGlobal(file.lpstrDefExt);
			Marshal.FreeHGlobal(file.lpstrInitialDir);
			Marshal.FreeHGlobal(file.lpstrTitle);
			Marshal.FreeHGlobal(file.lpstrFilter);

			// open file
			if (!string.IsNullOrEmpty(filename))
			{
				if (maxWidth == 0 || maxHeight == 0)
				{
					streamLoadedCallback(new FileStream(filename, FileMode.Open, FileAccess.Read), true);
				}
				else
				{
					var newStream = new MemoryStream();
					try
					{
						using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
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