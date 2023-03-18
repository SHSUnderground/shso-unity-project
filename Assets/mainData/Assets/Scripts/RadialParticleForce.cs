using UnityEngine;

[AddComponentMenu("Particles/Forces/Radial")]
[ExecuteInEditMode]
[RequireComponent(typeof(ParticleEmitter))]
public class RadialParticleForce : BaseParticleForce
{
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
		Matrix4x4 lhs = default(Matrix4x4);
		lhs.SetTRS(Offset * -1f, Quaternion.identity, new Vector3(1f, 1f, 1f));
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		localToWorldMatrix = lhs * localToWorldMatrix.inverse;
		float num2 = Falloff;
		if (num2 > 1f)
		{
			num2 = 1f;
		}
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		for (int i = 0; i < particles.Length; i++)
		{
			Vector3 vector = localToWorldMatrix.MultiplyPoint(particles[i].position);
			float magnitude = vector.magnitude;
			if (!(magnitude > Radius))
			{
				float num3 = Strength * (1f - magnitude / Radius);
				float d = Strength * (1f - num2) + num3 * num2;
				Vector3 vector2 = vector.normalized * num * d;
				particles[i].velocity += vector2;
			}
		}
	}
}
