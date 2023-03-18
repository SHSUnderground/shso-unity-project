using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum StatType
	{
		Unknown,
		Health,
		Power,
		AttackPower,
		SpecialPower
	}

	protected CharacterGlobals charGlobals;

	public CharacterStat[] Stats;

	public void InitializeFromData(DataWarehouse statsData)
	{
		Dictionary<StatType, CharacterStat> dictionary = new Dictionary<StatType, CharacterStat>();
		List<CharacterStat> list = new List<CharacterStat>();
		StatType[] array = (StatType[])Enum.GetValues(typeof(StatType));
		foreach (StatType statType in array)
		{
			CharacterStat characterStat = new CharacterStat();
			characterStat.StatType = statType;
			dictionary.Add(statType, characterStat);
			list.Add(characterStat);
		}
		foreach (DataWarehouse item in statsData.GetIterator("stat"))
		{
			string text = item.TryGetString("stat_type", "*missing*");
			CharacterStat characterStat2 = null;
			switch (text)
			{
			case "Health":
				characterStat2 = dictionary[StatType.Health];
				break;
			case "Power":
				characterStat2 = dictionary[StatType.Power];
				break;
			case "AttackPower":
				characterStat2 = dictionary[StatType.AttackPower];
				break;
			case "SpecialPower":
				characterStat2 = dictionary[StatType.SpecialPower];
				break;
			default:
				CspUtils.DebugLog("Unknown stat type " + text);
				continue;
			}
			characterStat2.TimedUpdateDelay = item.TryGetFloat("stat_change_pusle_rate", 0.25f);
			characterStat2.TimedUpdateChange = item.TryGetFloat("stat_change_amount", 0f);
			characterStat2.InitialValue = item.TryGetFloat("initial_value", 0f);
			characterStat2.InitialMaximum = item.TryGetFloat("initial_maximum", 100f);
			characterStat2.levelScaling = item.TryGetFloat("level_scaling", 0f);
			if (characterStat2.TimedUpdateChange != 0f)
			{
				characterStat2.StartTimedUpdates();
			}
		}
		Stats = list.ToArray();
	}

	public CharacterStat GetStat(StatType Type)
	{
		if (Stats == null)
		{
			return null;
		}
		CharacterStat result = null;
		CharacterStat[] stats = Stats;
		foreach (CharacterStat characterStat in stats)
		{
			if (characterStat.StatType == Type)
			{
				result = characterStat;
				break;
			}
		}
		return result;
	}

	protected virtual void Start()
	{
		if (Stats != null)
		{
			CharacterStat[] stats = Stats;
			foreach (CharacterStat characterStat in stats)
			{
				characterStat.Character = base.gameObject;
				characterStat.MaximumValue = characterStat.InitialMaximum;
				characterStat.Value = characterStat.InitialValue;
			}
		}
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
	}

	protected virtual void Update()
	{
		if ((!(charGlobals != null) || !(charGlobals.networkComponent != null) || !charGlobals.networkComponent.IsOwnedBySomeoneElse()) && Stats != null)
		{
			for (int i = 0; i < Stats.Length; i++)
			{
				Stats[i].PulseStatChange();
			}
		}
	}
}
