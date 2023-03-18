using UnityEngine;

public class HqFXReflector : HqReflector
{
	public EffectSequence effectOnForce;

	private EffectSequence effectInstance;

	protected override void ApplyForce(Collision collisionInfo)
	{
		base.ApplyForce(collisionInfo);
		HqObject2 component = Utils.GetComponent<HqObject2>(collisionInfo.gameObject, Utils.SearchChildren);
		if (!(component == null) && component.State == typeof(HqObject2.HqObjectFlinga) && effectOnForce != null)
		{
			PlaySequence();
		}
	}

	private void PlaySequence()
	{
		if (effectInstance != null)
		{
			effectInstance.Cancel();
		}
		effectInstance = (Object.Instantiate(effectOnForce) as EffectSequence);
		effectInstance.Initialize(base.gameObject, null, null);
		effectInstance.StartSequence();
	}
}
