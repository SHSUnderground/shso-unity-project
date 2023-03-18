using System;
using UnityEngine;

public class RevertCharacterState : PolymorphBaseState
{
	public RevertCharacterState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		bool flag = false;
		NetworkComponent polymorphNetworkComponent = base.stateData.GetPolymorphNetworkComponent();
		if (polymorphNetworkComponent == null || polymorphNetworkComponent.IsOwner())
		{
			flag = true;
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "REVERT NET ACTION SENT");
			NetActionPolymorph action = new NetActionPolymorph(true);
			base.stateData.Original.networkComponent.QueueNetActionIgnoringOwnership(action);
		}
		BehaviorPolymorph behaviorPolymorph = base.stateData.Original.behaviorManager.getBehavior() as BehaviorPolymorph;
		if (behaviorPolymorph != null)
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "REVERT LOCAL " + flag);
			base.stateData.Original.behaviorManager.endBehavior();
		}
		else
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "REVERT - failed to end behavior");
		}
		if (GUIManager.Instance != null && GUIManager.Instance.GetTargetedEnemy() == base.stateData.PolymorphObject)
		{
			GUIManager.Instance.DetachAttackingIndicator();
		}
		if (!string.IsNullOrEmpty(base.stateData.RevertEffect) && base.stateData.PolymorphObject != null)
		{
			EffectSequence.TransferEffect(base.stateData.RevertEffect, base.stateData.PolymorphObject, base.stateData.OriginalObject);
			GameObject gameObject = EffectSequence.FindEffect(base.stateData.RevertEffect, base.stateData.OriginalObject);
			if (gameObject != null)
			{
				TintObject component = gameObject.GetComponent<TintObject>();
				if (component != null)
				{
					component.StartFade(TintObject.TintFadeType.TintFadeOut);
				}
			}
		}
		base.stateData.Original.polymorphController.OnDestroyPolymorph(base.stateData.PolymorphObject);
		if (base.stateData.Polymorph != null)
		{
			base.stateData.Polymorph.spawnData.Despawn(EntityDespawnMessage.despawnType.polymorph, false, false);
		}
		else if (base.stateData.PolymorphObject != null)
		{
			Utils.DelayedDestroy(base.stateData.PolymorphObject);
		}
		base.stateData.PolymorphObject = null;
		base.stateData.Polymorph = null;
		done = true;
	}

	public override void Leave(Type nextState)
	{
		base.Leave(nextState);
		if (base.stateData.Original != null && base.stateData.Original.polymorphController != null)
		{
			base.stateData.Original.polymorphController.OnEndPolymorph();
		}
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphNullState);
	}
}
