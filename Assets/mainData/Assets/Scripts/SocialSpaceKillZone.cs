using UnityEngine;

public class SocialSpaceKillZone : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void OnCollisionStay(Collision otherCollision)
	{
		if (SocialSpaceController.Instance != null)
		{
			RespawnCollider(otherCollision.collider);
		}
	}

	private void OnTriggerStay(Collider otherCollider)
	{
		if (SocialSpaceController.Instance != null)
		{
			RespawnCollider(otherCollider);
		}
	}

	protected void RespawnCollider(Collider other)
	{
		PlayerInputController component = Utils.GetComponent<PlayerInputController>(other.gameObject, Utils.SearchParents);
		if (!(component == null))
		{
			CharacterMotionController component2 = Utils.GetComponent<CharacterMotionController>(component.gameObject);
			if (!(component2 == null))
			{
				CspUtils.DebugLogError("Respawning player <" + other.gameObject.name + ">");
				Vector3 respawnPoint = SocialSpaceController.Instance.GetRespawnPoint();
				component2.teleportTo(respawnPoint);
				component2.setDestination(respawnPoint);
			}
		}
	}
}
