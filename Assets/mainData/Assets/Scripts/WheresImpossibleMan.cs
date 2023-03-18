using UnityEngine;

public class WheresImpossibleMan : ActivityObject
{
	public GameObject interactiveObject;

	public EffectSequence collectionSequence;

	public ISHSActivity activityReference;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public void SetInteractiveObject(GameObject interactiveObject)
	{
		if (this.interactiveObject != null)
		{
			Utils.DetachGameObject(base.gameObject);
		}
		this.interactiveObject = interactiveObject;
		base.gameObject.transform.localPosition = Vector3.zero;
		Utils.AttachGameObject(interactiveObject, base.gameObject);
		Activate();
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (action == ActivityObjectActionNameEnum.Click || action == ActivityObjectActionNameEnum.PowerEmote)
		{
			PlayCollectionSequence();
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
