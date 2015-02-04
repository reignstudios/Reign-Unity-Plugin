using UnityEngine;
using System.Collections.Generic;

namespace Reign
{
	public enum ButtonTypes
	{
		// Cross buttons
		CrossButtonLeft,
		CrossButtonRight,
		CrossButtonBottom,
		CrossButtonTop,

		// DPad buttons
		DPadLeft,
		DPadRight,
		DPadDown,
		DPadUp,

		// Bumper buttons
		BumperLeft,
		BumperRight,

		// Analog buttons
		AnalogLeft,
		AnalogRight,

		// System buttons
		Start,
		Back
	}

	public enum AnalogTypes
	{
		AxisLeftX,
		AxisLeftY,
		AxisRightX,
		AxisRightY,
		TriggerLeft,
		TriggerRight
	}

	public enum ControllerPlayers
	{
		Any,
		Player1,
		Player2,
		Player3,
		Player4
	}

	public class InputExButtonMap
	{
		public ButtonTypes Type;
		public ControllerPlayers Player;
		public string Name, AnalogName;
		public bool AnalogPositive;
		public bool AnalogOn, AnalogDown, AnalogUp;

		public InputExButtonMap(ButtonTypes type, ControllerPlayers player, string name)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
		}

		public InputExButtonMap(ButtonTypes type, ControllerPlayers player, string name, string analogName, bool analogPositive)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
			this.AnalogName = analogName;
			this.AnalogPositive = analogPositive;
		}

		public void Update()
		{
			if (AnalogName == null) return;

			bool lastOn = AnalogOn;
			if (AnalogPositive) AnalogOn = Input.GetAxisRaw(AnalogName) >= .5;
			else AnalogOn = Input.GetAxisRaw(AnalogName) <= -.5;

			if (AnalogOn && !lastOn) AnalogDown = true;
			else AnalogDown = false;

			if (!AnalogOn && lastOn) AnalogUp = true;
			else AnalogUp = false;
		}
	}

	public class InputExAnalogMap
	{
		public AnalogTypes Type;
		public ControllerPlayers Player;
		public string Name;

		public InputExAnalogMap(AnalogTypes type, ControllerPlayers player, string name)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
		}
	}

	public static class InputEx
	{
		public static float AnalogTolerance = .25f, AnalogTriggerTolerance = .1f;
		public static InputExButtonMap[] ButtonMappings;
		public static InputExAnalogMap[] AnalogMappings;

		static InputEx()
		{
			ButtonMappings = new InputExButtonMap[]
			{
				// Cross buttons
				new InputExButtonMap(ButtonTypes.CrossButtonLeft, ControllerPlayers.Any, "Left CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonRight, ControllerPlayers.Any, "Right CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonBottom, ControllerPlayers.Any, "Bottom CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonTop, ControllerPlayers.Any, "Top CrossButton - PlayerAny"),

				// DPad buttons
				#if UNITY_STANDALONE_WIN
				new InputExButtonMap(ButtonTypes.DPadLeft, ControllerPlayers.Any, "Left DPadButton - PlayerAny", "Horizontal DPadAnalog - PlayerAny", false),
				new InputExButtonMap(ButtonTypes.DPadRight, ControllerPlayers.Any, "Right DPadButton - PlayerAny", "Horizontal DPadAnalog - PlayerAny", true),
				new InputExButtonMap(ButtonTypes.DPadDown, ControllerPlayers.Any, "Down DPadButton - PlayerAny", "Vertical DPadAnalog - PlayerAny", false),
				new InputExButtonMap(ButtonTypes.DPadUp, ControllerPlayers.Any, "Up DPadButton - PlayerAny", "Vertical DPadAnalog - PlayerAny", true),
				#else
				new InputExButtonMap(ButtonTypes.DPadLeft, ControllerPlayers.Any, "Left DPadButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.DPadRight, ControllerPlayers.Any, "Right DPadButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.DPadDown, ControllerPlayers.Any, "Down DPadButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.DPadUp, ControllerPlayers.Any, "Up DPadButton - PlayerAny"),
				#endif

				// Bumper buttons
				new InputExButtonMap(ButtonTypes.BumperLeft, ControllerPlayers.Any, "Left Bumper - PlayerAny"),
				new InputExButtonMap(ButtonTypes.BumperRight, ControllerPlayers.Any, "Right Bumper - PlayerAny"),

				// Analog buttons
				new InputExButtonMap(ButtonTypes.AnalogLeft, ControllerPlayers.Any, "Left AnalogButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.AnalogRight, ControllerPlayers.Any, "Right AnalogButton - PlayerAny"),

				// System buttons
				new InputExButtonMap(ButtonTypes.Start, ControllerPlayers.Any, "Start Button - PlayerAny"),
				new InputExButtonMap(ButtonTypes.Back, ControllerPlayers.Any, "Back Button - PlayerAny")
			};

			AnalogMappings = new InputExAnalogMap[]
			{
				// Sticks
				new InputExAnalogMap(AnalogTypes.AxisLeftX, ControllerPlayers.Any, "Left AnalogX - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisLeftY, ControllerPlayers.Any, "Left AnalogY - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisRightX, ControllerPlayers.Any, "Right AnalogX - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisRightY, ControllerPlayers.Any, "Right AnalogY - PlayerAny"),

				// Triggers
				new InputExAnalogMap(AnalogTypes.TriggerLeft, ControllerPlayers.Any, "Left Trigger - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.TriggerRight, ControllerPlayers.Any, "Right Trigger - PlayerAny")
			};
		}

		public static string LogKeys()
		{
			string label = null;
			for (int i = 0; i != 430; ++i)
			{
				var key = (KeyCode)i;
				if (Input.GetKeyDown(key))
				{
					label = "KeyPressed: " + key;
					Debug.Log(label);
				}
			}

			return label;
		}

		public static string LogButtons()
		{
			string label = null;
			for (int i = 0; i != 14; ++i)
			{
				var button = (ButtonTypes)i;
				if (InputEx.GetButtonDown(button, ControllerPlayers.Any))
				{
					label = "ButtonPressed: " + button;
					Debug.Log(label);
				}
			}

			return label;
		}

		public static string LogAnalogs()
		{
			string label = null;
			for (int i = 0; i != 6; ++i)
			{
				var analog = (AnalogTypes)i;
				float value = GetAxis(analog, ControllerPlayers.Any);
				if (value != 0)
				{
					label = string.Format("AnalogType {0} value: {1}", analog, value);
					Debug.Log(label);
				}
			}

			return label;
		}

		private static string findButtonName(ButtonTypes type, ControllerPlayers player, out InputExButtonMap mapping)
		{
			foreach (var map in ButtonMappings)
			{
				if (map.Type == type && map.Player == player)
				{
					mapping = map;
					return map.Name;
				}
			}

			Debug.LogError(string.Format("Failed to find Button {0} for Player {1}", type, player));
			mapping = null;
			return "Unknown";
		}

		public static bool GetButton(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.AnalogOn) return true;

			return Input.GetButton(name);
		}

		public static bool GetButtonDown(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.AnalogDown) return true;

			return Input.GetButtonDown(name);
		}

		public static bool GetButtonUp(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.AnalogUp) return true;

			return Input.GetButtonUp(name);
		}

		private static string findAnalogName(AnalogTypes type, ControllerPlayers player)
		{
			foreach (var map in AnalogMappings)
			{
				if (map.Type == type && map.Player == player)
				{
					return map.Name;
				}
			}

			Debug.LogError(string.Format("Failed to find Analog {0} for Player {1}", type, player));
			return "Unknown";
		}

		private static float processAnalogValue(AnalogTypes type, float value)
		{
			float tolerance = AnalogTolerance;
			switch (type)
			{
				case AnalogTypes.TriggerLeft:
				case AnalogTypes.TriggerRight:
					tolerance = AnalogTriggerTolerance;
					break;
			}

			return (Mathf.Abs(value) <= tolerance) ? 0 : value;
		}

		public static float GetAxis(AnalogTypes type, ControllerPlayers player)
		{
			return processAnalogValue(type, Input.GetAxisRaw(findAnalogName(type, player)));
		}

		public static float GetAxisRaw(AnalogTypes type, ControllerPlayers player)
		{
			return Input.GetAxisRaw(findAnalogName(type, player));
		}
	}
}