using UnityEngine;

public class RallyTicketActivityObject : ActivityObject
{
	public GameObject soundEffectPrefab;

	public int ticketsToGive = 1;

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
		MeshRenderer[] components = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		MeshRenderer[] array2 = components;
		foreach (MeshRenderer meshRenderer in array2)
		{
			meshRenderer.enabled = true;
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
		MeshRenderer[] components = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		MeshRenderer[] array2 = components;
		foreach (MeshRenderer meshRenderer in array2)
		{
			meshRenderer.enabled = false;
		}
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (soundEffectPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(soundEffectPrefab, base.transform.position, Quaternion.identity) as GameObject;
			gameObject.AddComponent<SuicideOnStop>();
		}
		for (int i = 0; i < ticketsToGive; i++)
		{
			NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
		}
		base.ActionTriggered(action);
	}
}
