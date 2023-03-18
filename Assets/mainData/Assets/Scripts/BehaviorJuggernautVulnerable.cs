public class BehaviorJuggernautVulnerable : BehaviorGenericHitRecoil
{
	protected string idleAnimName = "recoil_helmet_idle";

	protected string hitAnimName = "recoil_helmet_recoil";

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		SetIdleAnimation(getAnimationName(idleAnimName));
		SetHitAnimation(getAnimationName(hitAnimName));
		if (animationComponent.IsPlaying(hitAnim))
		{
			currentHitRecoilState = HitRecoiilState.Hit;
		}
		else
		{
			Idle();
		}
	}
}
