using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class SocialDemo : MonoBehaviour
{
	public Button ShareButton, BackButton;
	public Sprite ReignLogo;

	public GameObject BB10_ShareSelectorUI;
	public Text BB10_ShareSelectorTitle;
	public Button BB10_CloseButton, BB10_ShareSelectorBBM, BB10_ShareSelectorFacebook, BB10_ShareSelectorTwitter;

	void Start ()
	{
		// bind button events
		ShareButton.Select();
		ShareButton.onClick.AddListener(shareClicked);
		BackButton.onClick.AddListener(backClicked);

		// Init the share plugin
		var desc = new SocialDesc()
		{
			BB10_ShareSelectorUI = BB10_ShareSelectorUI,
			BB10_ShareSelectorTitle = BB10_ShareSelectorTitle,
			BB10_CloseButton = BB10_CloseButton,
			BB10_ShareSelectorBBM = BB10_ShareSelectorBBM,
			BB10_ShareSelectorFacebook = BB10_ShareSelectorFacebook,
			BB10_ShareSelectorTwitter = BB10_ShareSelectorTwitter
		};
		SocialManager.Init(desc);
	}

	private void shareClicked()
	{
		var data = ReignLogo.texture.EncodeToPNG();
		SocialManager.Share(data, "Demo Text", "Reign Demo", "Reign Demo Desc", SocialShareDataTypes.Image_PNG);
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
