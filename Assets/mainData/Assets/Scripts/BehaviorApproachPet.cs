public class BehaviorApproachPet : BehaviorApproach
{
	private string _customRunAnim = "movement_run";

	private string _customIdleAnim = "movement_idle";

	public void setPetAnimMode(float speed, string customRunAnim = "movement_run", string customIdleAnim = "movement_idle")
	{
		if (customRunAnim != string.Empty)
		{
			_customRunAnim = customRunAnim;
		}
		if (customIdleAnim != string.Empty)
		{
			_customIdleAnim = customIdleAnim;
		}
		runAnimName = _customRunAnim;
		idleAnimName = _customIdleAnim;
		behaviorBegin();
	}

	public override void configureMovementBehaviors()
	{
		if (charGlobals.behaviorManager.runAnimOverride != string.Empty)
		{
			runAnimName = charGlobals.behaviorManager.runAnimOverride;
		}
		else
		{
			runAnimName = "movement_run";
		}
		if (charGlobals.behaviorManager.idleAnimOverride != string.Empty)
		{
			idleAnimName = charGlobals.behaviorManager.idleAnimOverride;
		}
		else
		{
			idleAnimName = "movement_idle";
		}
	}
}
