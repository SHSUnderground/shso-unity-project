using UnityEngine;

public class BehaviorAttackApproach : BehaviorMovement
{
	public bool secondaryAttack;

	public string attackName;

	public CharacterGlobals targetCharGlobals;

	private float combinedRadius = 1f;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public void Initialize(bool newSecondaryAttack)
	{
		Initialize(newSecondaryAttack, null);
	}

	public void Initialize(bool newSecondaryAttack, string newAttackName)
	{
		secondaryAttack = newSecondaryAttack;
		attackName = newAttackName;
		targetCharGlobals = (targetObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		if (charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
			if (targetCharGlobals != null)
			{
				if (targetObject != GUIManager.Instance.GetTargetedEnemy())
				{
					GUIManager.Instance.DetachAttackingIndicator();
				}
				GUIManager.Instance.AttachAttackingEnemyIndicator(targetObject);
			}
			GUIManager.Instance.AttachHealthBarIndicator(targetObject);
		}
		if (charGlobals != null && targetCharGlobals != null && charGlobals.characterController != null && targetCharGlobals.characterController != null)
		{
			combinedRadius = charGlobals.characterController.radius + targetCharGlobals.characterController.radius;
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!targetObject)
		{
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		if (charGlobals.combatController.beginAttack(targetObject, secondaryAttack, false, attackName))
		{
			charGlobals.motionController.setDestination(owningObject.transform.position);
			return;
		}
		Vector3 b = owningObject.transform.position - targetObject.transform.position;
		b.y = 0f;
		b.Normalize();
		b *= combinedRadius;
		charGlobals.motionController.setDestination(targetObject.transform.position + b);
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (charGlobals.spawnData != null && charGlobals.spawnData.spawnType != CharacterSpawn.Type.LocalPlayer)
		{
		}
	}

	public override bool behaviorEndOnCutScene()
	{
		return true;
	}

	public override void destinationChanged()
	{
		charGlobals.behaviorManager.endBehavior();
	}
}
