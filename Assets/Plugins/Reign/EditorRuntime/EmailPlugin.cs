#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class EmailPlugin : IEmailPlugin
	{
		public void Send(string to, string subject, string body)
		{
			Debug.Log(string.Format("(NOTE: Editor does not realy send emails out): To={0} Subject={1} Body={2}", to, subject, body));
		}
	}
}
#endif