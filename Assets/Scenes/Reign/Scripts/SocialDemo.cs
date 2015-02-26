using UnityEngine;
using System.Collections;
using Reign;

public class SocialDemo : MonoBehaviour
{
	public Texture2D TestImage;

	void OnGUI()
	{
		if (GUI.Button(new Rect(0, 64, 140, 64), "Share Test Image"))
		{
			// save test image to local storage
			//StreamManager.SaveFile("TestSocial.png", TestImage.EncodeToPNG(), FolderLocations.Storage, imageSavedCallback);

			// 
			SocialManager.ShareImage(TestImage.EncodeToPNG(), Reign.Plugin.SocialShareTypes.Images);
		}
	}

	private void imageSavedCallback(bool succeeded)
	{
		// share test image
		//if (succeeded) SocialManager.ShareImage("TestSocial.png");
	}
}
