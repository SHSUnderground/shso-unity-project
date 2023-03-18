using System.Collections.Generic;
using UnityEngine;

public class CombatEffectResister
{
	private CharacterSpawn.Type mTypeResisters;

	private HashSet<string> mCharacterResisters;

	private string mResisterEffect;

	public string ResisterEffect
	{
		get
		{
			return mResisterEffect;
		}
	}

	public void InitializeFromData(DataWarehouse resisterData)
	{
		int count = resisterData.GetCount("type_resister");
		for (int i = 0; i < count; i++)
		{
			switch (resisterData.GetString("type_resister", i).ToLower())
			{
			case "player":
				mTypeResisters |= CharacterSpawn.Type.Player;
				break;
			case "ai":
				mTypeResisters |= CharacterSpawn.Type.AI;
				break;
			case "boss":
				mTypeResisters |= CharacterSpawn.Type.Boss;
				break;
			}
		}
		mCharacterResisters = new HashSet<string>();
		count = resisterData.GetCount("character_resister");
		for (int j = 0; j < count; j++)
		{
			mCharacterResisters.Add(resisterData.GetString("character_resister", j));
		}
		mResisterEffect = resisterData.TryGetString("resister_effect", string.Empty);
	}

	public bool IsResister(GameObject target)
	{
		return IsTypeResister(target) || IsCharacterResister(target);
	}

	public bool IsTypeResister(GameObject target)
	{
		if (target == null)
		{
			return false;
		}
		SpawnData component = target.GetComponent<SpawnData>();
		return component != null && (mTypeResisters & component.spawnType) != 0;
	}

	public bool IsCharacterResister(GameObject target)
	{
		if (target == null)
		{
			return false;
		}
		return mCharacterResisters.Contains(target.name);
	}

	public bool ResisterHasEffect()
	{
		return !string.IsNullOrEmpty(mResisterEffect);
	}
}
