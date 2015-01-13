using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManageGamesManager : MonoBehaviour
{
	public Toggle[] GameItems;

	void Start()
	{
		foreach (var item in GameItems)
		{
			item.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		
	}
}
