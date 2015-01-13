using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReignScores.Services.Clients
{
	public static class Client
	{
		private const string clientAPIKey = "8AC57612-B355-4F24-9B77-8C020B57E361";

		public static string Login(string apiKey, string username, string password)
		{
			string response = ResponseTool.CheckAPIKey(apiKey, clientAPIKey);
			if (response != null) return response;

			using (var conn = DataManager.CreateConnectionObject())
			{
				conn.Open();
				using (var command = conn.CreateCommand())
				{
					string passwordEncrypted = SecurityManager.Hash(password);
					command.CommandText = string.Format("SELECT ID FROM Clients WHERE Username = '{0}' and Password = '{1}'", username, passwordEncrypted);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var webResponse = new WebResponse(ResponseTypes.Succeeded)
							{
								ClientID = reader["ID"].ToString()
							};
							return ResponseTool.GenerateXML(webResponse);
						}
						else
						{
							var webResponse = new WebResponse(ResponseTypes.Error)
							{
								ErrorMessage = "Failed to find client"
							};
							return ResponseTool.GenerateXML(webResponse);
						}
					}
				}
			}
		}

		public static string CreateGame(string apiKey, string clientID, string name)
		{
			string response = ResponseTool.CheckAPIKey(apiKey, clientAPIKey);
			if (response != null) return response;

			using (var conn = DataManager.CreateConnectionObject())
			{
				conn.Open();
				using (var command = conn.CreateCommand())
				{
					string values = string.Format("(NEWID(), '{0}', '{1}', '{2}')", clientID, name, DateTime.UtcNow);
					command.CommandText = "INSERT INTO Games (ID, ClientID, Name, DateCreated) VALUES " + values;
					if (command.ExecuteNonQuery() == 1)
					{
						var webResponse = new WebResponse(ResponseTypes.Succeeded);
						return ResponseTool.GenerateXML(webResponse);
					}
					else
					{
						var webResponse = new WebResponse(ResponseTypes.Error)
						{
							ErrorMessage = "Failed to properly create game"
						};
						return ResponseTool.GenerateXML(webResponse);
					}
				}
			}
		}
	}
}