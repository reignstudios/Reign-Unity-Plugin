using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Reign;

public class InputExService : MonoBehaviour
{
	private static List<InputExButtonAnalogValue> buttonAnalogs;

	internal static InputExButtonAnalogValue FindButtonAnalog(Buttons button, ControllerPlayers player)
	{
		foreach (var analog in buttonAnalogs)
		{
			if (analog.Button == button && analog.Player == player) return analog;
		}

		return null;
	}

	void Awake()
	{
		#if UNITY_STANDALONE_WIN
		buttonAnalogs = new List<InputExButtonAnalogValue>()
		{
			new InputExButtonAnalogValue(Buttons.DPadLeft, ControllerPlayers.All),
			new InputExButtonAnalogValue(Buttons.DPadRight, ControllerPlayers.All),
			new InputExButtonAnalogValue(Buttons.DPadDown, ControllerPlayers.All),
			new InputExButtonAnalogValue(Buttons.DPadUp, ControllerPlayers.All),

			new InputExButtonAnalogValue(Buttons.DPadLeft, ControllerPlayers.Player1),
			new InputExButtonAnalogValue(Buttons.DPadRight, ControllerPlayers.Player1),
			new InputExButtonAnalogValue(Buttons.DPadDown, ControllerPlayers.Player1),
			new InputExButtonAnalogValue(Buttons.DPadUp, ControllerPlayers.Player1),

			new InputExButtonAnalogValue(Buttons.DPadLeft, ControllerPlayers.Player2),
			new InputExButtonAnalogValue(Buttons.DPadRight, ControllerPlayers.Player2),
			new InputExButtonAnalogValue(Buttons.DPadDown, ControllerPlayers.Player2),
			new InputExButtonAnalogValue(Buttons.DPadUp, ControllerPlayers.Player2),

			new InputExButtonAnalogValue(Buttons.DPadLeft, ControllerPlayers.Player3),
			new InputExButtonAnalogValue(Buttons.DPadRight, ControllerPlayers.Player3),
			new InputExButtonAnalogValue(Buttons.DPadDown, ControllerPlayers.Player3),
			new InputExButtonAnalogValue(Buttons.DPadUp, ControllerPlayers.Player3),

			new InputExButtonAnalogValue(Buttons.DPadLeft, ControllerPlayers.Player4),
			new InputExButtonAnalogValue(Buttons.DPadRight, ControllerPlayers.Player4),
			new InputExButtonAnalogValue(Buttons.DPadDown, ControllerPlayers.Player4),
			new InputExButtonAnalogValue(Buttons.DPadUp, ControllerPlayers.Player4)
		};
		#endif
	}

	void Update()
	{
		foreach (var analog in buttonAnalogs) analog.Update();
	}
}