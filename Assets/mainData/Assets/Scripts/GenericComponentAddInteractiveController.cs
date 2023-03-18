using System;
using UnityEngine;

public class GenericComponentAddInteractiveController : OperateDeviceInteractiveObjectController
{
	public EffectSequence effectSequence;

	public GameObject parentObject;

	public float duration = 30f;

	public ShsAudioSource soundEffect;

	public string componentToAdd;

	protected Type typeToAdd;

	public void Start()
	{
		typeToAdd = Type.GetType(componentToAdd);
		if (typeToAdd == null)
		{
			CspUtils.DebugLog("Unable to find type <" + componentToAdd + ">");
		}
	}

	public override bool CanPlayerUse(GameObject player)
	{
		Component component = player.GetComponent(typeToAdd);
		return component == null;
	}

	protected override void ApproachArrived(GameObject obj)
	{
		base.ApproachArrived(obj);
		if (this.effectSequence != null && parentObject != null)
		{
			EffectSequence effectSequence = UnityEngine.Object.Instantiate(this.effectSequence) as EffectSequence;
			effectSequence.Initialize(parentObject, null, null);
			effectSequence.StartSequence();
		}
		if (soundEffect != null)
		{
			ShsAudioSource.PlayAutoSound(soundEffect.gameObject, obj.transform);
		}
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
		}
		else
		{
			behaviorManager.requestChangeBehavior<BehaviorWait>(false);
		}
	}

	protected override void AnimationComplete(GameObject obj)
	{
		base.AnimationComplete(obj);
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager != null)
		{
			behaviorManager.endBehavior();
		}
		Component component = obj.GetComponent(typeToAdd);
		if (component == null)
		{
			component = obj.AddComponent(typeToAdd);
		}
	}
}
