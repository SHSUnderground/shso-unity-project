using UnityEngine;

public class EntityPolymorphMessage : ShsEventMessage
{
	public GameObject original;

	public GameObject polymorph;

	public CharacterSpawn.Type originalType;

	public CharacterSpawn.Type polymorphType;

	public bool revert;

	public EntityPolymorphMessage(GameObject original, GameObject polymorph, CharacterSpawn.Type originalType, CharacterSpawn.Type polymorphType, bool revert)
	{
		this.original = original;
		this.polymorph = polymorph;
		this.originalType = originalType;
		this.polymorphType = polymorphType;
		this.revert = revert;
	}
}
