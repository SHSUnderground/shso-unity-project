using UnityEngine;

public class EffectOnCollision : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence effect;

	private void OnCharacterCollided(ControllerColliderHit collision)
	{
		if (ShouldPlayEffect(collision.controller.gameObject))
		{
			PlayEffectOnContact(collision.point, (-1f * collision.normal).normalized, collision.controller.gameObject, collision.collider);
		}
	}

	protected virtual bool ShouldPlayEffect(GameObject obj)
	{
		return Utils.GetComponent<EffectOnCollisionTracker>(obj) == null;
	}

	public virtual bool ShouldMaintainEffect(GameObject obj)
	{
		return true;
	}

	protected virtual void PlayEffectOnContact(Vector3 point, Vector3 normal, GameObject owner, Collider contactCollider)
	{
		GameObject gameObject = Object.Instantiate(effect.gameObject) as GameObject;
		if (gameObject == null)
		{
			CspUtils.DebugLog("Could not play effect on contact; effect was not valid");
			return;
		}
		EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject);
		if (component == null)
		{
			CspUtils.DebugLog("Could not play effect on contact; effect was not valid");
			return;
		}
		component.Initialize(base.gameObject, null, null);
		gameObject.transform.position = point;
		gameObject.transform.LookAt(point + normal);
		component.StartSequence();
		EffectOnCollisionTracker effectOnCollisionTracker = Utils.AddComponent<EffectOnCollisionTracker>(owner);
		effectOnCollisionTracker.attachedSequence = component;
		effectOnCollisionTracker.collisionNormal = -1f * normal;
		effectOnCollisionTracker.contactCollider = contactCollider;
		effectOnCollisionTracker.source = this;
	}
}
