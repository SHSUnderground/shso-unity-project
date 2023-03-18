using System;
using UnityEngine;

[Serializable]
public class FoodSequence
{
	public string Animation;

	public GameObject Food;

	public EffectSequence effectSequencePrefab;

	public string GetName()
	{
		if (!string.IsNullOrEmpty(Animation))
		{
			return Animation;
		}
		return "<Animation>";
	}
}
