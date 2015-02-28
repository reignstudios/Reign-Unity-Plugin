using UnityEngine;
using System.Collections;
using Reign;

public class SocialDemo : MonoBehaviour
{
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
	}

	void OnGUI()
	{
		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Social Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back")) Application.LoadLevel("MainDemo");
		offset += 64;

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
