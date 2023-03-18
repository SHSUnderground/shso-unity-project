using System;
using UnityEngine;

public class PolymorphEndState : PolymorphBaseState
{
	private float _endTime;

	private float _delayTime;

	private bool _pendingOwnershipRequest;

	public PolymorphEndState(PolymorphStateProxy proxy)
		: base(proxy)
	{
		_delayTime = 0f;
		_endTime = 0f;
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		_endTime = Time.time + _delayTime;
		_pendingOwnershipRequest = false;
		if (!string.IsNullOrEmpty(base.stateData.RevertEffect) && base.stateData.PolymorphObject != null && EffectSequence.FindEffect(base.stateData.RevertEffect, base.stateData.PolymorphObject) == null)
		{
			base.stateData.CreateEffect(base.stateData.RevertEffect, base.stateData.PolymorphObject);
		}
		if (!(base.stateData.Polymorph != null))
		{
			return;
		}
		if (base.stateData.Polymorph.combatController.AwaitingPursuit())
		{
			base.stateData.Polymorph.behaviorManager.clearQueuedBehavior();
		}
		if (base.stateData.Polymorph.combatController.InPursuit())
		{
			base.stateData.Polymorph.behaviorManager.endBehavior();
			base.stateData.Polymorph.motionController.stopGently();
		}
		else if (base.stateData.Polymorph.combatController.InAttack())
		{
			base.stateData.Polymorph.behaviorManager.clearQueuedBehavior();
			BehaviorAttackBase behaviorAttackBase = base.stateData.Polymorph.behaviorManager.getBehavior() as BehaviorAttackBase;
			if (behaviorAttackBase != null)
			{
				behaviorAttackBase.CeaseAttack();
			}
		}
		if (base.stateData.Polymorph.brawlerCharacterAI != null)
		{
			base.stateData.Polymorph.brawlerCharacterAI.RunAI(false);
		}
	}

	public override void Update()
	{
		if (!(Time.time >= _endTime) || done)
		{
			return;
		}
		NetworkComponent polymorphNetworkComponent = base.stateData.GetPolymorphNetworkComponent();
		if (polymorphNetworkComponent == null || polymorphNetworkComponent.IsOwner() || polymorphNetworkComponent.IsOwnedBySomeoneElse())
		{
			if (polymorphNetworkComponent == null)
			{
				PolymorphController.LogPolymorphAction(base.stateData.PolymorphObject, base.stateData.Polymorph, "POLYMORPH END WITHOUT NETWORK COMPONENT");
			}
			else
			{
				PolymorphController.LogPolymorphAction(base.stateData.PolymorphObject, base.stateData.Polymorph, "OWNERSHIP KNOWN <" + polymorphNetworkComponent.NetOwnerId + ">");
			}
			done = true;
		}
		else if (!polymorphNetworkComponent.IsPendingOwner() && !_pendingOwnershipRequest)
		{
			PolymorphController.LogPolymorphAction(base.stateData.PolymorphObject, base.stateData.Polymorph, "OWNERSHIP REQUEST");
			AppShell.Instance.ServerConnection.Game.TakeOwnership(base.stateData.PolymorphObject, OnOwnershipChange);
			_pendingOwnershipRequest = true;
		}
	}

	public void Initialize(float delayTime)
	{
		_delayTime = delayTime;
		_endTime = 0f;
	}

	public override Type GetNextState()
	{
		return base.stateData.GetRevertState();
	}

	private void OnOwnershipChange(GameObject polymorph, bool assumedOwnership)
	{
		if (_pendingOwnershipRequest && !(base.stateData.PolymorphObject == null) && !(polymorph != base.stateData.PolymorphObject))
		{
			PolymorphController.LogPolymorphAction(base.stateData.PolymorphObject, base.stateData.Polymorph, "OWNERSHIP CHANGE RECEIVED");
			_pendingOwnershipRequest = false;
			done = true;
		}
	}
}
