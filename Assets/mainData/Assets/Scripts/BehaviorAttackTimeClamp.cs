public class BehaviorAttackTimeClamp : BehaviorAttackBase
{
	protected float minAnimLength;

	protected float maxAnimLength = 10f;

	protected float oldAnimSpeed = -1f;

	public override void behaviorFirstUpdate()
	{
		base.behaviorFirstUpdate();
		minAnimLength = ActionTimesDefinition.Instance.ThrowMin;
		maxAnimLength = ActionTimesDefinition.Instance.ThrowMax;
		oldAnimSpeed = clampAnimationSpeed(baseAnim, minAnimLength, maxAnimLength);
	}

	public override void behaviorEnd()
	{
		if (oldAnimSpeed != -1f)
		{
			restoreAnimationSpeed(baseAnim, oldAnimSpeed);
		}
		base.behaviorEnd();
	}
}
