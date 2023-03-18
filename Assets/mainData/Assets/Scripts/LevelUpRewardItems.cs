using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public class LevelUpRewardItems
{
	private readonly Dictionary<int, List<int>> levelItems = new Dictionary<int, List<int>>();

	[CompilerGenerated]
	private string _003CHeroName_003Ek__BackingField;

	public string HeroName
	{
		[CompilerGenerated]
		get
		{
			return _003CHeroName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CHeroName_003Ek__BackingField = value;
		}
	}

	public LevelUpRewardItems(string heroName)
	{
		HeroName = heroName;
	}

	public List<int> GetRewardsForLevel(int level)
	{
		List<int> value;
		if (!levelItems.TryGetValue(level, out value))
		{
			value = new List<int>();
			levelItems.Add(level, value);
		}
		return value;
	}

	public void AddItem(int level, int ownableTypeId)
	{
		List<int> rewardsForLevel = GetRewardsForLevel(level);
		rewardsForLevel.Add(ownableTypeId);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("heroName: " + HeroName);
		foreach (int key in levelItems.Keys)
		{
			List<int> list = levelItems[key];
			foreach (int item in list)
			{
				stringBuilder.AppendLine("  lvl " + key + ": item " + item);
			}
		}
		return stringBuilder.ToString();
	}
}
