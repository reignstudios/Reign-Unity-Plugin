using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

#if UNITY_EDITOR || UNITY_STANDALONE
using System.Security.Cryptography;
#endif

namespace Reign.MM_AdXML
{
	public class TextContent
	{
		[XmlText] public string Content;
	}

	public class Image
	{
		[XmlElement("url")] public TextContent url;
		[XmlElement("mime_type")] public TextContent mime_type;
		[XmlElement("height")] public TextContent height;
		[XmlElement("width")] public TextContent width;
		[XmlElement("altText")] public TextContent altText;
	}

	[XmlRoot("ad")]
	public class Ad
	{
		[XmlElement("bodyType")] public TextContent bodyType;
		[XmlElement("clickUrl")] public TextContent clickUrl;
		[XmlElement("image")] public Image image;
		[XmlElement("text")] public TextContent text;
	}
}

namespace Reign.Plugin
{
    public class MM_AdPlugin : IAdPlugin
    {
		public bool Visible {get; set;}
		private bool testing, guiOverride;
		private AdEventCallbackMethod adEvent;
		private int offsetX, offsetY, width, height, refreshRate;
		private float refreshRateTic, adScale;
		private AdGravity adGravity;
		private string deviceID, externalIP, apid;
		private Texture adImage;
		private TextureGIF gifImage;
		private MonoBehaviour service;
		private GUIStyle style;
		private Reign.MM_AdXML.Ad adMeta;

