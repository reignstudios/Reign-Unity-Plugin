using UnityEngine;
using System.Collections;
using Reign;

public class InputExDemo : MonoBehaviour
{
	private string label = "???";

	void Update()
	{
		// You can log input here for debuging...

		// All button and key input
		//string keyLabel = InputEx.LogKeys();
		//if (keyLabel != null)
		//{
		//	label = keyLabel;
		//	return;
		//}

		// All GamePad input
		string buttonLabel = InputEx.LogButtons();
		string analogLabel = InputEx.LogAnalogs();

		if (buttonLabel != null) label = buttonLabel;
		else if (analogLabel != null) label = analogLabel;

		// Input use case examples
		//if (InputEx.GetButton(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetButtonDown(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetButtonUp(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetAxis(AnalogTypes.AxisLeftX, ControllerPlayers.Any) >= .1f);// do soething...
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 64, 32), "Back"))
		{
			gameObject.SetActive(false);
			Application.LoadLevel("MainDemo");
			return;
		}

		GUI.Label(new Rect(0, 128, 256, 64), label);
	}
}
