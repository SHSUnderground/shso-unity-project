public class BehaviorAttackOnce : BehaviorAttackBase
{
	public override void behaviorFirstUpdate()
	{
		base.behaviorFirstUpdate();
		canChain = false;
		continueAttackingTarget = false;
	}
}
