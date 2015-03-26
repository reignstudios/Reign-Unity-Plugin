#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Reign.Plugin
{
    public class AdPlugin : IAdPlugin
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
		private Rect guiRect;
		
		public AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			bool pass = true;
			try
			{
				this.desc = desc;

				if (!desc.UseClassicGUI)
				{
					// Create Ad Canvas
					adCanvas = new GameObject("Editor Ad");
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
					var image = ad.AddComponent<Image>();
					image.sprite = Resources.Load<Sprite>("Reign/Ads/DemoAd");
					image.preserveAspect = true;
					var button = ad.AddComponent<Button>();
					button.onClick.AddListener(adClicked);
				}

				// set default visible state and gravity
				Visible = desc.Visible;
				SetGravity(desc.Editor_AdGravity);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
				
			if (createdCallback != null) createdCallback(pass);
		}

		private void adClicked()
		{
			Debug.Log("Ad Clicked!");
			if (desc.EventCallback != null) desc.EventCallback(AdEvents.Clicked, null);
		}

		public void Dispose()
		{
			if (!desc.UseClassicGUI && adCanvas != null)
			{
				GameObject.Destroy(adCanvas);
				adCanvas = null;
			}
		}

		public void SetGravity(AdGravity gravity)
		{
			if (desc.UseClassicGUI)
			{
				float screenWidth = Screen.width, screenHeight = Screen.height;
				float scale = new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude;
				float adWidth = 320 * desc.Editor_GuiAdScale * scale, adHeight = 53 * desc.Editor_GuiAdScale * scale;
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
			Debug.Log("Editor Ad Refreshed");
			if (desc.EventCallback != null) desc.EventCallback(AdEvents.Refreshed, null);
		}
		
		public void Update()
		{
			// do nothing...
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
			if (desc.UseClassicGUI && visible)
			{
				SetGravity(desc.Editor_AdGravity);
				if (GUI.Button(guiRect, "Reign Test Ad")) adClicked();
			}
		}
    }
}
#endif