using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Audio/Timed Player")]
public class TimedSFXPlayer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected struct AudioInstance
	{
		public ShsAudioSource src;

		public float originalVolume;
	}

	public ShsAudioSource sfx;

	public bool autoDestroySFX = true;

	public FRange initialDelay;

	public FRange timing;

	public bool repeat;

	protected List<AudioInstance> instances = new List<AudioInstance>();

	protected float externalSetVolume = -1f;

	private void OnEnable()
	{
		StartCoroutine(WaitAndPlay());
	}

	private void OnDisable()
	{
		foreach (AudioInstance instance in instances)
		{
			if (instance.src != null)
			{
				Object.Destroy(instance.src.gameObject);
			}
		}
		instances.Clear();
	}

	private IEnumerator WaitAndPlay()
	{
		if (initialDelay.minimum > 0f || initialDelay.maximum > 0f)
		{
			yield return new WaitForSeconds(initialDelay.RandomValue);
			PlayAudio();
		}
		do
		{
			yield return new WaitForSeconds(timing.RandomValue);
			PlayAudio();
		}
		while (repeat);
	}

	private void PlayAudio()
	{
		if (sfx != null && (externalSetVolume > 0f || externalSetVolume == -1f))
		{
			ShsAudioSource shsAudioSource = (!autoDestroySFX) ? ShsAudioSource.PlayFromPrefab(sfx.gameObject, base.transform) : ShsAudioSource.PlayAutoSound(sfx.gameObject, base.transform);
			shsAudioSource.transform.parent = base.transform;
			if (repeat)
			{
				shsAudioSource.Loop = false;
			}
			AudioInstance item = default(AudioInstance);
			item.src = shsAudioSource;
			item.originalVolume = shsAudioSource.Volume;
			instances.Add(item);
			if (externalSetVolume != -1f)
			{
				shsAudioSource.Volume *= externalSetVolume;
			}
		}
	}

	private void SetVolume(float volume)
	{
		externalSetVolume = volume;
		instances.RemoveAll(delegate(AudioInstance instance)
		{
			return instance.src == null;
		});
		foreach (AudioInstance instance in instances)
		{
			instance.src.Volume = instance.originalVolume * volume;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawIcon(base.transform.position, "SFX.png");
	}
}
