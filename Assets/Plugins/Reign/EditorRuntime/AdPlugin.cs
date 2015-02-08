#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Reign.Plugin
{
    public class AdPlugin : IAdPlugin
    {
		public bool Visible {get; set;}
		private bool testing, guiOverride;
		private int offsetX, offsetY, width, height;
		
		public AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			guiOverride = desc.Editor_AdGUIOverrideEnabled;
			bool pass = true;
			try
			{
				Visible = desc.Visible;
				testing = desc.Testing;
				width = desc.Editor_AdWidth;
				height = desc.Editor_AdHeight;
				SetGravity(desc.Editor_AdGravity);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
				
			if (createdCallback != null) createdCallback(pass);
		}

		public void Dispose()
		{
			// do nothing...
		}

		public void SetGravity(AdGravity gravity)
		{
			switch (gravity)
			{
				case AdGravity.BottomCenter:
					offsetX = (Screen.width / 2) - (width / 2);
					offsetY = Screen.height - height;
					break;

				case AdGravity.BottomLeft:
					offsetX = 0;
					offsetY = Screen.height - height;
					break;

				case AdGravity.BottomRight:
					offsetX = Screen.width - width;
					offsetY = Screen.height - height;
					break;

				case AdGravity.TopCenter:
					offsetX = (Screen.width / 2) - (width / 2);
					offsetY = 0;
					break;

				case AdGravity.TopLeft:
					offsetX = 0;
					offsetY = 0;
					break;

				case AdGravity.TopRight:
					offsetX = Screen.width - width;
					offsetY = 0;
					break;

				case AdGravity.CenterScreen:
					offsetX = (Screen.width / 2) - (width / 2);
					offsetY = (Screen.height / 2) - (height / 2);
					break;

				default: throw new Exception("Unsuported Ad gravity: " + gravity);
			}
		}
		
		public void Refresh()
		{
			Debug.Log("Editor Ad Refreshed");	
		}

		private void onGUI()
		{
			if (Visible && testing)
			{
				if (GUI.Button(new Rect(offsetX, offsetY, width, height), "Editor Test Ad"))
				{
					Debug.Log("Ad Clicked!");
				}
			}
		}

		public void OnGUI()
		{
			if (!guiOverride) onGUI();
		}

		public void OnGUIOverride()
		{
			if (guiOverride) onGUI();
		}
		
		public void Update()
		{
			// do nothing...
		}
    }
}
#endif