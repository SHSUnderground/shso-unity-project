using UnityEngine;

public class PetSpawn : CharacterSpawn
{
	public NPCPath path;

	public string npcName;

	public float spawnPriority;

	public GameObject interactivityPrefab;

	public virtual string GetNextDefaultName()
	{
		return "Fluffy";
	}

	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		CspUtils.DebugLog("PetSpawn FinalSpawnSetup ");
		base.FinalSpawnSetup(newCharacter, spawnData);
		if (interactivityPrefab != null)
		{
			CspUtils.DebugLog("  +interactive");
			GameObject child = Object.Instantiate(interactivityPrefab) as GameObject;
			Utils.AttachGameObject(newCharacter, child);
		}
	}
}
