using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Reign;

public class InputExService : MonoBehaviour
{
	void Update()
	{
		foreach (var map in InputEx.ButtonMappings) map.Update();
	}
}