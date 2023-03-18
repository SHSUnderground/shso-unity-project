using System;
using UnityEngine;

public class BehaviorGenericHitRecoil : BehaviorBase
{
	protected enum HitRecoiilState
	{
		Idle,
		Hit
	}

	protected HitRecoiilState currentHitRecoilState;

	protected string idleAnim;

	protected string hitAnim;

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (currentHitRecoilState == HitRecoiilState.Hit && !animationComponent.IsPlaying(hitAnim))
		{
			Idle();
		}
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil))
		{
			return true;
		}
		return false;
	}

	public override void HitByEnemy(CombatController enemy)
	{
		base.HitByEnemy(enemy);
		Hit();
	}

	public void SetIdleAnimation(string animation)
	{
		if (animationComponent[animation] != null)
		{
			idleAnim = animation;
			animationComponent[idleAnim].wrapMode = WrapMode.Loop;
		}
	}

	public void SetHitAnimation(string animation)
	{
		if (animationComponent[animation] != null)
		{
			hitAnim = animation;
			animationComponent[hitAnim].wrapMode = WrapMode.Once;
		}
	}

	public void Idle()
	{
		if (animationComponent[idleAnim] != null)
		{
			currentHitRecoilState = HitRecoiilState.Idle;
			animationComponent.Play(idleAnim);
		}
		else
		{
			CspUtils.DebugLog("idle animation <" + idleAnim + "> not found for <" + owningObject.name + "> while in behavior <" + GetType() + ">");
		}
	}

	public void Hit()
	{
		if (animationComponent[hitAnim] != null)
		{
			currentHitRecoilState = HitRecoiilState.Hit;
			animationComponent.Rewind(hitAnim);
			animationComponent.Play(hitAnim);
		}
		else
		{
			CspUtils.DebugLog("hit animation <" + hitAnim + "> not found for <" + owningObject.name + "> while in behavior <" + GetType() + ">");
		}
	}
}
