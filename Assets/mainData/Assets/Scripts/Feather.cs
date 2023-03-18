using UnityEngine;

public class Feather : ActivityObject
{
	public EffectSequence collectionSequence;

	public GameObject face;

	public void SetHeroIcon(Texture heroTexture)
	{
		if (face != null)
		{
			face.renderer.material.mainTexture = heroTexture;
		}
	}

	public void SetHeroIconByPath(string heroTexturePath)
	{
		Texture2D texture;
		if (GUIManager.Instance.LoadTexture(heroTexturePath, out texture))
		{
			SetHeroIcon(texture);
			return;
		}
		GUIManager.Instance.LoadTexture("common_bundle|wip_attractive", out texture);
		SetHeroIcon(texture);
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
