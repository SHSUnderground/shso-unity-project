using UnityEngine;

public class EffectOnBuildingCollision : EffectOnCollision, IFadeInformable
{
	private bool buildingFaded;

	protected override bool ShouldPlayEffect(GameObject obj)
	{
		if (buildingFaded && GameController.GetController().LocalPlayer == obj)
		{
			return base.ShouldPlayEffect(obj);
		}
		return false;
	}

	public override bool ShouldMaintainEffect(GameObject obj)
	{
		return buildingFaded && base.ShouldMaintainEffect(obj);
	}

	protected override void PlayEffectOnContact(Vector3 point, Vector3 normal, GameObject owner, Collider contactCollider)
	{
		base.PlayEffectOnContact(point, normal, owner, contactCollider);
		SeeThrough component = Camera.main.GetComponent<SeeThrough>();
		if (component != null)
		{
			component.CollisionWithFaded(base.gameObject);
		}
	}

	public void Faded()
	{
		buildingFaded = true;
	}

	public void Restored()
	{
		buildingFaded = false;
	}
}
