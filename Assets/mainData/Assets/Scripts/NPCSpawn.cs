using UnityEngine;

public class NPCSpawn : CharacterSpawn
{
	private static int defaultNameIndex = 0;

	private static string[] DefaultNames = new string[37]
	{
		"Buffy",
		"Jody",
		"Sissy",
		"Inky",
		"Pinky",
		"Blinky",
		"Clyde",
		"Aimee",
		"Fred",
		"Ethel",
		"Lucy",
		"Ricky",
		"Bubba",
		"Barney",
		"Wilma",
		"Velma",
		"Louise",
		"Stuffy",
		"Smokey",
		"Piggy",
		"Dirk",
		"Gus",
		"Hoss",
		"Sam",
		"Max",
		"Todd",
		"Tim",
		"Lana",
		"Anne",
		"Brett",
		"Luke",
		"Theo",
		"Zabrina",
		"Trina",
		"Tom",
		"Ziggy",
		"Zibby"
	};

	public NPCPath path;

	public string npcName;

	public float spawnPriority;

	public GameObject interactivityPrefab;

	public virtual string GetNextDefaultName()
	{
		if (defaultNameIndex >= DefaultNames.Length)
		{
			defaultNameIndex = 0;
		}
		return DefaultNames[defaultNameIndex++];
	}

	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.FinalSpawnSetup(newCharacter, spawnData);
		if (interactivityPrefab != null)
		{
			GameObject child = Object.Instantiate(interactivityPrefab) as GameObject;
			Utils.AttachGameObject(newCharacter, child);
		}
	}
}
