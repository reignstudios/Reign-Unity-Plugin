// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Main Reign delegate class.
/// </summary>
public class ReignServices : MonoBehaviour
{
	/// <summary>
	/// Current instance of the ReignServices object.
	/// </summary>
	public static ReignServices Singleton {get; private set;}

	internal bool requestFrameDone, canDestroy;
	internal delegate void frameDoneCallbackMethod();
	internal frameDoneCallbackMethod frameDoneCallback;

	private static int lastScreenWidth, lastScreenHeight;
	public delegate void ScreenSizeChangedCallbackMethod(int oldWidth, int oldHeight, int newWidth, int newHeight);
	public static event ScreenSizeChangedCallbackMethod ScreenSizeChangedCallback;

	/// <summary>
	/// Sevice delegate method.
	/// </summary>
	public delegate void ServiceMethod();
	private static event ServiceMethod updateService, onguiService, destroyService;
	private static List<ServiceMethod>[] invokeOnUnityThreadCallbacks;
	private static int invokeSwap;

	/// <summary>
	/// Used to add service callbacks.
	/// </summary>
	/// <param name="update">Update callback.</param>
	/// <param name="onGUI">OnGUI callback.</param>
	/// <param name="destroy">OnDestroy callback.</param>
	public static void AddService(ServiceMethod update, ServiceMethod onGUI, ServiceMethod destroy)
	{
		if (update != null)
		{
			updateService -= update;
			updateService += update;
		}

		if (onGUI != null)
		{
			onguiService -= onGUI;
			onguiService += onGUI;
		}

		if (destroy != null)
		{
			destroyService -= destroy;
			destroyService += destroy;
		}
	}

	/// <summary>
	/// Used to remove service callbacks.
	/// </summary>
	/// <param name="update">Update callback.</param>
	/// <param name="onGUI">OnGUI callback.</param>
	/// <param name="destroy">OnDestroy callback.</param>
	public static void RemoveService(ServiceMethod update, ServiceMethod onGUI, ServiceMethod destroy)
	{
		if (update != null) updateService -= update;
		if (onGUI != null) onguiService -= onGUI;
		if (destroy != null) destroyService -= destroy;
	}

	public static void InvokeOnUnityThread(ServiceMethod callback)
	{
		lock (Singleton)
		{
			invokeOnUnityThreadCallbacks[invokeSwap].Add(callback);
		}
	}

	static ReignServices()
	{
		#if DISABLE_REIGN
		Debug.LogWarning("NOTE: Reign is disabled for this platform. Check Player Settings for 'DISABLE_REIGN'");
		#endif

		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		invokeOnUnityThreadCallbacks = new List<ServiceMethod>[2];
		invokeOnUnityThreadCallbacks[0] = new List<ServiceMethod>();
		invokeOnUnityThreadCallbacks[1] = new List<ServiceMethod>();
	}
	
	/// <summary>
	/// Used to check if the ReignServices obj exists in the scene.
	/// </summary>
	public static void CheckStatus()
	{
		if (Singleton == null) Debug.LogError("ReignServices Prefab or Script does NOT exist in your scene!");
	}

	void Awake()
	{
		if (Singleton != null)
		{
			canDestroy = false;
			Destroy(gameObject);
			return;
		}

		canDestroy = true;
		Singleton = this;
		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		if (!canDestroy) return;

		PlayerPrefs.Save ();
		if (destroyService != null) destroyService();
		
		Singleton = null;
		updateService = null;
		onguiService = null;
		destroyService = null;
	}

	void Update()
	{
		// invoke update server delegates
		if (updateService != null) updateService();

		// if end of frame wait requested
		if (requestFrameDone)
		{
			requestFrameDone = false;
			StartCoroutine(frameSync());
		}

		// screen size changed callback helper
		int newScreenWidth = Screen.width;
		int newScreenHeight = Screen.height;
		if (newScreenWidth != lastScreenWidth || newScreenHeight != lastScreenHeight)
		{
			if (ScreenSizeChangedCallback != null) ScreenSizeChangedCallback(lastScreenWidth, lastScreenHeight, newScreenWidth, newScreenHeight);
		}
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		// unity thread delegates
		if (invokeOnUnityThreadCallbacks != null && invokeOnUnityThreadCallbacks[invokeSwap].Count != 0)
		{
			invokeSwap = 1 - invokeSwap;
			foreach (var callback in invokeOnUnityThreadCallbacks[1 - invokeSwap])
			{
				callback();
			}

			invokeOnUnityThreadCallbacks[1 - invokeSwap].Clear();
		}
	}

	private IEnumerator frameSync()
	{
		yield return new WaitForEndOfFrame();
		if (frameDoneCallback != null) frameDoneCallback();
	}

	void OnGUI()
	{
		if (onguiService != null) onguiService(); 
	}
}
