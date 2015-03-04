using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class SocialDemo : MonoBehaviour
{
	public GameObject BB10_ShareSelectorUI;
	public Text BB10_ShareSelectorTitle;
	public Button BB10_CloseButton, BB10_ShareSelectorBBM, BB10_ShareSelectorFacebook, BB10_ShareSelectorTwitter;
	public Texture2D TestImage;
	GUIStyle uiStyle;

	void Start ()
	{
		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white}
		};

		// Init the share plugin
		var desc = new SocialDesc()
		{
			BB10_ShareSelectorUI = BB10_ShareSelectorUI,
			BB10_ShareSelectorTitle = BB10_ShareSelectorTitle,
			BB10_CloseButton = BB10_CloseButton,
			BB10_ShareSelectorBBM = BB10_ShareSelectorBBM,
			BB10_ShareSelectorFacebook = BB10_ShareSelectorFacebook,
			BB10_ShareSelectorTwitter = BB10_ShareSelectorTwitter
		};
		SocialManager.Init(desc);
	}

	void OnGUI()
	{
		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Social Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back")) Application.LoadLevel("MainDemo");
		offset += 64;

		// share png or jpg image data
		if (GUI.Button(new Rect(0, offset, 140, 64), "Share Test Image"))
		{
			SocialManager.Share(TestImage.EncodeToPNG(), "Reign Demo", "Reign Demo Desc", SocialShareTypes.Image_PNG);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
