using System.Collections;
using UnityEngine;

public class CustomRolloff : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject sfx;

	public float fullVolumeRadius = 2f;

	public float muteRadius = 10f;

	private AudioListener listener;

	private ShsAudioSource sfxInstance;

	private float originalVolume = 1f;

	private float muteRadiusSq;

	private float fullVolumeRadiusSq;

	private bool muted;

	private bool unloading;

	private void Start()
	{
		muteRadiusSq = muteRadius * muteRadius;
		fullVolumeRadiusSq = fullVolumeRadius * fullVolumeRadius;
	}

	private void OnEnable()
	{
		StartCoroutine(AcquireListener());
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		if (sfxInstance != null && !unloading)
		{
			Object.Destroy(sfxInstance.gameObject);
		}
	}

	private IEnumerator AcquireListener()
	{
		while (listener == null)
		{
			listener = (Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
			if (listener == null)
			{
				yield return new WaitForSeconds(1f);
			}
		}
		StartCoroutine(PollDistance());
	}

	private IEnumerator PollDistance()
	{
		while (listener != null)
		{
			float distanceSqr = (listener.transform.position - base.transform.position).sqrMagnitude;
			if (distanceSqr <= muteRadiusSq)
			{
				SetSFXVolume(distanceSqr);
			}
			else
			{
				MuteSFX();
			}
			yield return 0;
		}
	}

	private void SetSFXVolume(float distanceSqr)
	{
		if (sfxInstance == null && sfx != null)
		{
			GameObject gameObject = Object.Instantiate(sfx) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			sfxInstance = Utils.GetComponent<ShsAudioSource>(gameObject);
			originalVolume = sfxInstance.Volume;
			sfxInstance.Play();
		}
		float num = Mathf.Lerp(0f, originalVolume, (muteRadiusSq - distanceSqr) / (muteRadiusSq - fullVolumeRadiusSq));
		if (sfxInstance != null)
		{
			sfxInstance.Volume = num;
		}
		muted = false;
		SendMessage("SetVolume", num, SendMessageOptions.DontRequireReceiver);
	}

	private void MuteSFX()
	{
		if (sfxInstance != null)
		{
			Object.Destroy(sfxInstance.gameObject);
		}
		if (!muted)
		{
			muted = true;
			SendMessage("SetVolume", 0f, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.3f, 0.1f, 0f, 0.15f);
		Gizmos.DrawSphere(base.transform.position, fullVolumeRadius);
		Gizmos.DrawSphere(base.transform.position, muteRadius);
	}
}
