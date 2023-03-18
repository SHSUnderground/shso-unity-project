using UnityEngine;

public class BehaviorBlock : BehaviorRecoil
{
	protected GameObject enemyCharacter;

	protected bool interruptible;

	public void Initialize(GameObject newEnemy, bool isInterruptible)
	{
		enemyCharacter = newEnemy;
		interruptible = isInterruptible;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		string text = "block";
		if (charGlobals.animationComponent[text] == null)
		{
			text = "emote_taunt";
		}
		if (charGlobals.animationComponent[text] != null)
		{
			charGlobals.animationComponent[text].wrapMode = WrapMode.Loop;
			charGlobals.animationComponent.CrossFade(text);
		}
		else
		{
			CspUtils.DebugLog(owningObject.name + " does not have a block or taunt animation");
		}
		charGlobals.effectsList.TryOneShot("block_sequence", owningObject);
		if (!interruptible)
		{
			charGlobals.combatController.launchResistance += 100f;
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		Vector3 lookDir = enemyCharacter.transform.position - owningObject.transform.position;
		charGlobals.motionController.rotateTowards(lookDir);
	}

	public void endBlock()
	{
		charGlobals.behaviorManager.endBehavior();
	}

	public override void behaviorEnd()
	{
		if (!interruptible)
		{
			charGlobals.combatController.launchResistance -= 100f;
		}
		base.behaviorEnd();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		if (interruptible)
		{
			return CombatController.AttackData.RecoilType.None;
		}
		return CombatController.AttackData.RecoilType.Uninterruptible;
	}
}
