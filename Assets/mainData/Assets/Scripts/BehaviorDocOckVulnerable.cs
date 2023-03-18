using System;
using UnityEngine;

public class BehaviorDocOckVulnerable : BehaviorBase
{
	public override void behaviorBegin()
	{
		base.behaviorBegin();
		animationComponent["recoil_knockdown_idle"].wrapMode = WrapMode.Loop;
		animationComponent.Play("recoil_knockdown_idle");
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return true;
		}
		return false;
	}
}
