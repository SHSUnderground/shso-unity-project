public class BehaviorFinFangFoomVulnerable : BehaviorFinFangFoomKnockdown
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
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
