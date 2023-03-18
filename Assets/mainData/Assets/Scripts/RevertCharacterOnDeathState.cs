using System;

public class RevertCharacterOnDeathState : PolymorphBaseState
{
	private string _revertDeathEffect;

	public RevertCharacterOnDeathState(PolymorphStateProxy proxy)
		: base(proxy)
	{
		_revertDeathEffect = string.Empty;
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		BehaviorPolymorph behaviorPolymorph = base.stateData.Original.behaviorManager.getBehavior() as BehaviorPolymorph;
		if (behaviorPolymorph != null)
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "DEATH REVERT");
			base.stateData.Original.behaviorManager.endBehavior();
		}
		else
		{
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, base.stateData.PolymorphObject, base.stateData.Polymorph, "DEATH REVERT - failed to end behavior");
		}
		if (GUIManager.Instance != null && GUIManager.Instance.GetTargetedEnemy() == base.stateData.PolymorphObject)
		{
			GUIManager.Instance.DetachAttackingIndicator();
		}
		if (!string.IsNullOrEmpty(_revertDeathEffect) && base.stateData.OriginalObject == base.stateData.Original.polymorphController.GetOriginalObject())
		{
			base.stateData.CreateEffect(_revertDeathEffect, base.stateData.OriginalObject);
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

	public void Initialize(string revertDeathEffect)
	{
		_revertDeathEffect = revertDeathEffect;
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphNullState);
	}
}
