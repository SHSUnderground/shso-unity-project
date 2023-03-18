using UnityEngine;

[AddComponentMenu("Particles/Forces/Turbulence")]
[RequireComponent(typeof(ParticleEmitter))]
[ExecuteInEditMode]
public class TurbulenceParticleForce : BaseParticleForce
{
	public Vector3 Offset;

	public float Radius = 1f;

	public float Strength = 1f;

	public float Falloff = 1f;

	public Vector3 TurbulenceAxisScalar = new Vector3(1f, 1f, 1f);

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
		float num3 = Strength * num;
		for (int i = 0; i < particles.Length; i++)
		{
			float magnitude = localToWorldMatrix.MultiplyPoint(particles[i].position).magnitude;
			if (!(magnitude > Radius))
			{
				float num4 = (1f - num2) * num3 + num3 * (num2 * (1f - magnitude / Radius));
				Vector3 vector = new Vector3(Random.Range(-1f, 1f) * TurbulenceAxisScalar.x * num4, Random.Range(-1f, 1f) * TurbulenceAxisScalar.y * num4, Random.Range(-1f, 1f) * TurbulenceAxisScalar.z * num4);
				particles[i].velocity += vector;
			}
		}
	}
}
