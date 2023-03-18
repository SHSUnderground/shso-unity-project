using UnityEngine;

[AddComponentMenu("Particles/Auto Deactive")]
public class ParticleDeactivate : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected bool bHasBeenActivated;

	private void Update()
	{
		if (!bHasBeenActivated)
		{
			if (base.particleEmitter.particleCount > 0)
			{
				bHasBeenActivated = true;
			}
		}
		else if (base.particleEmitter.particleCount <= 0)
		{
			bHasBeenActivated = false;
			base.gameObject.active = false;
		}
	}
}
