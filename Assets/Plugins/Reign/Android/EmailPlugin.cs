#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class EmailPlugin : IEmailPlugin
	{
		private AndroidJavaClass native;
		
		public EmailPlugin()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.EmailNative");
		}
		
		~EmailPlugin()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}
	
		public void Send(string to, string subject, string body)
		{
			native.CallStatic("Send", to, subject, body);
		}
	}
}
#endif