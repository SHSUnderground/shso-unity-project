using System.Collections.Generic;
using UnityEngine;

public class EffectScaler : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected class EmitterData
	{
		public ParticleEmitter emitter;

		public float originalMinSize;

		public float originalMaxSize;

		public Vector3 originalWorldVelocity;
	}

	public float startingScale = 1f;

	public float endingScale = 2f;

	public float duration = 5f;

	protected float currentScale;

	protected List<EmitterData> emitterData;

	protected float startTime;

	private void Start()
	{
		currentScale = startingScale;
		startTime = Time.time;
		this.emitterData = new List<EmitterData>();
		Component[] componentsInChildren = GetComponentsInChildren(typeof(ParticleEmitter));
		if (componentsInChildren.Length > 0)
		{
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				ParticleEmitter particleEmitter = component as ParticleEmitter;
				EmitterData emitterData = new EmitterData();
				emitterData.emitter = particleEmitter;
				emitterData.originalMaxSize = particleEmitter.maxSize;
				emitterData.originalMinSize = particleEmitter.minSize;
				emitterData.originalWorldVelocity = particleEmitter.worldVelocity;
				this.emitterData.Add(emitterData);
			}
		}
		else
		{
			CspUtils.DebugLog("EffectScaler found no emitters to operate on for object " + base.gameObject.name);
		}
	}

	private void Update()
	{
		if (updateScale())
		{
			updateEmitters();
		}
	}

	protected bool updateScale()
	{
		if (currentScale == endingScale)
		{
			return false;
		}
		float num = (Time.time - startTime) / duration;
		if (num > 1f)
		{
			num = 1f;
		}
		currentScale = num * (endingScale - startingScale) + startingScale;
		return true;
	}

	protected void updateEmitters()
	{
		foreach (EmitterData emitterDatum in emitterData)
		{
			emitterDatum.emitter.minSize = emitterDatum.originalMinSize * currentScale;
			emitterDatum.emitter.maxSize = emitterDatum.originalMaxSize * currentScale;
			emitterDatum.emitter.worldVelocity = emitterDatum.originalWorldVelocity * currentScale;
		}
	}
}
