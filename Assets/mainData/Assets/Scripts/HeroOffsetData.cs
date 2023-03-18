using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class HeroOffsetData
{
	public enum OffsetMode
	{
		Normal
	}

	private const float heroIdleDefault = 2f;

	private const float heroJumpUpDefault = 2.7f;

	private const float heroJumpDownDefault = 2.7f;

	private const float heroJumpRunUpDefault = 2.7f;

	private const float heroJumpRunDownDefault = 2.7f;

	private const string jumpUpAnimName = "jump_up";

	private const string jumpDownAnimName = "jump_down";

	private const string jumpRunUpAnimName = "jump_run_up";

	private const string jumpRunDownAnimName = "jump_run_down";

	private Hashtable offsetData = new Hashtable();

	private Hashtable ascendIntervalData = new Hashtable();

	private Hashtable descendIntervalData = new Hashtable();

	private static Hashtable allHeroOffsetData;

	[CompilerGenerated]
	private float _003CDefaultOffset_003Ek__BackingField;

	public float DefaultOffset
	{
		[CompilerGenerated]
		get
		{
			return _003CDefaultOffset_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDefaultOffset_003Ek__BackingField = value;
		}
	}

	public static Hashtable AllHeroOffsetData
	{
		get
		{
			if (allHeroOffsetData == null)
			{
				allHeroOffsetData = new Hashtable();
			}
			return allHeroOffsetData;
		}
	}

	public void AddModeOffsetEntry(OffsetMode mode)
	{
		if (!offsetData.ContainsKey(mode))
		{
			offsetData[mode] = new Hashtable();
		}
	}

	public void AddModeAscendIntervalEntry(OffsetMode mode)
	{
		if (!ascendIntervalData.ContainsKey(mode))
		{
			ascendIntervalData[mode] = new Hashtable();
		}
	}

	public void AddModeDescendIntervalEntry(OffsetMode mode)
	{
		if (!descendIntervalData.ContainsKey(mode))
		{
			descendIntervalData[mode] = new Hashtable();
		}
	}

	public void AddAnimationOffsetEntry(OffsetMode mode, string animation, float offset, bool overwrite)
	{
		if (offsetData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)offsetData[mode];
			if (!hashtable.ContainsKey(animation) || overwrite)
			{
				hashtable[animation] = offset;
			}
		}
	}

	public void AddAnimationAscendIntervalEntry(OffsetMode mode, string animation, float timeInterval)
	{
		if (ascendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)ascendIntervalData[mode];
			if (!ascendIntervalData.ContainsKey(animation))
			{
				hashtable[animation] = timeInterval;
			}
		}
	}

	public void AddAnimationDescendIntervalEntry(OffsetMode mode, string animation, float timeInterval)
	{
		if (descendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)descendIntervalData[mode];
			if (!descendIntervalData.ContainsKey(animation))
			{
				hashtable[animation] = timeInterval;
			}
		}
	}

	public bool DoesOffsetExistForAnimation(OffsetMode mode, string animation)
	{
		if (offsetData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)offsetData[mode];
			return hashtable.Contains(animation);
		}
		return false;
	}

	public bool DoesAscendIntervalExistForAnimation(OffsetMode mode, string animation)
	{
		if (ascendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)ascendIntervalData[mode];
			return hashtable.Contains(animation);
		}
		return false;
	}

	public bool DoesDescendIntervalExistForAnimation(OffsetMode mode, string animation)
	{
		if (descendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)descendIntervalData[mode];
			return hashtable.Contains(animation);
		}
		return false;
	}

	public float GetAnimationOffset(OffsetMode mode, string animation)
	{
		if (offsetData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)offsetData[mode];
			if (hashtable.ContainsKey(animation))
			{
				return (float)hashtable[animation];
			}
		}
		return 0f;
	}

	public float GetAscendTimeInterval(OffsetMode mode, string animation)
	{
		if (ascendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)ascendIntervalData[mode];
			if (hashtable.ContainsKey(animation))
			{
				return (float)hashtable[animation];
			}
		}
		return 0f;
	}

	public float GetDescendTimeInterval(OffsetMode mode, string animation)
	{
		if (descendIntervalData.ContainsKey(mode))
		{
			Hashtable hashtable = (Hashtable)descendIntervalData[mode];
			if (hashtable.ContainsKey(animation))
			{
				return (float)hashtable[animation];
			}
		}
		return 0f;
	}

	public static void AddHeroOffsetData(string characterName, DataWarehouse data)
	{
		if (!AllHeroOffsetData.ContainsKey(characterName))
		{
			allHeroOffsetData[characterName] = new HeroOffsetData();
		}
		HeroOffsetData heroOffsetData = (HeroOffsetData)AllHeroOffsetData[characterName];
		heroOffsetData.DefaultOffset = 2f;
		heroOffsetData.AddModeOffsetEntry(OffsetMode.Normal);
		heroOffsetData.AddModeAscendIntervalEntry(OffsetMode.Normal);
		heroOffsetData.AddModeDescendIntervalEntry(OffsetMode.Normal);
		heroOffsetData.AddAnimationOffsetEntry(OffsetMode.Normal, "jump_up", 2.7f, true);
		heroOffsetData.AddAnimationOffsetEntry(OffsetMode.Normal, "jump_down", 2.7f, true);
		heroOffsetData.AddAnimationOffsetEntry(OffsetMode.Normal, "jump_run_up", 2.7f, true);
		heroOffsetData.AddAnimationOffsetEntry(OffsetMode.Normal, "jump_run_down", 2.7f, true);
		if (data != null)
		{
			float defaultValue = heroOffsetData.DefaultOffset = data.TryGetFloat("//offset_data/default_offset", 2f);
			IEnumerable<DataWarehouse> iterator = data.GetIterator("//offset_data/animation_offset_data/animation_offset");
			IEnumerable<DataWarehouse> iterator2 = data.GetIterator("//offset_data/animation_ascend_interval_data/ascend_interval");
			IEnumerable<DataWarehouse> iterator3 = data.GetIterator("//offset_data/animation_descend_interval_data/descend_interval");
			foreach (DataWarehouse item in iterator)
			{
				string animation = item.TryGetString("name", "generic_animation_name");
				float offset = item.TryGetFloat("offset", defaultValue);
				string text = item.TryGetString("mode", "normal");
				OffsetMode mode = OffsetMode.Normal;
				switch (text)
				{
				}
				heroOffsetData.AddAnimationOffsetEntry(mode, animation, offset, true);
			}
			foreach (DataWarehouse item2 in iterator2)
			{
				string animation2 = item2.TryGetString("name", "generic_animation_name");
				float timeInterval = item2.TryGetFloat("interval", 0f);
				string text2 = item2.TryGetString("mode", "normal");
				OffsetMode mode2 = OffsetMode.Normal;
				switch (text2)
				{
				}
				heroOffsetData.AddAnimationAscendIntervalEntry(mode2, animation2, timeInterval);
			}
			foreach (DataWarehouse item3 in iterator3)
			{
				string animation3 = item3.TryGetString("name", "generic_animation_name");
				float timeInterval2 = item3.TryGetFloat("interval", 0f);
				string text3 = item3.TryGetString("mode", "normal");
				OffsetMode mode3 = OffsetMode.Normal;
				switch (text3)
				{
				}
				heroOffsetData.AddAnimationDescendIntervalEntry(mode3, animation3, timeInterval2);
			}
		}
	}
}
