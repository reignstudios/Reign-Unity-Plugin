// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using System.Collections;
using Reign;

public class MessageBoxDemo : MonoBehaviour
{
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
		GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< MessageBox Demo >>", uiStyle);
		if (GUI.Button(new Rect(0, offset, 64, 32), "Back")) Application.LoadLevel("MainDemo");
		offset += 34;

		// Simple OK message box
		if (GUI.Button(new Rect(0, offset, 256, 64), "Show OK MessageBox")) MessageBoxManager.Show("Yahoo", "Hello World!");

		// OK/Cancel message box
		if (GUI.Button(new Rect(Screen.width-256, offset, 256, 64), "Show OK/Cancel MessageBox")) MessageBoxManager.Show("Yahoo", "Are you Awesome!?", MessageBoxTypes.OkCancel, callback);
	}

	private void callback(MessageBoxResult result)
	{
		Debug.Log(result);
		if (result == MessageBoxResult.Ok) Debug.Log("+1 for you!");
		else if (result == MessageBoxResult.Cancel) Debug.Log("How sad...");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