		public MM_AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback, MonoBehaviour service)
		{
			this.service = service;

			try
			{
				style = new GUIStyle();
				Visible = desc.Visible;
				testing = desc.Testing;
				adEvent = desc.EventCallback;

				#if UNITY_EDITOR
				adGravity = desc.Editor_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.Editor_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.Editor_GuiAdScale;
				apid = desc.Editor_MillennialMediaAdvertising_APID;
				guiOverride = desc.Editor_AdGUIOverrideEnabled;
				#elif UNITY_BLACKBERRY
				adGravity = desc.BB10_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.BB10_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.BB10_GuiAdScale;
				apid = desc.BB10_MillennialMediaAdvertising_APID;
				guiOverride = desc.BB10_AdGUIOverrideEnabled;
				#elif UNITY_WP8
				adGravity = desc.WP8_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.WP8_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.WP8_GuiAdScale;
				apid = desc.WP8_MillennialMediaAdvertising_APID;
				guiOverride = desc.WP8_AdGUIOverrideEnabled;
				#elif UNITY_METRO
				adGravity = desc.WinRT_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.WinRT_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.WinRT_GuiAdScale;
				apid = desc.WinRT_MillennialMediaAdvertising_APID;
				guiOverride = desc.WinRT_AdGUIOverrideEnabled;
				#elif UNITY_IOS
				adGravity = desc.iOS_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.iOS_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.iOS_GuiAdScale;
				apid = desc.iOS_MillennialMediaAdvertising_APID;
				guiOverride = desc.iOS_AdGUIOverrideEnabled;
				#elif UNITY_ANDROID
				adGravity = desc.Android_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.Android_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.Android_GuiAdScale;
				apid = desc.Android_MillennialMediaAdvertising_APID;
				guiOverride = desc.Android_AdGUIOverrideEnabled;
				#elif UNITY_STANDALONE_WIN
				adGravity = desc.Win32_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.Win32_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.Win32_GuiAdScale;
				apid = desc.Win32_MillennialMediaAdvertising_APID;
				guiOverride = desc.Win32_AdGUIOverrideEnabled;
				#elif UNITY_STANDALONE_OSX
				adGravity = desc.OSX_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.OSX_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.OSX_GuiAdScale;
				apid = desc.OSX_MillennialMediaAdvertising_APID;
				guiOverride = desc.OSX_AdGUIOverrideEnabled;
				#elif UNITY_STANDALONE_LINUX
				adGravity = desc.Linux_MillennialMediaAdvertising_AdGravity;
				refreshRate = desc.Linux_MillennialMediaAdvertising_RefreshRate;
				adScale = desc.Linux_GuiAdScale;
				apid = desc.Linux_MillennialMediaAdvertising_APID;
				guiOverride = desc.Linux_AdGUIOverrideEnabled;
				#endif

				// create or get device ID
				if (PlayerPrefs.HasKey("Reign_MMWebAds_DeviceID"))
				{
					deviceID = PlayerPrefs.GetString("Reign_MMWebAds_DeviceID");
				}
				else
				{
					#if UNITY_EDITOR || UNITY_STANDALONE
					var hash = new SHA1CryptoServiceProvider().ComputeHash(Guid.NewGuid().ToByteArray());
					deviceID = BitConverter.ToString(hash).ToLower();
					#else
					deviceID = Guid.NewGuid().ToString().Replace("-", "0").ToLower() + "0000";
					#endif

					PlayerPrefs.SetString("Reign_MMWebAds_DeviceID", deviceID);
				}

				// load ad
				service.StartCoroutine(init(createdCallback));
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false);
			}
		}

		private IEnumerator init(AdCreatedCallbackMethod createdCallback)
		{
			// request Ad
			if (testing)
			{
				var www = new WWW("http://media.mydas.mobi/images/rich/T/test_mm/collapsed.gif");
				yield return www;

				var data = www.bytes;
				if (data == null || data.Length == 0)
				{
					Debug.LogError("Test Ad failed to loadb");
					if (createdCallback != null) createdCallback(false);
					yield break;
				}

				gifImage = new TextureGIF(data, frameUpdatedCallback);
				adImage = gifImage.CurrentFrame.Texture;
				width = (int)(adImage.width * adScale);
				height = (int)(adImage.height * adScale);
				Debug.Log(string.Format("Ad Image Size: {0}x{1}", width, height));
				SetGravity(adGravity);
				if (createdCallback != null) createdCallback(true);
				if (adEvent != null) adEvent(AdEvents.Refreshed, null);
			}
			else
			{
				// get external IP address
				var ipWWW = new WWW("http://checkip.dyndns.org/");
				yield return ipWWW;
				var match = Regex.Match(ipWWW.text, @"Current IP Address\: (\d*\.\d*\.\d*\.\d*)");
				if (!match.Success)
				{
					if (createdCallback != null) createdCallback(false);
					yield break;
				}
				externalIP = match.Groups[1].Value;
				Debug.Log("External IP: " + externalIP);

				// load ad
				service.StartCoroutine(asyncRefresh(createdCallback));
			}
		}

		private void frameUpdatedCallback(TextureGIFFrame frame)
		{
			adImage = frame.Texture;
		}

		public void Dispose()
		{
			if (gifImage != null)
			{
				gifImage.Dispose();
				gifImage = null;
			}
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

			adGravity = gravity;
		}
		
		public void Refresh()
		{
			Debug.Log("Refreshing Ad");
			if (!testing) service.StartCoroutine(asyncRefresh(null));
			else if (adEvent != null) adEvent(AdEvents.Refreshed, null);
		}

		private IEnumerator asyncRefresh(AdCreatedCallbackMethod createdCallback)
		{
			string url = "http://ads.mp.mydas.mobi/getAd.php5?";
			url += "&apid=" + apid;// ID
			url += "&auid=" + deviceID;// Device UID Hash HEX value
			//url += "&ua=" + "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.137 Safari/537.36";
			url += "&ua=";
			url += "&uip=" + externalIP;
			var www = new WWW(url);
			yield return www;

			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, www.error);
				yield break;
			}

			if (www.text.Contains(@"""error"""))
			{
				Debug.LogError(www.text);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, www.text);
				yield break;
			}

			//Debug.Log("Request Text: " + www.text);
			if (string.IsNullOrEmpty(www.text))
			{
				string error = "Invalid server responce! No data!";
				Debug.LogError(error);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, error);
				yield break;
			}

			var xml = new XmlSerializer(typeof(Reign.MM_AdXML.Ad));
			using (var data = new MemoryStream(www.bytes))
			{
				adMeta = (Reign.MM_AdXML.Ad)xml.Deserialize(data);
			}

			string imageURL = adMeta.image.url.Content;
			Debug.Log("MMWeb Ad ImageURL: " + imageURL);
			www = new WWW(imageURL);
			yield return www;

			if (gifImage != null)
			{
				gifImage.Dispose();
				gifImage = null;
			}
			gifImage = new TextureGIF(www.bytes, frameUpdatedCallback);
			adImage = gifImage.CurrentFrame.Texture;
			width = (int)(adImage.width * adScale);
			height = (int)(adImage.height * adScale);
			Debug.Log(string.Format("Ad Image Size: {0}x{1}", width, height));
			SetGravity(adGravity);

			if (adEvent != null) adEvent(AdEvents.Refreshed, null);
		}

		private void onGUI()
		{
			if (Visible && gifImage != null && adImage != null)
			{
				GUI.DrawTexture(new Rect(offsetX, offsetY, width, height), adImage);
				if (GUI.Button(new Rect(offsetX, offsetY, width, height), "", style))
				{
					if (testing)
					{
						Debug.Log("Ad Clicked!");
						Application.OpenURL("http://www.millennialmedia.com/");
					}
					else
					{
						Debug.Log("Opening Ad at URL: " + adMeta.clickUrl.Content);
						Application.OpenURL(adMeta.clickUrl.Content);
						Debug.Log("Ad Clicked!");
						if (adEvent != null) adEvent(AdEvents.Clicked, null);
					}
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
			if (gifImage != null) gifImage.Update();

			refreshRateTic += Time.deltaTime;
			if (refreshRateTic >= refreshRate)
			{
				refreshRateTic = 0;
				Refresh();
			}
		}
    }
}