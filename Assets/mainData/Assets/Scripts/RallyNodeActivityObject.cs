using UnityEngine;

public class RallyNodeActivityObject : ActivityObject
{
	public GameObject soundEffectPrefab;

	private ParticleRenderer[] prenderers;

	public override void Activate()
	{
		base.Activate();
		prenderers = Utils.GetComponents<ParticleRenderer>(base.gameObject, Utils.SearchChildren);
		ParticleRenderer[] array = prenderers;
		foreach (ParticleRenderer particleRenderer in array)
		{
			if (particleRenderer.gameObject.name.Contains("inactive"))
			{
				particleRenderer.enabled = false;
			}
			else
			{
				particleRenderer.enabled = true;
			}
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		prenderers = Utils.GetComponents<ParticleRenderer>(base.gameObject, Utils.SearchChildren);
		ParticleRenderer[] array = prenderers;
		foreach (ParticleRenderer particleRenderer in array)
		{
			if (particleRenderer.gameObject.name.Contains("inactive"))
			{
				particleRenderer.enabled = true;
			}
			else
			{
				particleRenderer.enabled = false;
			}
		}
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (isActivated)
		{
			if (soundEffectPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(soundEffectPrefab, base.transform.position, Quaternion.identity) as GameObject;
				gameObject.AddComponent<SuicideOnStop>();
			}
			base.ActionTriggered(action);
		}
	}
}
