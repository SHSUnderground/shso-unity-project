using System;
using UnityEngine;

[AddComponentMenu("Particles/Forces/Vortex")]
[RequireComponent(typeof(ParticleEmitter))]
[ExecuteInEditMode]
public class VortexParticleForce : BaseParticleForce
{
	public Vector3 Axis;

	public Vector3 Offset;

	public float Radius = 1f;

	public float Strength = 1f;

	public float Falloff = 1f;

	protected override void ParticleUpdate(Particle[] particles)
	{
		float num = Time.deltaTime;
		if (num < 0.0166666675f)
		{
			num = 0.0166666675f;
		}
		float num2 = (float)Math.PI * 2f * num * Strength;
		Matrix4x4 inverse = base.transform.localToWorldMatrix.inverse;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x = Matrix4x4.identity;
		matrix4x.SetTRS(new Vector3(0f, 0f, 0f), Quaternion.FromToRotation(Axis, new Vector3(0f, 1f, 0f)), new Vector3(1f, 1f, 1f));
		Matrix4x4 rhs = default(Matrix4x4);
		rhs.SetTRS(Offset * -1f, Quaternion.identity, new Vector3(1f, 1f, 1f));
		Matrix4x4 matrix4x2 = matrix4x * rhs * inverse;
		Matrix4x4 inverse2 = matrix4x2.inverse;
		float num3 = Falloff;
		if (num3 > 1f)
		{
			num3 = 1f;
		}
		if (num3 < 0f)
		{
			num3 = 0f;
		}
		Vector3 v = default(Vector3);
		for (int i = 0; i < particles.Length; i++)
		{
			Particle particle = particles[i];
			Vector3 vector = matrix4x2.MultiplyPoint(particle.position);
			float magnitude = vector.magnitude;
			if (!(magnitude > Radius))
			{
				magnitude = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
				float num4 = num2 * (1f - magnitude / Radius);
				float f = num2 * (1f - num3) + num4 * num3;
				v.x = vector.x * Mathf.Cos(f) - vector.z * Mathf.Sin(f);
				v.y = vector.y;
				v.z = vector.x * Mathf.Sin(f) + vector.z * Mathf.Cos(f);
				particles[i].position = inverse2.MultiplyPoint(v);
			}
		}
	}
}
