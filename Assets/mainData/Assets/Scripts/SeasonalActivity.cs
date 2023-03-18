using UnityEngine;

public class SeasonalActivity : ActivityObject
{
	public EffectSequence collectionSequence;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GameController.GetController().LocalPlayer)
		{
			base.gameObject.BroadcastMessage("Triggered", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		base.ActionTriggered(action);
		if (action == ActivityObjectActionNameEnum.Collision)
		{
			PlayCollectionSequence();
			Despawn();
		}
	}

	private void PlayCollectionSequence()
	{
		if (collectionSequence != null)
		{
			GameObject gameObject = Object.Instantiate(collectionSequence.gameObject) as GameObject;
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = base.transform.rotation;
			gameObject.GetComponent<EffectSequence>().Initialize(null, DestroyCollectionSequence, null);
		}
	}

	private void DestroyCollectionSequence(EffectSequence instance)
	{
		Object.Destroy(instance.gameObject);
	}
}
