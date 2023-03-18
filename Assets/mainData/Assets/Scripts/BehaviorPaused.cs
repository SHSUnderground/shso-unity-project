public class BehaviorPaused : BehaviorAnimate
{
	public override void behaviorBegin()
	{
		animation = "movement_idle";
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(owningObject);
		if (component != null)
		{
			component.setDestination(owningObject.transform.position);
		}
		animationComponent.Play(animation);
	}

	public override void behaviorEnd()
	{
	}

	public override void behaviorUpdate()
	{
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}
}
