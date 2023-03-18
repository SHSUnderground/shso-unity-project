using UnityEngine;

public class BehaviorAttackAbomination : BehaviorAttackBase
{
	private static float stunTimeBase = 2f;

	private static float stunTimeModifier = 2f;

	public override void motionCollided()
	{
		if (elapsedTime < 0.75f)
		{
			return;
		}
		BehaviorRecoilStun behaviorRecoilStun = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilStun)) as BehaviorRecoilStun;
		if (behaviorRecoilStun != null)
		{
			GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName("Abom_recoil_sequence") as GameObject;
			if (gameObject != null)
			{
				GameObject child = Object.Instantiate(gameObject) as GameObject;
				Utils.AttachGameObject(owningObject, child);
			}
			behaviorRecoilStun.InitializeWithIntro(stunTimeBase * stunTimeModifier, "recoil_charge");
			behaviorRecoilStun.disallowInterrupt = true;
		}
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (forciblyEnded)
		{
			return;
		}
		BehaviorRecoilStun behaviorRecoilStun = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilStun)) as BehaviorRecoilStun;
		if (behaviorRecoilStun != null)
		{
			GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName("Abom_skid_sequence") as GameObject;
			if (gameObject != null)
			{
				GameObject child = Object.Instantiate(gameObject) as GameObject;
				Utils.AttachGameObject(owningObject, child);
			}
			behaviorRecoilStun.InitializeWithIntro(stunTimeBase * stunTimeModifier, "charge_skid");
			behaviorRecoilStun.disallowInterrupt = true;
		}
	}
}
