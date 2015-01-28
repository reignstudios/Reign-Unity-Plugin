#if UNITY_STANDALONE_OSX
using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class MessageBoxPlugin_OSX : IMessageBoxPlugin
	{
		[DllImport("/Users/andrewwitte/Reign/Reign-Unity-Plugin/Assets/Plugins/x86/ReignNative", EntryPoint="Foo")]
		private extern static int Foo();//(IntPtr hWnd, string lpText, string lpCaption, uint uType);

		private const uint MB_OK = (uint)0x00000000L;
		private const uint MB_OKCANCEL = (uint)0x00000001L;
		private const int IDCANCEL = 2;

		public void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			if (type == MessageBoxTypes.Ok)
			{
				Foo();
				if (callback != null) callback(MessageBoxResult.Ok);
			}
			else
			{
				int result = Foo();
				Debug.Log("VALUE: " + result);
				if (callback != null) callback(result != IDCANCEL ? MessageBoxResult.Ok : MessageBoxResult.Cancel);
			}
		}
		
		public void Update()
		{
			// do nothing...
		}
	}
}
#endif