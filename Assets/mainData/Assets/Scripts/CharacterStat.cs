using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStat
{
	private class StatModifier
	{
		public float statChangePulseRate = 0.25f;

		public float statChangeAmount;

		public float lastChangePulse;
	}

	public class StatChangeEvent : ShsEventMessage
	{
		public CharacterStats.StatType StatType;

		public GameObject Character;

		public float OldValue;

		public float NewValue;

		public float MaxValue;

		public StatChangeEvent(CharacterStats.StatType StatType, GameObject Character, float OldValue, float NewValue, float MaxValue)
		{
			this.StatType = StatType;
			this.Character = Character;
			this.OldValue = OldValue;
			this.NewValue = NewValue;
			this.MaxValue = MaxValue;
		}
	}

	public class StatMinimumEvent : StatChangeEvent
	{
		public StatMinimumEvent(CharacterStats.StatType StatType, GameObject Character, float OldValue, float NewValue, float MaxValue)
			: base(StatType, Character, OldValue, NewValue, MaxValue)
		{
		}
	}

	public class StatMaximumEvent : StatChangeEvent
	{
		public StatMaximumEvent(CharacterStats.StatType StatType, GameObject Character, float OldValue, float NewValue, float MaxValue)
			: base(StatType, Character, OldValue, NewValue, MaxValue)
		{
		}
	}

	public CharacterStats.StatType StatType;

	public float InitialValue;

	public float InitialMaximum = 100f;

	protected float InitialMinimum;

	protected GameObject character;

	public float levelScaling;

	private int _level;

	protected float value;

	protected float maximumValue = 100f;

	protected float minimumValue;

	protected float statChangePulseRate = 0.25f;

	protected float statChangeAmount;

	protected float lastChangePulse;

	protected bool timedUpdatesActive;

	private int statKey;

	private Dictionary<int, StatModifier> statModifiers = new Dictionary<int, StatModifier>();

	public float Value
	{
		get
		{
			if (StatType == CharacterStats.StatType.AttackPower)
			{
				if (level <= 20)
				{
					return 0f;
				}
				int num = level - 20;
				if (level >= 40)
				{
					num += 5;
				}
				return num;
			}
			if (levelScaling <= 0f)
			{
				return value;
			}
			return value + (float)level * levelScaling;
		}
		set
		{
			float oldValue = this.value;
			this.value = value;
			if (this.value < minimumValue)
			{
				this.value = minimumValue;
			}
			if (this.value > maximumValue)
			{
				this.value = maximumValue;
			}
			lastChangePulse = Time.timeSinceLevelLoad;
			onValueChanged(oldValue);
		}
	}

	public float Percent
	{
		get
		{
			float num = maximumValue - minimumValue;
			return (value - minimumValue) / num * 100f;
		}
		set
		{
			float num = maximumValue - minimumValue;
			Value = minimumValue + value / 100f * num;
		}
	}

	public float MaximumValue
	{
		get
		{
			return maximumValue;
		}
		set
		{
			maximumValue = value;
			if (this.value > maximumValue)
			{
				Value = maximumValue;
			}
		}
	}

	public float MinimumValue
	{
		get
		{
			return minimumValue;
		}
		set
		{
			minimumValue = value;
			if (this.value < minimumValue)
			{
				Value = minimumValue;
			}
		}
	}

	public GameObject Character
	{
		get
		{
			return character;
		}
		set
		{
			character = value;
		}
	}

	public bool TimedUpdateActive
	{
		get
		{
			return timedUpdatesActive;
		}
	}

	public float TimedUpdateChange
	{
		get
		{
			return statChangeAmount;
		}
		set
		{
			statChangeAmount = value;
		}
	}

	public float TimedUpdateDelay
	{
		get
		{
			return statChangePulseRate;
		}
		set
		{
			statChangePulseRate = value;
		}
	}

	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
		}
	}

	public virtual void StartTimedUpdates()
	{
		lastChangePulse = Time.timeSinceLevelLoad;
		timedUpdatesActive = true;
	}

	public virtual void StartTimedUpdates(float TimeBetweenUpdates, float ChangeAmount)
	{
		statChangeAmount = ChangeAmount;
		statChangePulseRate = TimeBetweenUpdates;
		lastChangePulse = Time.timeSinceLevelLoad;
		timedUpdatesActive = true;
	}

	public virtual void StopTimedUpdates()
	{
		timedUpdatesActive = false;
	}

	public virtual int AddTimedUpdate(float TimeBetweenUpdates, float ChangeAmount)
	{
		int num = statKey;
		statKey++;
		StatModifier statModifier = new StatModifier();
		statModifier.statChangeAmount = ChangeAmount;
		statModifier.statChangePulseRate = TimeBetweenUpdates;
		statModifier.lastChangePulse = Time.timeSinceLevelLoad;
		statModifiers.Add(num, statModifier);
		return num;
	}

	public virtual void RemoveTimedUpdate(int key)
	{
		statModifiers.Remove(key);
	}

	public void PulseStatChange()
	{
		if (timedUpdatesActive && Time.timeSinceLevelLoad > lastChangePulse + statChangePulseRate)
		{
			float oldValue = value;
			float num = (Time.timeSinceLevelLoad - lastChangePulse) * statChangeAmount / statChangePulseRate;
			value += num;
			if (value > maximumValue)
			{
				value = maximumValue;
			}
			if (value < minimumValue)
			{
				value = minimumValue;
			}
			lastChangePulse = Time.timeSinceLevelLoad;
			onValueChanged(oldValue);
		}
		StatModifier statModifier = null;
		foreach (StatModifier value2 in statModifiers.Values)
		{
			if (statModifier == null)
			{
				statModifier = value2;
			}
			else if (value2.statChangeAmount > statModifier.statChangeAmount)
			{
				statModifier = value2;
			}
		}
		if (statModifier != null && Time.timeSinceLevelLoad > statModifier.lastChangePulse + statModifier.statChangePulseRate)
		{
			float oldValue2 = value;
			float num2 = (Time.timeSinceLevelLoad - statModifier.lastChangePulse) * statModifier.statChangeAmount / statModifier.statChangePulseRate;
			value += num2;
			if (value > maximumValue)
			{
				value = maximumValue;
			}
			if (value < minimumValue)
			{
				value = minimumValue;
			}
			statModifier.lastChangePulse = Time.timeSinceLevelLoad;
			onValueChanged(oldValue2);
		}
	}

	protected virtual void onValueChanged(float OldValue)
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.Fire(character, new StatChangeEvent(StatType, character, OldValue, Value, MaximumValue));
			if (Value == MinimumValue)
			{
				AppShell.Instance.EventMgr.Fire(character, new StatMinimumEvent(StatType, character, OldValue, Value, MaximumValue));
			}
			else if (Value == MaximumValue)
			{
				AppShell.Instance.EventMgr.Fire(character, new StatMaximumEvent(StatType, character, OldValue, Value, MaximumValue));
			}
		}
		else
		{
			CspUtils.DebugLog("Scene does not have a created AppShell!");
		}
	}
}
