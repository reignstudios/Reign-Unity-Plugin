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
		Generic,
		Xbox360,
		PS3
	}

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

		public static void LogAnalogs()
		{
			for (int i = 0; i != 6; ++i)
			{
				var analog = (AnalogTypes)i;
				float value = GetAxis(analog, ControllerPlayers.All);
				if (value != 0) Debug.Log(string.Format("AnalogType {0} value: {1}", analog, value));
			}
		}

		public static string GetPlatformButtonName(Buttons key)
		{
			// Cross buttons
			if (key == Buttons.Button1)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "A";
				else if (ControllerTarget == ControllerTargets.PS3) return "X";
			}
			else if (key == Buttons.Button2)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "B";
				else if (ControllerTarget == ControllerTargets.PS3) return "Circle";
			}
			else if (key == Buttons.Button3)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "X";
				else if (ControllerTarget == ControllerTargets.PS3) return "Square";
			}
			else if (key == Buttons.Button4)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return "Y";
				else if (ControllerTarget == ControllerTargets.PS3) return "Triangle";
			}

			// System buttons
			else if (key == Buttons.Start)
			{
				return key.ToString();
			}
			else if (key == Buttons.Back)
			{
				if (ControllerTarget == ControllerTargets.PS3) return "Select";
			}

			return key.ToString();
		}

		public static KeyCode ConvertKeyCode(Buttons key, ControllerPlayers player)
		{
			// Cross buttons
			if (key == Buttons.Button1)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton0;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.Button2)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton1;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.Button3)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton2;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.Button4)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton3;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// DPad buttons
			else if (key == Buttons.DPadLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;// TODO
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (key == Buttons.DPadRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (key == Buttons.DPadDown)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}
			else if (key == Buttons.DPadUp)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.None;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.None;
			}

			// Bumper buttons
			else if (key == Buttons.BumperLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton4;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.BumperRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton5;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// System buttons
			else if (key == Buttons.Start)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton7;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.Back)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton6;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			// Analog buttons
			else if (key == Buttons.AnalogLeft)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton8;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}
			else if (key == Buttons.AnalogRight)
			{
				if (ControllerTarget == ControllerTargets.Xbox360) return KeyCode.JoystickButton9;
				else if (ControllerTarget == ControllerTargets.PS3) return KeyCode.JoystickButton0;
			}

			return KeyCode.None;
		}

		public static bool GetButton(Buttons key, ControllerPlayers player)
		{
			return Input.GetKey(ConvertKeyCode(key, player));
		}

		public static bool GetButtonDown(Buttons key, ControllerPlayers player)
		{
			return Input.GetKeyDown(ConvertKeyCode(key, player));
		}

		public static bool GetButtonUp(Buttons key, ControllerPlayers player)
		{
			return Input.GetKeyUp(ConvertKeyCode(key, player));
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