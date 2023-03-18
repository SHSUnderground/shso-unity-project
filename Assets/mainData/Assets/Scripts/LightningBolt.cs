using System;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Transform target;

	public int zigs = 100;

	public float speed = 1f;

	public float scale = 1f;

	public Light startLight;

	public Light endLight;

	private Perlin noise;

	private float oneOverZigs;

	private Particle[] particles;

	private void Start()
	{
		oneOverZigs = 1f / (float)zigs;
		base.particleEmitter.emit = false;
		base.particleEmitter.Emit(zigs);
		particles = base.particleEmitter.particles;
	}

	private void Update()
	{
		if (noise == null)
		{
			noise = new Perlin();
		}
		float num = Time.time * speed * (154f / (415f * (float)Math.E));
		float num2 = Time.time * speed * 1.21688f;
		float num3 = Time.time * speed * 2.5564f;
		for (int i = 0; i < particles.Length; i++)
		{
			Vector3 position = Vector3.Lerp(base.transform.position, target.position, oneOverZigs * (float)i);
			Vector3 a = new Vector3(noise.Noise(num + position.x, num + position.y, num + position.z), noise.Noise(num2 + position.x, num2 + position.y, num2 + position.z), noise.Noise(num3 + position.x, num3 + position.y, num3 + position.z));
			position += a * scale * ((float)i * oneOverZigs);
			particles[i].position = position;
			particles[i].color = Color.white;
			particles[i].energy = 1f;
		}
		base.particleEmitter.particles = particles;
		if (base.particleEmitter.particleCount >= 2)
		{
			if ((bool)startLight)
			{
				startLight.transform.position = particles[0].position;
			}
			if ((bool)endLight)
			{
				endLight.transform.position = particles[particles.Length - 1].position;
			}
		}
	}
}
