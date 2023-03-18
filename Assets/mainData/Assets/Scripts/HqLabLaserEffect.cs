using UnityEngine;

public class HqLabLaserEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected float startTime;

	protected float destroyTime;

	protected float duration = 10f;

	protected GameObject particlePrefabInstance;

	public float Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}

	public GameObject ParticlePrefabInstance
	{
		get
		{
			return particlePrefabInstance;
		}
	}

	public void AttachParticlePrefab(GameObject particlePrefab)
	{
		if (particlePrefabInstance == null)
		{
			particlePrefabInstance = (Object.Instantiate(particlePrefab, base.gameObject.transform.position, Quaternion.identity) as GameObject);
			particlePrefabInstance.transform.parent = base.gameObject.transform;
		}
	}

	public void StartEffect()
	{
		startTime = Time.time;
	}

	public void OnDisable()
	{
		if (particlePrefabInstance != null)
		{
			particlePrefabInstance.particleEmitter.emit = false;
			particlePrefabInstance.particleEmitter.ClearParticles();
		}
	}

	public void OnEnable()
	{
		if (particlePrefabInstance != null && !particlePrefabInstance.particleEmitter.emit)
		{
			particlePrefabInstance.particleEmitter.emit = true;
		}
	}

	public void Update()
	{
		float num = Time.time - startTime;
		if (particlePrefabInstance != null && particlePrefabInstance.particleEmitter != null)
		{
			if (num > duration + particlePrefabInstance.particleEmitter.maxEnergy)
			{
				CspUtils.DebugLog("Destroying lab effect.");
				Object.Destroy(particlePrefabInstance);
				Object.Destroy(this);
			}
			else if (num > duration)
			{
				particlePrefabInstance.particleEmitter.emit = false;
			}
		}
	}
}
