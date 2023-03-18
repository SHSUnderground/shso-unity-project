using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Particles/Forces/Drag")]
[RequireComponent(typeof(ParticleEmitter))]
public class DragParticleForce : BaseParticleForce
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
			float magnitude = localToWorldMatrix.MultiplyPoint(particles[i].position).magnitude;
			if (!(magnitude > Radius))
			{
				float num3 = 1f - Strength * num;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				particles[i].velocity = particles[i].velocity * num3;
			}
		}
	}
}
