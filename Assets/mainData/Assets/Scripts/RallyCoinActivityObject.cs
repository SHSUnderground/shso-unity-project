using UnityEngine;

public class RallyCoinActivityObject : ActivityObject
{
	public GameObject soundEffectPrefab;

	public int coinsToGive = 1;

	public GameObject[] directionFinders = new GameObject[0];

	private ParticleRenderer[] prenderers;

	private float collisionRadiusSqr = -1f;

	private GameObject player;

	public override void Activate()
	{
		base.Activate();
		InteractiveObject component = Utils.GetComponent<InteractiveObject>(base.gameObject);
		collisionRadiusSqr = component.maxInteractRange * component.maxInteractRange;
		player = GameController.GetController().LocalPlayer;
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
		SpinAndBounce component2 = Utils.GetComponent<SpinAndBounce>(base.gameObject, Utils.SearchChildren);
		if (component2 != null)
		{
			component2.Go();
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
		GameObject g = new GameObject("Coin Awarder");
		TokenAwarder tokenAwarder = Utils.AddComponent<TokenAwarder>(g);
		tokenAwarder.coinsToAward = coinsToGive;
		base.ActionTriggered(action);
	}

	public void Update()
	{
		if (collisionRadiusSqr > 0f && player != null && (player.transform.position - base.gameObject.transform.position).sqrMagnitude <= collisionRadiusSqr)
		{
			collisionRadiusSqr = -1f;
			MagneticActivityObjectTriggerAdapter magneticActivityObjectTriggerAdapter = Utils.AddComponent<MagneticActivityObjectTriggerAdapter>(base.gameObject);
			magneticActivityObjectTriggerAdapter.immobileSubObjects = directionFinders;
			magneticActivityObjectTriggerAdapter.Triggered();
		}
	}
}
