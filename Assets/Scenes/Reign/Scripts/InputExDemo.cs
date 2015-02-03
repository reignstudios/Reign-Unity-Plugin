using UnityEngine;
using System.Collections;
using Reign;

public class InputExDemo : MonoBehaviour
{
	void Update()
	{
		// You can log input here for debuging
		//InputEx.LogKeys();
		InputEx.LogButtons(true);
		InputEx.LogAnalogs();
	}
}
