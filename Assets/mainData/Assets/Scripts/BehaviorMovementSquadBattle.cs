using UnityEngine;

public class BehaviorMovementSquadBattle : BehaviorMovement
{
	private const float despawnTime = 0.1f;

	private const float goHomeTime = 0.1f;

	protected SquadBattleCharacterLocator destinationLocator;

	protected float fidgetTime;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		destinationLocator = null;
		scheduleFidget();
		if (charGlobals.squadBattleCharacterAI.homeLocation != Vector3.zero)
		{
			charGlobals.motionController.setDestination(charGlobals.squadBattleCharacterAI.homeLocation, charGlobals.squadBattleCharacterAI.homeFacing);
			charGlobals.squadBattleCharacterAI.homeLocation = Vector3.zero;
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		SquadBattleCharacterLocator homeLocator = charGlobals.squadBattleCharacterAI.homeLocator;
		if (charGlobals.squadBattleCharacterAI.HasAction())
		{
			elapsedTime = 0f;
			destinationLocator = null;
		}
		else if (homeLocator == null)
		{
			if (elapsedTime > 0.1f)
			{
				charGlobals.combatController.playDespawnEffect();
				SquadBattleCharacterController.Instance.RemoveCharacter(owningObject);
			}
		}
		else if (homeLocator != destinationLocator && elapsedTime > 0.1f)
		{
			charGlobals.motionController.setDestination(homeLocator.transform.position, homeLocator.transform.forward);
			destinationLocator = homeLocator;
		}
		else if (fidgetTime > 0f && Time.time > fidgetTime)
		{
			charGlobals.squadBattleCharacterAI.DoFidget();
			scheduleFidget();
		}
	}

	protected void scheduleFidget()
	{
		fidgetTime = Time.time + Random.Range(15f, 30f);
	}

	public override void motionArrived()
	{
		base.motionArrived();
		charGlobals.squadBattleCharacterAI.Arrived();
		if (charGlobals.squadBattleCharacterAI.homeLocator != null && charGlobals.squadBattleCharacterAI.homeLocator == destinationLocator)
		{
			scheduleFidget();
		}
	}

	public override void destinationChanged()
	{
		base.destinationChanged();
		if ((charGlobals.motionController.getDestination() - charGlobals.transform.position).sqrMagnitude < 0.01f)
		{
			scheduleFidget();
		}
		else
		{
			fidgetTime = 0f;
		}
	}
}
