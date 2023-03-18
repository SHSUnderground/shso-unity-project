using UnityEngine;

public class GoodCharacterSpawn : CharacterSpawn
{
	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.FinalSpawnSetup(newCharacter, spawnData);
		CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals.combatController != null)
		{
			characterGlobals.combatController.faction = CombatController.Faction.Player;
		}
		if (characterGlobals.brawlerCharacterAI != null)
		{
			characterGlobals.brawlerCharacterAI.InitializeAttackFrequency(3f);
		}
		SpawnData component = spawnData.spawner.GetComponent<SpawnData>();
		if ((bool)component)
		{
			component.spawnType |= Type.Ally;
		}
	}

	public override void Awake()
	{
		base.Awake();
		forceAICombat = true;
		SpawnName = "good_" + CharacterName;
	}

	public override void SetCharacterName(string newName)
	{
		base.SetCharacterName(newName);
		SpawnName = "good_" + CharacterName;
	}
}
