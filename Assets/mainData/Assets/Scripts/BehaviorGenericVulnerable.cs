public class BehaviorGenericVulnerable : BehaviorGenericHitRecoil
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		SetIdleAnimation(getAnimationName("recoil_knockdown_idle"));
		SetHitAnimation(getAnimationName("knockdown_idle_recoil"));
		Idle();
	}
}
