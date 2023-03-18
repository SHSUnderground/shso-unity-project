using UnityEngine;

public class EvilCharacterSpawn : CharacterSpawn
{
	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.FinalSpawnSetup(newCharacter, spawnData);
		CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals.combatController != null)
		{
			characterGlobals.combatController.faction = CombatController.Faction.Enemy;
		}
		if (characterGlobals.brawlerCharacterAI != null)
		{
			characterGlobals.brawlerCharacterAI.InitializeAttackFrequency(3f);
		}
	}

	public override void Awake()
	{
		base.Awake();
		forceAICombat = true;
		SpawnName = "evil_" + CharacterName;
	}

	public override void SetCharacterName(string newName)
	{
		base.SetCharacterName(newName);
		SpawnName = "evil_" + CharacterName;
	}
}
