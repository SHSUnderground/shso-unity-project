using System;
using UnityEngine;

[Serializable]
public class EffectSequenceReference
{
	public string characterSequenceName;

	public EffectSequence directSequenceReference;

	public EffectSequence GetSequence(GameObject character)
	{
		if (directSequenceReference != null)
		{
			return directSequenceReference;
		}
		return ResolveNamedSequence(character);
	}

	public EffectSequence ResolveNamedSequence(GameObject character)
	{
		if (character == null || string.IsNullOrEmpty(characterSequenceName))
		{
			return directSequenceReference;
		}
		EffectSequenceList component = character.GetComponent<EffectSequenceList>();
		if (component != null)
		{
			GameObject gameObject = component.TryGetEffectSequencePrefabByName(characterSequenceName) as GameObject;
			if (gameObject != null)
			{
				return gameObject.GetComponent<EffectSequence>();
			}
		}
		return directSequenceReference;
	}
}
