using System;
using UnityEngine;
using UnityEngine.UI;
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
		private bool visible;
		public bool Visible
		{
			get {return visible;}
			set
			{
				visible = value;
				if (!desc.UseClassicGUI) adCanvas.SetActive(value);
			}
		}

		private AdDesc desc;
		private GameObject adCanvas;
		private RectTransform adRect;
		private Image adImage;
		private Texture2D guiTexture;
		private Rect guiRect;
		private float guiScale;
		private bool testing;
		private AdEventCallbackMethod adEvent;
		private int refreshRate;
		private float refreshRateTic;
		private string deviceID, externalIP, apid;
		private TextureGIF gifImage;
		private MonoBehaviour service;
		private Reign.MM_AdXML.Ad adMeta;
		private AdGravity gravity;
		private GUIStyle guiSytle;

		public MM_AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback, MonoBehaviour service)
		{
			this.service = service;

			try
			{
				this.desc = desc;
				testing = desc.Testing;
				adEvent = desc.EventCallback;

				gravity = AdGravity.CenterScreen;
				#if !DISABLE_REIGN
				#if UNITY_EDITOR
				refreshRate = desc.Editor_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Editor_MillennialMediaAdvertising_APID;
				gravity = desc.Editor_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.Editor_GuiAdScale;
				#elif UNITY_BLACKBERRY
				refreshRate = desc.BB10_MillennialMediaAdvertising_RefreshRate;
				apid = desc.BB10_MillennialMediaAdvertising_APID;
				gravity = desc.BB10_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.BB10_GuiAdScale;
				#elif UNITY_WP8
				refreshRate = desc.WP8_MillennialMediaAdvertising_RefreshRate;
				apid = desc.WP8_MillennialMediaAdvertising_APID;
				gravity = desc.WP8_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.WP8_GuiAdScale;
				#elif UNITY_METRO
				refreshRate = desc.WinRT_MillennialMediaAdvertising_RefreshRate;
				apid = desc.WinRT_MillennialMediaAdvertising_APID;
				gravity = desc.WinRT_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.WinRT_GuiAdScale;
				#elif UNITY_IOS
				refreshRate = desc.iOS_MillennialMediaAdvertising_RefreshRate;
				apid = desc.iOS_MillennialMediaAdvertising_APID;
				gravity = desc.iOS_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.iOS_GuiAdScale;
				#elif UNITY_ANDROID
				refreshRate = desc.Android_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Android_MillennialMediaAdvertising_APID;
				gravity = desc.Android_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.Android_GuiAdScale;
				#elif UNITY_STANDALONE_WIN
				refreshRate = desc.Win32_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Win32_MillennialMediaAdvertising_APID;
				gravity = desc.Win32_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.Win32_GuiAdScale;
				#elif UNITY_STANDALONE_OSX
				refreshRate = desc.OSX_MillennialMediaAdvertising_RefreshRate;
				apid = desc.OSX_MillennialMediaAdvertising_APID;
				gravity = desc.OSX_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.OSX_GuiAdScale;
				#elif UNITY_STANDALONE_LINUX
				refreshRate = desc.Linux_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Linux_MillennialMediaAdvertising_APID;
				gravity = desc.Linux_MillennialMediaAdvertising_AdGravity;
				guiScale = desc.Linux_GuiAdScale;
				#endif
				#endif

				// make sure ad refresh rate doesn't go under 1 min
				if (refreshRate < 60) refreshRate = 60;

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

				if (desc.UseClassicGUI)
				{
					guiSytle = new GUIStyle()
					{
						stretchWidth = true,
						stretchHeight = true
					};
				}
				else
				{
					// Create Ad Canvas
					adCanvas = new GameObject("MM Ad");
					GameObject.DontDestroyOnLoad(adCanvas);
					adCanvas.AddComponent<RectTransform>();
					var canvas = adCanvas.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.sortingOrder = 1000;
					adCanvas.AddComponent<CanvasScaler>();
					adCanvas.AddComponent<GraphicRaycaster>();

					// Create ad
					var ad = new GameObject("AdButtonImage");
					ad.transform.parent = adCanvas.transform;
					adRect = ad.AddComponent<RectTransform>();
					adImage = ad.AddComponent<Image>();
					adImage.sprite = Resources.Load<Sprite>("Reign/Ads/AdLoading");
					adImage.preserveAspect = true;
					var button = ad.AddComponent<Button>();
					button.onClick.AddListener(adClicked);
				}

				// set default visible state
				Visible = desc.Visible;
				SetGravity(gravity);

				// load ad
				service.StartCoroutine(init(createdCallback));
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false);
			}
		}

		private void adClicked()
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
			}

			if (adEvent != null) adEvent(AdEvents.Clicked, null);
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
				if (desc.UseClassicGUI) guiTexture = gifImage.CurrentFrame.Texture;
				else adImage.sprite = gifImage.CurrentFrame.Sprite;
				SetGravity(gravity);
				var texture = gifImage.CurrentFrame.Texture;
				Debug.Log(string.Format("Ad Image Size: {0}x{1}", texture.width, texture.height));
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
			if (desc.UseClassicGUI) guiTexture = frame.Texture;
			else adImage.sprite = frame.Sprite;
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
			if (desc.UseClassicGUI)
			{
				if (guiTexture == null) return;
				
				float screenWidth = Screen.width, screenHeight = Screen.height;
				float scale = (new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude) * guiScale;
				float adWidth = guiTexture.width * scale, adHeight = guiTexture.height * scale;
				Debug.Log(adWidth);
				switch (gravity)
				{
					case AdGravity.CenterScreen:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), (screenHeight/2)-(adHeight/2), adWidth, adHeight);
						break;

					case AdGravity.BottomCenter:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.BottomLeft:
						guiRect = new Rect(0, screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.BottomRight:
						guiRect = new Rect(screenWidth-adWidth, screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.TopCenter:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), 0, adWidth, adHeight);
						break;

					case AdGravity.TopLeft:
						guiRect = new Rect(0, 0, adWidth, adHeight);
						break;

					case AdGravity.TopRight:
						guiRect = new Rect(screenWidth-adWidth, 0, adWidth, adHeight);
						break;

					default:
						Debug.LogError("Unsuported Gravity: " + gravity);
						break;
				}
			}
			else
			{
				switch (gravity)
				{
					case AdGravity.CenterScreen:
						adRect.anchorMin = new Vector2(0, .5f-(desc.UnityUI_AdMaxHeight*.5f));
						adRect.anchorMax = new Vector2(1, .5f+(desc.UnityUI_AdMaxHeight*.5f));
						break;

					case AdGravity.BottomCenter:
						adRect.anchorMin = new Vector2(0, 0);
						adRect.anchorMax = new Vector2(1, desc.UnityUI_AdMaxHeight);
						break;

					case AdGravity.BottomLeft:
						adRect.anchorMin = new Vector2(0, 0);
						adRect.anchorMax = new Vector2(desc.UnityUI_AdMaxWidth, desc.UnityUI_AdMaxHeight);
						break;

					case AdGravity.BottomRight:
						adRect.anchorMin = new Vector2(1-desc.UnityUI_AdMaxWidth, 0);
						adRect.anchorMax = new Vector2(1, desc.UnityUI_AdMaxHeight);
						break;

					case AdGravity.TopCenter:
						adRect.anchorMin = new Vector2(0, 1-desc.UnityUI_AdMaxHeight);
						adRect.anchorMax = new Vector2(1, 1);
						break;

					case AdGravity.TopLeft:
						adRect.anchorMin = new Vector2(0, 1-desc.UnityUI_AdMaxHeight);
						adRect.anchorMax = new Vector2(desc.UnityUI_AdMaxWidth, 1);
						break;

					case AdGravity.TopRight:
						adRect.anchorMin = new Vector2(1-desc.UnityUI_AdMaxWidth, 1-desc.UnityUI_AdMaxHeight);
						adRect.anchorMax = new Vector2(1, 1);
						break;

					default:
						Debug.LogError("Unsuported Gravity: " + gravity);
						break;
				}

				adRect.offsetMin = Vector2.zero;
				adRect.offsetMax = Vector2.zero;
			}
		}
		
		public void Refresh()
		{
			Debug.Log("Refreshing Ad");
			if (!testing) service.StartCoroutine(asyncRefresh(null));
			else if (adEvent != null) adEvent(AdEvents.Refreshed, null);
		}

		private IEnumerator asyncRefresh(AdCreatedCallbackMethod createdCallback)
		{
			if (!Visible) yield break;

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
			if (desc.UseClassicGUI) guiTexture = gifImage.CurrentFrame.Texture;
			else adImage.sprite = gifImage.CurrentFrame.Sprite;
			var texture = gifImage.CurrentFrame.Texture;
			Debug.Log(string.Format("Ad Image Size: {0}x{1}", texture.width, texture.height));

			if (adEvent != null) adEvent(AdEvents.Refreshed, null);
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

		public void OnGUI()
		{
			if (!desc.GUIOverrideEnabled) onGUI();
		}

		public void OverrideOnGUI()
		{
			if (desc.UseClassicGUI) onGUI();
		}

		private void onGUI()
		{
			if (desc.UseClassicGUI && visible && guiTexture != null)
			{
				SetGravity(gravity);
				GUI.DrawTexture(guiRect, guiTexture, ScaleMode.StretchToFill);
				if (GUI.Button(guiRect, "", guiSytle)) adClicked();
			}
		}

    }
}