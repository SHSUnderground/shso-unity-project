public class BehaviorIdle : BehaviorWait
{
	public override void behaviorBegin()
	{
		charGlobals.animationComponent.CrossFade("movement_idle");
	}

	public override bool useMotionController()
	{
		return false;
	}
}
