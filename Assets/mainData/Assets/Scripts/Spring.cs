using System;
using UnityEngine;

[Serializable]
public class Spring
{
	public float constant = 100f;

	public float damping = 15f;

	public float mass = 1f;

	public float maxTimeStep = 30f;

	[HideInInspector]
	public Vector3 position;

	[HideInInspector]
	public Vector3 velocity;

	public Spring(float constant, float damping, float mass, float maxTimeStep)
	{
		this.constant = constant;
		this.damping = damping;
		this.mass = mass;
		this.maxTimeStep = 1f / maxTimeStep;
	}

	public void Reset(Vector3 position)
	{
		this.position = position;
		velocity = Vector3.zero;
	}

	public void Update(Vector3 target, float deltaTime)
	{
		float num = deltaTime;
		while (num > 0f)
		{
			float num2 = num;
			if (num2 > maxTimeStep)
			{
				num2 = maxTimeStep;
			}
			num -= num2;
			position += velocity * num2;
			Vector3 a = target - position;
			Vector3 a2 = a * constant - velocity * damping;
			velocity += a2 / mass * num2;
		}
	}
}
