using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BaseCharacterCombination
{
	protected List<List<string>> mCharacterList;

	protected bool mIsApplied;

	private List<string> mActiveCharacters;

	private string mDisplayName;

	private string mDisplayIcon;

	private string mCombatEffect;

	[CompilerGenerated]
	private string _003CId_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CDisplayDescription_003Ek__BackingField;

	public string Id
	{
		[CompilerGenerated]
		get
		{
			return _003CId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CId_003Ek__BackingField = value;
		}
	}

	public bool IsApplied
	{
		get
		{
			return mIsApplied;
		}
	}

	public List<string> ActiveCharacters
	{
		get
		{
			return mActiveCharacters;
		}
	}

	public string DisplayName
	{
		get
		{
			return mDisplayName;
		}
	}

	public string DisplayIcon
	{
		get
		{
			return mDisplayIcon;
		}
	}

	public string DisplayDescription
	{
		[CompilerGenerated]
		get
		{
			return _003CDisplayDescription_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CDisplayDescription_003Ek__BackingField = value;
		}
	}

	public string CombatEffect
	{
		get
		{
			return mCombatEffect;
		}
	}

	public BaseCharacterCombination()
	{
		Id = string.Empty;
		mCharacterList = null;
		mIsApplied = false;
		mActiveCharacters = new List<string>();
		mDisplayName = string.Empty;
		mDisplayIcon = string.Empty;
		mCombatEffect = string.Empty;
		DisplayDescription = string.Empty;
	}

	public abstract bool IsActive(List<string> characters);

	public virtual bool Apply(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		if (!HasCharacter(player.name))
		{
			return false;
		}
		return true;
	}

	public virtual void Erase(GameObject player)
	{
	}

	public virtual void InitializeFromData(DataWarehouse data)
	{
		Id = data.TryGetString("combo_id", Id);
		mDisplayName = data.TryGetString("display_name", mDisplayName);
		mDisplayIcon = data.TryGetString("display_icon", mDisplayIcon);
		mCombatEffect = data.TryGetString("combat_effect", mCombatEffect);
		DisplayDescription = data.TryGetString("display_description", DisplayDescription);
	}

	public int GetCharacterCount()
	{
		return (mCharacterList != null) ? mCharacterList.Count : 0;
	}

	public string GetCharacter(int index)
	{
		if (mCharacterList == null)
		{
			return string.Empty;
		}
		if (index < 0 || index >= mCharacterList.Count)
		{
			return string.Empty;
		}
		return (mCharacterList[index].Count <= 0) ? string.Empty : mCharacterList[index][0];
	}

	public bool HasCharacter(string character)
	{
		foreach (List<string> mCharacter in mCharacterList)
		{
			if (mCharacter != null && mCharacter.Contains(character))
			{
				return true;
			}
		}
		return false;
	}

	public void GetActiveCharacters(List<string> characters)
	{
		mActiveCharacters.Clear();
		foreach (string character in characters)
		{
			if (HasCharacter(character))
			{
				mActiveCharacters.Add(character);
			}
		}
	}

	public int GetActiveComboMin()
	{
		int num = 0;
		if (mCharacterList != null)
		{
			foreach (List<string> mCharacter in mCharacterList)
			{
				if (mCharacter != null)
				{
					foreach (string item in mCharacter)
					{
						if (mActiveCharacters.Contains(item))
						{
							num++;
							break;
						}
					}
				}
			}
			return num;
		}
		return num;
	}

	public int GetCharacterMatchCount(List<string> characters)
	{
		int num = 0;
		foreach (string character in characters)
		{
			if (HasCharacter(character))
			{
				num++;
			}
		}
		return num;
	}

	public bool ApplyCombinationBuff(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		CombatController component = player.GetComponent<CombatController>();
		if (component == null)
		{
			return false;
		}
		component.createCombatEffect(CombatEffect, component, true);
		return true;
	}

	public void EraseCombinationBuff(GameObject player)
	{
		if (!(player == null))
		{
			CombatController component = player.GetComponent<CombatController>();
			if (!(component == null))
			{
				component.removeCombatEffect(CombatEffect);
			}
		}
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (List<string> mCharacter in mCharacterList)
		{
			if (mCharacter != null)
			{
				string text2 = string.Empty;
				if (mCharacter.Count > 1)
				{
					text += "Combo Family:\n";
					text2 = "\t";
				}
				foreach (string item in mCharacter)
				{
					string text3 = text;
					text = text3 + text2 + "Combo Character: " + item + "\n";
				}
			}
		}
		return text;
	}
}
