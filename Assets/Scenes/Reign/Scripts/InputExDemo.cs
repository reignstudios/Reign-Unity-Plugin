using UnityEngine;
using System.Collections;
using Reign;

public class InputExDemo : MonoBehaviour
{
	private string label = "???";

	void Update()
	{
		// You can log input here for debuging
		//string keyLabel = InputEx.LogKeys();
		string buttonLabel = InputEx.LogButtons();
		string analogLabel = InputEx.LogAnalogs();

		if (buttonLabel != null) label = buttonLabel;
		else if (analogLabel != null) label = analogLabel;
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 256, 64), label);
	}
}
