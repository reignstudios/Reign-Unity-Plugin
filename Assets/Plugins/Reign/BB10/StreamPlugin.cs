#if UNITY_BLACKBERRY
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class StreamPlugin_BB10 : StreamPluginBase
	{
		private IntPtr invoke;
		private const int NAVIGATOR_INVOKE_TARGET_RESULT = 0x13;
		private const int NAVIGATOR_CHILD_CARD_CLOSED = 0x21;

		public override void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback)
		{
			if (folderLocation == FolderLocations.Pictures)
			{
				using (var file = new FileStream("/accounts/1000/shared/photos/"+fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}
					
				if(steamSavedCallback != null) steamSavedCallback(true);
			}
			else if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("Save file in folder location: " + folderLocation + " is not supported.");
				if (steamSavedCallback != null) steamSavedCallback(false);
			}
			else
			{
				base.SaveFile(fileName, data, folderLocation, steamSavedCallback);
			}
		}

		public override void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("Load file in folder location: " + folderLocation + " is not supported.");
				streamLoadedCallback(null, false);
			}
			else
			{
				base.LoadFile(fileName, folderLocation, streamLoadedCallback);
			}
		}

		public override void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (folderLocation != FolderLocations.Pictures)
			{
				Debug.LogError("LoadFileDialog not supported for folder location: " + folderLocation + " on this Platform yet.");
				streamLoadedCallback(null, false);
			}
			else
			{
				if (Common.navigator_invoke_invocation_create(ref invoke) != 0) return;
				if (Common.navigator_invoke_invocation_set_target(invoke, "sys.filepicker.target") != 0)// sys.filesaver.target << use for file save dialog
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}
			
				if (Common.navigator_invoke_invocation_set_action(invoke, "bb.action.OPEN") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}
			
				if (Common.navigator_invoke_invocation_set_type(invoke, "application/vnd.blackberry.file_picker") != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}
			
				if (Common.navigator_invoke_invocation_send(invoke) != 0)
				{
					Common.navigator_invoke_invocation_destroy(invoke);
					return;
				}
			
				// wait for messge box event
				while (true)
				{
					IntPtr _event = IntPtr.Zero;
					Common.bps_get_event(ref _event, -1);// wait here for next event
					if (_event != IntPtr.Zero)
					{
						if (Common.bps_event_get_code(_event) == NAVIGATOR_CHILD_CARD_CLOSED)
						{
							IntPtr reasonPtr = Common.navigator_event_get_card_closed_reason(_event);
							string reason = Marshal.PtrToStringAnsi(reasonPtr);
							Debug.Log("reason: " + reason);
							if (reason == "save")//save - cancel
							{
								IntPtr dataPathPtr = Common.navigator_event_get_card_closed_data(_event);
								string dataPath = Marshal.PtrToStringAnsi(dataPathPtr);
								Debug.Log("Loading file from dataPath: " + dataPath);
							
								try
								{
									var matches = System.Text.RegularExpressions.Regex.Matches(dataPath, @"file\://(/accounts/1000/shared/photos/)([\w|\.]*)");
									if (matches.Count == 0) matches = System.Text.RegularExpressions.Regex.Matches(dataPath, @"file\://(/accounts/1000/shared/camera/)([\w|\.]*)");
									foreach (System.Text.RegularExpressions.Match match in matches)
									{
										if (match.Groups.Count == 3)
										{
											string path = match.Groups[1].Value;
											string fileName = match.Groups[2].Value;
										
											// check for valid file type
											bool pass = false;
											foreach (var type in fileTypes)
											{
												if (Path.GetExtension(fileName) == type)
												{
													pass = true;
													break;
												}
											}
											if (!pass) throw new Exception("Invalid file ext.");
										
											// load file
											MemoryStream stream = null;
											using (var file = new FileStream(path+fileName, FileMode.Open, FileAccess.Read))
											{
												var data = new byte[file.Length];
												file.Read(data, 0, data.Length);
												stream = new MemoryStream(data);
											}
											streamLoadedCallback(stream, true);
											return;
										}
										else
										{
											throw new Exception("Invalid dataPath.");
										}
									}
								}
								catch (Exception e)
								{
									Debug.LogError(e.Message);
								}
							
								streamLoadedCallback(null, false);
							}
							else
							{
								streamLoadedCallback(null, false);
							}
						
							break;
						}
					}
				}
			
				Common.navigator_invoke_invocation_destroy(invoke);
			}
		}
	}
}
#endif