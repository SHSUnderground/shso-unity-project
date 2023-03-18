using System;

public class BehaviorAttackSpecial : BehaviorAttackBase
{
	public override void destinationChanged()
	{
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return true;
		}
		return false;
	}

	public override void behaviorEnd()
	{
		if (charGlobals != null && charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
			charGlobals.motionController.setDestination(charGlobals.transform.position);
		}
		base.behaviorEnd();
	}
}
