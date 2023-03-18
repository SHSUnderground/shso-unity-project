using System.Collections;
using UnityEngine;

public class SuicideOnStop : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float TIMEOUT = 5f;

	public float killDelay;

	private ShsAudioSource sound;

	private void Start()
	{
		sound = Utils.GetComponent<ShsAudioSource>(base.gameObject);
		StartCoroutine(WaitToKill());
	}

	private IEnumerator WaitToKill()
	{
		float startTime = Time.time;
		if (sound != null)
		{
			while (!sound.IsPlaying && Time.time - startTime < 5f)
			{
				yield return 0;
			}
			while (sound.IsPlaying)
			{
				yield return 0;
			}
			float killTime = Time.time + killDelay;
			while (Time.time < killTime)
			{
				yield return 0;
			}
			yield return 0;
			Object.Destroy(base.gameObject);
		}
	}
}
