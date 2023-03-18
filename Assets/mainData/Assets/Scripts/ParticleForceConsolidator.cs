using UnityEngine;

[RequireComponent(typeof(ParticleEmitter))]
[ExecuteInEditMode]
internal class ParticleForceConsolidator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void UpdateParticlesDelegate(Particle[] particles);

	public event UpdateParticlesDelegate updateEvent;

	public void OnDisable()
	{
		this.updateEvent = null;
	}

	public void Update()
	{
		if (this.updateEvent != null)
		{
			Particle[] particles = base.particleEmitter.particles;
			this.updateEvent(particles);
			base.particleEmitter.particles = particles;
		}
	}
}
