using UnityEngine;

[AddComponentMenu("Social Space/Locks/Collision Lock")]
public class CollisionLock : BaseLock
{
	public Collider colliderToLock;

	public override void OnKeyEnabled(KeyObject key)
	{
		SetCollisionEnabled(key, false);
	}

	public override void OnKeyDisabled(KeyObject key)
	{
		SetCollisionEnabled(key, true);
	}

	private void SetCollisionEnabled(KeyObject key, bool enabled)
	{
		if (!(key == null))
		{
			Collider[] components = Utils.GetComponents<Collider>(key.Owner, Utils.SearchChildren);
			foreach (Collider collider in components)
			{
				Physics.IgnoreCollision(collider, colliderToLock, !enabled);
			}
		}
	}
}
