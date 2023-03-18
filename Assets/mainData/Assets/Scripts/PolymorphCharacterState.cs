using System;
using UnityEngine;

public class PolymorphCharacterState : PolymorphBaseState
{
	public PolymorphCharacterState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		if (base.stateData.PolymorphObject == null)
		{
			PolymorphController.LogPolymorphError(base.stateData.OriginalObject, base.stateData.Original, "POLYMORPH - no polymorph object");
			return;
		}
		if (!string.IsNullOrEmpty(base.stateData.PolymorphEffect))
		{
			GameObject gameObject = EffectSequence.FindEffect(base.stateData.PolymorphEffect, base.stateData.OriginalObject);
			TintObject tintObject = null;
			if (gameObject != null)
			{
				tintObject = gameObject.GetComponent<TintObject>();
			}
			if (tintObject != null)
			{
				tintObject.UnTint();
			}
			EffectSequence.TransferEffect(base.stateData.PolymorphEffect, base.stateData.OriginalObject, base.stateData.PolymorphObject);
			if (tintObject != null)
			{
				tintObject.StartFade(TintObject.TintFadeType.TintFadeOut);
			}
		}
		BehaviorPolymorph behaviorPolymorph = base.stateData.Original.behaviorManager.forceChangeBehavior(typeof(BehaviorPolymorph)) as BehaviorPolymorph;
		if (behaviorPolymorph != null)
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "POLYMORPH LOCAL " + base.stateData.Original.networkComponent.IsOwner());
			behaviorPolymorph.Initialize(base.stateData.PolymorphObject, ((PolymorphCharacterStateData)base.stateData).CombatTarget);
		}
		else
		{
			PolymorphController.LogPolymorphError(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "POLYMORPH - failed to start behavior");
		}
		if (GUIManager.Instance != null && GUIManager.Instance.GetTargetedEnemy() == base.stateData.OriginalObject)
		{
			GUIManager.Instance.DetachAttackingIndicator();
		}
		if (base.stateData.Original.networkComponent.IsOwner())
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "POLYMORPH NET ACTION SENT");
			NetActionPolymorph action = new NetActionPolymorph(false);
			base.stateData.Original.networkComponent.QueueNetAction(action);
		}
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphEndState);
	}
}
