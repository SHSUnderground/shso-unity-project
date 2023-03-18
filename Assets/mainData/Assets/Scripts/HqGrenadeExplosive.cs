using UnityEngine;

[AddComponentMenu("Hq/Explosives/Grenade Explosive")]
public class HqGrenadeExplosive : HqExplosive
{
	public float MinimumVelocity;

	protected float peakVelocity;

	public override void Update()
	{
		if (base.IsEnabled && hqObj != null)
		{
			if (hqObj.rigidbody.velocity.sqrMagnitude > peakVelocity)
			{
				peakVelocity = hqObj.rigidbody.velocity.sqrMagnitude;
			}
			if (peakVelocity >= MinimumVelocity * MinimumVelocity && hqObj.rigidbody.velocity.sqrMagnitude <= 0.1f)
			{
				Explode();
			}
		}
		base.Update();
	}
}
