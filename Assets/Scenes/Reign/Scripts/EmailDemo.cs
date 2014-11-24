// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections;
using Reign;

public class EmailDemo : MonoBehaviour
{
	GUIStyle uiStyle;

	void Start ()
	{
		uiStyle = new GUIStyle()
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 32,
			normal = new GUIStyleState() {textColor = Color.white},
		};
	}

	void OnGUI()
	{
		float offset = 0;
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Email Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back")) Application.LoadLevel("MainDemo");
		offset += 34;

		float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;
		if (GUI.Button(new Rect((Screen.width/2)-(64), (Screen.height/2)-(32*scale)+offset, 128, 64*scale), "Send Email"))
		{
			EmailManager.Send("support@reign-studios.com", "Subject", "Some body content...");
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
