using UnityEngine;
using System.Collections;
using Reign;

public class InputExDemo : MonoBehaviour
{
	void Update()
	{
		InputEx.LogKeys();
		InputEx.LogAnalogs();
	}
}
