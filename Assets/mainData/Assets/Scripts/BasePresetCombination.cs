using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BasePresetCombination : BaseCharacterCombination
{
	[CompilerGenerated]
	private int _003CComboCharacterMin_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CComboMin_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CAllPlayers_003Ek__BackingField;

	public int ComboCharacterMin
	{
		[CompilerGenerated]
		get
		{
			return _003CComboCharacterMin_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CComboCharacterMin_003Ek__BackingField = value;
		}
	}

	public int ComboMin
	{
		[CompilerGenerated]
		get
		{
			return _003CComboMin_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CComboMin_003Ek__BackingField = value;
		}
	}

	public bool AllPlayers
	{
		[CompilerGenerated]
		get
		{
			return _003CAllPlayers_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CAllPlayers_003Ek__BackingField = value;
		}
	}

	public BasePresetCombination()
	{
		ComboCharacterMin = 0;
		ComboMin = 0;
		AllPlayers = false;
		mIsApplied = false;
	}

	public override void InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		mCharacterList = new List<List<string>>();
		int count = data.GetCount("combo_family");
		for (int i = 0; i < count; i++)
		{
			DataWarehouse data2 = data.GetData("combo_family", i);
			if (data2 != null)
			{
				int count2 = data2.GetCount("combo_character");
				List<string> list = new List<string>(count2);
				for (int j = 0; j < count2; j++)
				{
					list.Add(data2.GetString("combo_character", j));
				}
				mCharacterList.Add(list);
			}
		}
		int count3 = data.GetCount("combo_character");
		for (int k = 0; k < count3; k++)
		{
			List<string> list2 = new List<string>(1);
			list2.Add(data.GetString("combo_character", k));
			mCharacterList.Add(list2);
		}
		ComboCharacterMin = data.TryGetInt("combo_character_min", 0);
		ComboMin = data.TryGetInt("combo_min", 0);
		AllPlayers = data.TryGetBool("all_players", false);
	}

	public override bool IsActive(List<string> characters)
	{
		if (characters == null)
		{
			return false;
		}
		if (characters.Count < ComboCharacterMin)
		{
			return false;
		}
		GetActiveCharacters(characters);
		if (base.ActiveCharacters.Count < ComboCharacterMin)
		{
			return false;
		}
		if (GetActiveComboMin() < ComboMin)
		{
			return false;
		}
		if (AllPlayers && base.ActiveCharacters.Count < characters.Count)
		{
			return false;
		}
		return true;
	}

	public override bool Apply(GameObject player)
	{
		if (!base.Apply(player))
		{
			return false;
		}
		if (!ApplyCombinationBuff(player))
		{
			return false;
		}
		mIsApplied = true;
		return true;
	}

	public override void Erase(GameObject player)
	{
		EraseCombinationBuff(player);
		mIsApplied = false;
	}

	public override string ToString()
	{
		string str = "Combo Character Min: " + ComboCharacterMin + "\nCombo Min: " + ComboMin + "\nAll Players: " + AllPlayers + "\n";
		return str + base.ToString();
	}
}
