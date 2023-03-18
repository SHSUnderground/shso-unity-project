using UnityEngine;

public interface IAnimTagListener
{
	void OnMoveStartAnimTag(AnimationEvent evt);

	void OnMoveEndAnimTag(AnimationEvent evt);

	void OnChainAttackAnimTag(AnimationEvent evt);

	void OnCollisionEnableAnimTag(AnimationEvent evt);

	void OnCollisionDisableAnimTag(AnimationEvent evt);

	void OnProjectileFireAnimTag(AnimationEvent evt);

	void OnPinballStartAnimTag(AnimationEvent evt);

	void OnPinballEndAnimTag(AnimationEvent evt);

	void OnMultishotInfoAnimTag(AnimationEvent evt);

	void OnTriggerEffectAnimTag(AnimationEvent evt);
}
