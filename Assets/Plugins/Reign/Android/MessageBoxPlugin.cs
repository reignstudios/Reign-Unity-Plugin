#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class MessageBoxPlugin : IMessageBoxPlugin
	{
		private AndroidJavaClass native;
		private MessageBoxCallback callback;
	
		public MessageBoxPlugin()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.MessageBoxNative");
		}
		
		~MessageBoxPlugin()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}
	
		public void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			this.callback = callback;
			native.CallStatic("Show", title, message, convertType(type));
		}
		
		private int convertType(MessageBoxTypes type)
		{
			switch (type)
			{
				case MessageBoxTypes.Ok: return 0;
				case MessageBoxTypes.OkCancel: return 1;
				default: throw new Exception("Unsuported MessageBoxType: " + type);
			}
		}
		
		public void Update()
		{
			if (native.CallStatic<bool>("GetOkStatus") && callback != null) callback(MessageBoxResult.Ok);
			if (native.CallStatic<bool>("GetCancelStatus") && callback != null) callback(MessageBoxResult.Cancel);
		}
	}
}
#endif