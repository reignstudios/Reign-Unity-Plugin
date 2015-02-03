using UnityEngine;
using System.Collections;

namespace Reign
{
	public enum Buttons
	{
		// Cross buttons
		Button1,
		Button2,
		Button3,
		Button4,

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
		All,
		Player1,
		Player2,
		Player3,
		Player4,
		//Player5,
		//Player6,
		//Player7,
		//Player8
	}

	public enum ControllerTargets
	{
		Custom,
		Xbox360,
		PS3
	}

	#if UNITY_STANDALONE_WIN
	class InputExButtonAnalogValue
	{
		public Buttons Button;
		public ControllerPlayers Player;
		public bool On, Down, Up;

		public InputExButtonAnalogValue(Buttons button, ControllerPlayers player)
		{
			this.Button = button;
			this.Player = player;
		}

		public void Update()
		{
			bool lastOn = On;
			On = false;
			Down = false;
			Up = false;
			switch (Button)
			{
				case Buttons.DPadLeft: On = Input.GetAxisRaw(string.Format("Axis{0}_{1}", 6, Player)) < -.5; break;
				case Buttons.DPadRight: On = Input.GetAxisRaw(string.Format("Axis{0}_{1}", 6, Player)) > .5; break;
				case Buttons.DPadDown: On = Input.GetAxisRaw(string.Format("Axis{0}_{1}", 7, Player)) < -.5; break;
				case Buttons.DPadUp: On = Input.GetAxisRaw(string.Format("Axis{0}_{1}", 7, Player)) > .5; break;
				default:
					Debug.LogError("Unsuported Button Analog type: " + Button);
					return;
			}

			if (On && !lastOn) Down = true;
			if (!On && lastOn) Up = true;
		}
	}
	#endif

	public static class InputEx
	{
		public static ControllerTargets ControllerTarget = ControllerTargets.Xbox360;
		public static float AnalogTolerance = .25f;

		// generic button mapping
		public static KeyCode GenericButton1 = KeyCode.JoystickButton0;
		public static KeyCode GenericButton2 = KeyCode.JoystickButton1;
		public static KeyCode GenericButton3 = KeyCode.JoystickButton2;
		public static KeyCode GenericButton4 = KeyCode.JoystickButton3;

		public static void LogKeys()
		{
			for (int i = 0; i != 430; ++i)
			{
				var key = (KeyCode)i;
				if (Input.GetKeyDown(key)) Debug.Log("KeyPressed: " + key);
			}
		}

		public static void LogButtons(bool convertNames)
		{
			for (int i = 0; i != 14; ++i)
			{
				var button = (Buttons)i;
				if (InputEx.GetButtonDown(button, ControllerPlayers.All))
				{
					if (convertNames) Debug.Log("ButtonPressed: " + GetPlatformButtonName(button));
					else Debug.Log("ButtonPressed: " + button);
				}
			}
		}

		public static void LogAnalogs()
		{
			for (int i = 0; i != 6; ++i)
			{
				var analog = (AnalogTypes)i;
				float value = GetAxis(analog, ControllerPlayers.All);
				if (value != 0) Debug.Log(string.Format("AnalogType {0} value: {1}", analog, value));
			}
		}

		public static string GetPlatformButtonName(Buttons button)
		{
			// Cross buttons
			if (button == Buttons.Button1)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "A";
				else if (ControllerTarget == ControllerTargets.PS3) return "X";
			}
			else if (button == Buttons.Button2)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "B";
				else if (ControllerTarget == ControllerTargets.PS3) return "Circle";
			}
			else if (button == Buttons.Button3)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "X";
				else if (ControllerTarget == ControllerTargets.PS3) return "Square";
			}
			else if (button == Buttons.Button4)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "Y";
				else if (ControllerTarget == ControllerTargets.PS3) return "Triangle";
			}

			// System buttons
			else if (button == Buttons.Start)
			{
				return button.ToString();
			}
			else if (button == Buttons.Back)
			{
				if (ControllerTarget == ControllerTargets.PS3) return "Select";
			}

			return button.ToString();
		}

		public static KeyCode ConvertKeyCode(Buttons button, ControllerPlayers player)
		{
			// Cross buttons
			if (button == Buttons.Button1)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton0;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.Button2)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton1;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.Button3)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton2;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.Button4)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton3;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// DPad buttons
			else if (button == Buttons.DPadLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;// TODO
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (button == Buttons.DPadRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (button == Buttons.DPadDown)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (button == Buttons.DPadUp)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}

			// Bumper buttons
			else if (button == Buttons.BumperLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton4;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.BumperRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton5;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// System buttons
			else if (button == Buttons.Start)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton7;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.Back)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton6;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// Analog buttons
			else if (button == Buttons.AnalogLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton8;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (button == Buttons.AnalogRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton9;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			return KeyCode.None;
		}

		public static bool GetButton(Buttons button, ControllerPlayers player)
		{
			#if UNITY_STANDALONE_WIN
			if (ControllerTarget == ControllerTargets.Xbox360)
			{
				var analog = InputExService.FindButtonAnalog(button, player);
				if (analog != null) return analog.On;
				else return Input.GetKey(ConvertKeyCode(button, player));
			}
			#endif

			return Input.GetKey(ConvertKeyCode(button, player));
		}

		public static bool GetButtonDown(Buttons button, ControllerPlayers player)
		{
			#if UNITY_STANDALONE_WIN
			if (ControllerTarget == ControllerTargets.Xbox360)
			{
				var analog = InputExService.FindButtonAnalog(button, player);
				if (analog != null) return analog.Down;
				else return Input.GetKeyDown(ConvertKeyCode(button, player));
			}
			#endif

			return Input.GetKeyDown(ConvertKeyCode(button, player));
		}

		public static bool GetButtonUp(Buttons button, ControllerPlayers player)
		{
			#if UNITY_STANDALONE_WIN
			if (ControllerTarget == ControllerTargets.Xbox360)
			{
				var analog = InputExService.FindButtonAnalog(button, player);
				if (analog != null) return analog.Up;
				else return Input.GetKeyUp(ConvertKeyCode(button, player));
			}
			#endif

			return Input.GetKeyUp(ConvertKeyCode(button, player));
		}

		private static string getAnalogTypeName(AnalogTypes type, ControllerPlayers player)
		{
			const string axisFormat = "Axis{0}_{1}";
			if (ControllerTarget == ControllerTargets.Xbox360)
			{
				switch (type)
				{
					case AnalogTypes.AxisLeftX: return string.Format(axisFormat, 1, player);
					case AnalogTypes.AxisLeftY: return string.Format(axisFormat, 2, player);
					case AnalogTypes.AxisRightX: return string.Format(axisFormat, 4, player);
					case AnalogTypes.AxisRightY: return string.Format(axisFormat, 5, player);
					case AnalogTypes.TriggerLeft: return string.Format(axisFormat, 9, player);
					case AnalogTypes.TriggerRight: return string.Format(axisFormat, 10, player);
					default: return "Unsuported AnalogType: " + type;
				}
			}
			
			Debug.LogError("Unknown ControllerTarget: " + ControllerTarget);
			return "Unknown";
		}

		private static float processAnalogValue(float value)
		{
			return (Mathf.Abs(value) <= AnalogTolerance) ? 0 : value;
		}

		public static float GetAxis(AnalogTypes type, ControllerPlayers player)
		{
			return processAnalogValue(Input.GetAxisRaw(getAnalogTypeName(type, player)));
		}

		public static float GetAxisRaw(AnalogTypes type, ControllerPlayers player)
		{
			return Input.GetAxisRaw(getAnalogTypeName(type, player));
		}
	}
}