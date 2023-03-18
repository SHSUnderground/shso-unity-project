public class BehaviorAttackCharge : BehaviorAttackBase
{
	public override void motionCollided()
	{
		if (networkComponent == null || networkComponent.IsOwner())
		{
			if (networkComponent != null)
			{
				networkComponent.QueueNetAction(new NetActionChargeCollision(owningObject));
			}
			base.motionCollided();
		}
	}

	public void OnNetMotionCollided()
	{
		base.motionCollided();
	}
}
