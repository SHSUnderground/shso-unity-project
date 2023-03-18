using System.Collections.Generic;
using UnityEngine;

public class BaseDynamicCombination : BaseCharacterCombination
{
	public static readonly int TRIGGER_CHARACTER;

	public static readonly int CHAIN_CHARACTER = 1;

	public static readonly int CHARACTER_COUNT = 2;

	public override void InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		mCharacterList = new List<List<string>>(CHARACTER_COUNT);
		mCharacterList[TRIGGER_CHARACTER] = new List<string>(1);
		mCharacterList[TRIGGER_CHARACTER].Add(data.GetString("trigger_character"));
		mCharacterList[CHAIN_CHARACTER] = new List<string>(1);
		mCharacterList[CHAIN_CHARACTER].Add(data.GetString("chain_character"));
	}

	public override bool IsActive(List<string> characters)
	{
		if (characters == null)
		{
			return false;
		}
		int characterMatchCount = GetCharacterMatchCount(characters);
		return characterMatchCount >= CHARACTER_COUNT;
	}

	public override bool Apply(GameObject player)
	{
		return true;
	}

	public override void Erase(GameObject player)
	{
	}

	public bool IsActive(string triggerCharacter, string chainCharacter)
	{
		if (mCharacterList == null || mCharacterList.Count < CHARACTER_COUNT)
		{
			return false;
		}
		if (mCharacterList[TRIGGER_CHARACTER].Count <= 0 || mCharacterList[CHAIN_CHARACTER].Count <= 0)
		{
			return false;
		}
		if (triggerCharacter != mCharacterList[TRIGGER_CHARACTER][0])
		{
			return false;
		}
		if (chainCharacter != mCharacterList[CHAIN_CHARACTER][0])
		{
			return false;
		}
		return true;
	}

	public string GetTriggerCharacter()
	{
		return GetCharacter(TRIGGER_CHARACTER);
	}

	public string GetChainCharacter()
	{
		return GetCharacter(CHAIN_CHARACTER);
	}

	public override string ToString()
	{
		string str = "Dynamic Combination:\n";
		return str + base.ToString();
	}
}
