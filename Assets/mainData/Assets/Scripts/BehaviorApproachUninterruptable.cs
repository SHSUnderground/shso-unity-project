using System;

public class BehaviorApproachUninterruptable : BehaviorApproach
{
	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
