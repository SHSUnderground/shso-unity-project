using System.Collections;
using UnityEngine;

[AddComponentMenu("Particles/Emitter Lifetime")]
public class ParticleLifetime : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float Lifetime = 1f;

	private void Awake()
	{
		StartCoroutine(CleanUp(Lifetime));
	}

	protected IEnumerator CleanUp(float time)
	{
		yield return new WaitForSeconds(time);
		if ((bool)base.particleEmitter)
		{
			base.particleEmitter.emit = false;
		}
	}
}
