#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Reign.Plugin
{
	public class MessageBoxPlugin : IMessageBoxPlugin
	{
		public void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			if (type == MessageBoxTypes.Ok)
			{
				EditorUtility.DisplayDialog(title, message, "OK");
				if (callback != null) callback(MessageBoxResult.Ok);
			}
			else
			{
				bool value = EditorUtility.DisplayDialog(title, message, "OK", "Cancel");
				if (callback != null) callback(value ? MessageBoxResult.Ok : MessageBoxResult.Cancel);
			}
		}
		
		public void Update()
		{
			// do nothing...
		}
	}
}
#endif