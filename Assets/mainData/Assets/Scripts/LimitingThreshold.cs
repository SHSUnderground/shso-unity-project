using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class LimitingThreshold
{
	[CompilerGenerated]
	private int _003CCharacterCount_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CMaxPercent_003Ek__BackingField;

	public int CharacterCount
	{
		[CompilerGenerated]
		get
		{
			return _003CCharacterCount_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCharacterCount_003Ek__BackingField = value;
		}
	}

	public float MaxPercent
	{
		[CompilerGenerated]
		get
		{
			return _003CMaxPercent_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CMaxPercent_003Ek__BackingField = value;
		}
	}

	public LimitingThreshold(int characterCount, float maxPercent)
	{
		CharacterCount = characterCount;
		MaxPercent = maxPercent;
	}

	public LimitingThreshold(DataWarehouse data)
	{
		CharacterCount = data.GetInt("character_count");
		MaxPercent = data.GetFloat("max_percent");
	}

	public static void Sort(List<LimitingThreshold> thresholds)
	{
		thresholds.Sort(delegate(LimitingThreshold x, LimitingThreshold y)
		{
			return x.CharacterCount - y.CharacterCount;
		});
	}

	public static List<LimitingThreshold> LoadAndSort(DataWarehouse data, string path)
	{
		List<LimitingThreshold> list = new List<LimitingThreshold>();
		foreach (DataWarehouse item in data.GetIterator(path + "/threshold"))
		{
			list.Add(new LimitingThreshold(item));
		}
		Sort(list);
		return list;
	}
}
