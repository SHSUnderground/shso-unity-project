using UnityEngine;

[ExecuteInEditMode]
public abstract class BaseParticleForce : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnEnable()
	{
		ParticleForceConsolidator particleForceConsolidator = base.gameObject.GetComponent<ParticleForceConsolidator>();
		if (particleForceConsolidator == null)
		{
			particleForceConsolidator = base.gameObject.AddComponent<ParticleForceConsolidator>();
		}
		particleForceConsolidator.updateEvent += ParticleUpdate;
	}

	public void OnDisable()
	{
		ParticleForceConsolidator component = base.gameObject.GetComponent<ParticleForceConsolidator>();
		if (component != null)
		{
			component.updateEvent -= ParticleUpdate;
		}
	}

	protected abstract void ParticleUpdate(Particle[] particles);
}
