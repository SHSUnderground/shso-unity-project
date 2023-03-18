using System;

public class PolymorphSpawnState : PolymorphBaseState
{
	public PolymorphSpawnState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		base.stateData.Original.behaviorManager.clearQueuedBehavior();
		base.stateData.Original.behaviorManager.endBehavior();
		if (base.stateData.Original.combatController.isKilled)
		{
			PolymorphController.LogPolymorphError(base.stateData.OriginalObject, base.stateData.Original, "LOCAL POLYMORPH SPAWN - original is killed");
			return;
		}
		PolymorphCharacterStateData polymorphCharacterStateData = base.stateData as PolymorphCharacterStateData;
		if (polymorphCharacterStateData.PolymorphIsCharacter)
		{
			if (base.stateData.Original.spawnData == null || base.stateData.Original.spawnData.spawner == null)
			{
				PolymorphController.LogPolymorphError(base.stateData.OriginalObject, base.stateData.Original, "LOCAL POLYMORPH SPAWN - original does not have a spawner");
				return;
			}
			CharacterSpawn spawner = base.stateData.Original.spawnData.spawner;
			spawner.goNetId = GoNetId.Invalid;
			if (spawner.spawnerNetwork != null && spawner.IsLocal && !AppShell.Instance.ServerConnection.IsGameHost())
			{
				spawner.IsLocal = false;
			}
			PolymorphController.LogPolymorphAction(base.stateData.OriginalObject, base.stateData.Original, "LOCAL POLYMORPH SPAWN <" + polymorphCharacterStateData.Form + ">");
			base.stateData.Original.polymorphController.SpawnPolymorphCharacter(polymorphCharacterStateData.Form, spawner, polymorphCharacterStateData.CombatEffect);
		}
		else
		{
			base.stateData.PolymorphObject = base.stateData.Original.polymorphController.SpawnPolymorphObject(polymorphCharacterStateData.Form);
			done = true;
		}
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphCharacterState);
	}
}
