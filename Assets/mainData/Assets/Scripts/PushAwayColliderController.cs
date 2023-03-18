using System.Collections.Generic;
using UnityEngine;

public class PushAwayColliderController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float velocity = 6f;

	protected CharacterGlobals charGlobals;

	protected List<CharacterGlobals> ignoredCharacters = new List<CharacterGlobals>();

	public void IgnoreCharacter(CharacterGlobals character, bool ignored)
	{
		if (ignored)
		{
			if (!ignoredCharacters.Contains(character))
			{
				ignoredCharacters.Add(character);
			}
		}
		else
		{
			ignoredCharacters.Remove(character);
		}
	}

	private void Start()
	{
		charGlobals = (base.gameObject.transform.root.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
	}

	protected void OnTriggerEnter(Collider other)
	{
	}

	protected void OnTriggerExit(Collider other)
	{
	}

	protected void OnTriggerStay(Collider other)
	{
		if (!charGlobals || (charGlobals.combatController != null && (charGlobals.combatController.isKilled || charGlobals.combatController.isHidden)) || charGlobals.motionController.IsForcedVelocity())
		{
			return;
		}
		if (other.isTrigger)
		{
			if (other.gameObject.active)
			{
				Physics.IgnoreCollision(base.collider, other.collider);
			}
			return;
		}
		if (other.transform == base.gameObject.transform.root)
		{
			Physics.IgnoreCollision(base.collider, other.collider);
			return;
		}
		CharacterGlobals characterGlobals = other.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (!(characterGlobals != null) || !(characterGlobals.motionController != null) || !(characterGlobals.combatController != null) || characterGlobals.combatController.isKilled || (characterGlobals.networkComponent != null && !characterGlobals.networkComponent.IsOwner()))
		{
			return;
		}
		if (characterGlobals.motionController.IsOnGround())
		{
			Vector3 position = characterGlobals.transform.position;
			float y = position.y;
			Vector3 position2 = base.transform.root.position;
			if (Mathf.Abs(y - position2.y) < 0.5f)
			{
				return;
			}
		}
		if (!characterGlobals.combatController.isHidden && !characterGlobals.motionController.IsForcedVelocity() && !ignoredCharacters.Contains(characterGlobals))
		{
			Vector3 newVelocity = other.transform.position - base.gameObject.transform.position;
			newVelocity.y = 0f;
			newVelocity.Normalize();
			newVelocity *= velocity;
			characterGlobals.motionController.setForcedVelocity(newVelocity, 0.3f);
			characterGlobals.motionController.setVerticalVelocity(5f);
		}
	}
}
