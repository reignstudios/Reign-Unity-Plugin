using UnityEngine;
using System.Collections;

namespace Reign.Tools
{
	public class InputMapper : MonoBehaviour
	{
		private string label = "Waiting for input...";

		void Update()
		{
			// map axises
			for (int i = 0; i != 20; ++i)
			{
				float value = Input.GetAxis("Axis" + (i+1));
				if (Mathf.Abs(value) >= .5f)
				{
					label = string.Format("Axis {0} of value {1}", i+1, value);
					Debug.Log(label);
				}
			}

			// map keys and buttons
			for (int i = 0; i != 430; ++i)
			{
				if (Input.GetKeyDown((KeyCode)i))
				{
					label = string.Format("Key/Button pressed {0}", (KeyCode)i);
					Debug.Log(label);
				}
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(0, 0, 512, 64), label);
		}
	}
}