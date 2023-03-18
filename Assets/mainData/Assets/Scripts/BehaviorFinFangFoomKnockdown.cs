public class BehaviorFinFangFoomKnockdown : BehaviorGenericHitRecoil
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		SetIdleAnimation(getAnimationName("head_vulnerable_idle"));
		SetHitAnimation(getAnimationName("head_vulnerable_recoil"));
	}
}
