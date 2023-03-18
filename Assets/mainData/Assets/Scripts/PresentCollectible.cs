using UnityEngine;

public class PresentCollectible : MagneticTriggerAdapter
{
	public EffectSequenceCollection effects;

	private void OnTriggerEnter(Collider other)
	{
		if (Utils.IsLocalPlayer(other.gameObject))
		{
			Triggered();
		}
	}

	protected override void OnCollected(GameObject player)
	{
		PresentCollectibleEffectTracker instance = PresentCollectibleEffectTracker.GetInstance(player);
		instance.PlayEffects(effects, player);
		base.OnCollected(player);
	}
}
