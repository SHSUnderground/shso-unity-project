using System.Collections;
using UnityEngine;

public class EmissionLifetime : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float lifetime = -1f;

	private float startTime;

	private bool wasDisabled;

	public void Start()
	{
		if (lifetime >= 0f)
		{
			startTime = Time.time;
			StartCoroutine(Wait());
		}
	}

	private void OnEnable()
	{
		if (wasDisabled)
		{
			wasDisabled = false;
			Start();
		}
	}

	private void OnDisable()
	{
		wasDisabled = true;
		float num = Time.time - startTime;
		if (num < lifetime)
		{
			lifetime -= num;
		}
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(lifetime);
		Finish();
	}

	private void Finish()
	{
		ParticleEmitter[] componentsInChildren = GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
		}
	}
}
