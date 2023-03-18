public class BehaviorAttackLeap : BehaviorAttackBase
{
	public void SetupLeapAttack()
	{
		float num = attackData.moveSpeed * charGlobals.motionController.speed;
		if (attackData.forwardSpeed != 0f)
		{
			num = attackData.forwardSpeed;
		}
		float magnitude = (combatController.transform.position - targetCombatController.transform.position).magnitude;
		attackData.attackDuration = magnitude / num;
		float verticalVelocity = charGlobals.motionController.gravity * attackData.attackDuration * 0.5f;
		charGlobals.motionController.setVerticalVelocity(verticalVelocity);
	}

	public override void behaviorFirstUpdate()
	{
		base.behaviorFirstUpdate();
		if (!startAnim)
		{
			SetupLeapAttack();
		}
	}

	protected override bool checkEndAttack()
	{
		if (startAnim && !animationComponent.IsPlaying(animName))
		{
			SetupLeapAttack();
		}
		return base.checkEndAttack();
	}
}
