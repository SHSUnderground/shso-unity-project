using System.Collections;
using UnityEngine;

[RequireComponent(typeof(IEmotor))]
[AddComponentMenu("Lab/Emotor/Flasher")]
public class EmotorFlasher : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Color FlashColor = Color.white;

	private bool _busy;

	private void Start()
	{
		(GetComponent(typeof(IEmotor)) as IEmotor).OnEmoteStart += EmotorFlasher_OnEmoteStart;
	}

	private void EmotorFlasher_OnEmoteStart(EmotesDefinition.EmoteDefinition emoteDef)
	{
		if (base.enabled && !_busy)
		{
			StartCoroutine(Flash());
		}
	}

	private IEnumerator Flash()
	{
		_busy = true;
		try
		{
			Camera oldCamera = Camera.main;
			if (oldCamera != null)
			{
				oldCamera.enabled = false;
			}
			Camera flashCamera = Utils.AddComponent<Camera>(new GameObject("CameraFlasher"));
			flashCamera.transform.parent = base.transform;
			flashCamera.cullingMask = 0;
			flashCamera.backgroundColor = FlashColor;
			flashCamera.clearFlags = CameraClearFlags.Color;
			yield return new WaitForEndOfFrame();
			if (oldCamera != null)
			{
				oldCamera.enabled = true;
			}
			flashCamera.enabled = false;
			yield return new WaitForEndOfFrame();
			Object.Destroy(flashCamera.gameObject);
		}
		finally
		{
			_busy = false;
		}
	}
}
