using System;

public class PolymorphFactionState : PolymorphBaseState
{
	public PolymorphFactionState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		PolymorphFactionStateData polymorphFactionStateData = base.stateData as PolymorphFactionStateData;
		base.stateData.Original.combatController.Charm(polymorphFactionStateData.NewFaction, polymorphFactionStateData.CanFactionDamage, polymorphFactionStateData.CanCharmBreak, polymorphFactionStateData.Charmer);
	}

	public override void Leave(Type nextState)
	{
		base.Leave(nextState);
		PolymorphFactionStateData polymorphFactionStateData = base.stateData as PolymorphFactionStateData;
		base.stateData.Original.combatController.EndCharm(polymorphFactionStateData.OldFaction);
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphEndState);
	}
}
