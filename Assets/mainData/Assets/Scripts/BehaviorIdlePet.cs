public class BehaviorIdlePet : BehaviorIdle
{
	private string _customIdleAnim = "movement_idle";

	public void setAnimName(string customIdleAnim = "movement_idle")
	{
		if (customIdleAnim != string.Empty)
		{
			_customIdleAnim = customIdleAnim;
		}
		behaviorBegin();
	}

	public override void behaviorBegin()
	{
		if (charGlobals.behaviorManager.idleAnimOverride != string.Empty)
		{
			charGlobals.animationComponent.CrossFade(charGlobals.behaviorManager.idleAnimOverride);
		}
		else
		{
			charGlobals.animationComponent.CrossFade(_customIdleAnim);
		}
	}
}
